using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SystemUsersAPI.Data;
using SystemUsersAPI.Models;

namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormBuilderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormBuilderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> GetForm(int id)
        {
           return await _context.Forms.AnyAsync(e => e.ActivityId == id);

        }

        [HttpGet("BasicForm")]
        public async Task<ActionResult<IEnumerable<Form>>> GetAllForm()
        {
            var forms = await _context.Forms.ToListAsync();
            return Ok(forms);
        }

        [HttpPost]
        public async Task<ActionResult<Form>> CreateForm([FromBody] FormBuilderRequest request)
        {
            // Configure JSON serialization options to handle circular references
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                WriteIndented = true
            };
            Console.WriteLine("Received request: " + request.FormName);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine("Received request 2 : ");
            // Ensure FormName is not empty
            if (string.IsNullOrEmpty(request.FormName))
            {
                request.FormName = "New Form";
            }

            // Ensure we have at least one column
            if (request.Columns == null || !request.Columns.Any())
            {
                return BadRequest("At least one column is required.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create Form
                var form = new Form
                {
                    FormName = request.ActivityName,
                    ActivityId =  int.Parse(request.ActivityId),
                    QueryText = request.queryDML,
                    CreatedBy = request.CreatedBy,

                };
                _context.Forms.Add(form);
                await _context.SaveChangesAsync();

                // Create FormColumns
                var columnDictionary = new Dictionary<string, FormColumn>();
                foreach (var columnDefinition in request.Columns)
                {
                    if (string.IsNullOrEmpty(columnDefinition.Name) || string.IsNullOrEmpty(columnDefinition.Type))
                    {
                        throw new InvalidOperationException("Column name and type are required.");
                    }

                    var column = new FormColumn
                    {
                        FormId = form.FormId,
                        ColumnName = columnDefinition.Name,
                        ColumnType = columnDefinition.Type,
                        IsLookup = columnDefinition.IsLookup
                    };
                    _context.FormColumns.Add(column);
                    columnDictionary[columnDefinition.Name] = column;
                }
                await _context.SaveChangesAsync();

                // Create FormData
                if (request.Data != null && request.Data.Any())
                {
                    foreach (var dataRow in request.Data)
                    {
                        if (dataRow.ColumnValues == null || !dataRow.ColumnValues.Any())
                        {
                            continue;
                        }

                        foreach (var columnData in dataRow.ColumnValues)
                        {
                            if (!columnDictionary.TryGetValue(columnData.ColumnName, out var column))
                            {
                                throw new InvalidOperationException($"Column {columnData.ColumnName} not found in the form definition.");
                            }

                            var formData = new FormData
                            {
                                FormId = form.FormId,
                                ColumnId = column.ColumnId,
                                RowId = 0,//dataRow.RowId,
                                Value = columnData.Value
                            };
                            _context.FormData.Add(formData);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return CreatedAtAction(nameof(CreateForm), new { id = form.FormId }, form);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public class FormBuilderRequest
    {
        public string? ActivityId { get; set; }
        
        public string? ActivityName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FormName { get; set; } = "New Form";
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one column is required.")]
        public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
        
        public string? CreatedBy { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public string queryDML { get; set; } = string.Empty;
        public List<FormDataRequest>? Data { get; set; }
    }

    public class ColumnDefinition
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        public bool IsLookup { get; set; }
    }

    public class FormDataRequest
    {
        public long RowId { get; set; }

        [MinLength(1, ErrorMessage = "At least one column value is required.")]
        public List<ColumnValue> ColumnValues { get; set; } = new List<ColumnValue>();
    }

    public class ColumnValue
    {
        [StringLength(100)]
        public string ColumnName { get; set; } = string.Empty;

        public string? Value { get; set; }
    }
}