using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SystemUsersAPI.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FormName { get; set; } = string.Empty;
        
        public int ActivityId { get; set; } = 0;
        public string QueryText { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<FormColumn> FormColumns { get; set; } = new List<FormColumn>();
        
        [JsonIgnore]
        public virtual ICollection<FormData> FormData { get; set; } = new List<FormData>();
    }
    
    public class FormColumn
    {
        [Key]
        public int ColumnId { get; set; }
        
        [ForeignKey(nameof(Form))]
        public int FormId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ColumnName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string ColumnType { get; set; } = string.Empty;
        
        [Required]
        public bool IsLookup { get; set; } = false;
        
        // Navigation property
        [JsonIgnore]
        public virtual Form Form { get; set; } = null!;
        
        // Navigation property for FormData
        [JsonIgnore]
        public virtual ICollection<FormData> FormData { get; set; } = new List<FormData>();
    }
    
    public class FormData
    {
        [Key]
        public int DataId { get; set; }
        
        [ForeignKey(nameof(Form))]
        public int FormId { get; set; }
        
        [Required]
        public int RowId { get; set; }
        
        [ForeignKey(nameof(FormColumn))]
        public int ColumnId { get; set; }
        
        public string? Value { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual Form Form { get; set; } = null!;
        
        [JsonIgnore]
        public virtual FormColumn FormColumn { get; set; } = null!;
    }
    
}