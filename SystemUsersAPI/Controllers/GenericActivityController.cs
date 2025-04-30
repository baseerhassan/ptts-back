using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using SystemUsersAPI.Data;
using System.Text.Json;


namespace SystemUsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public GenericActivityController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/GenericActivity/{tableName}
        [HttpGet("{tableName}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll(string tableName)
        {
            try
            {
                if (!IsValidActivityTable(tableName))
                {
                    return BadRequest($"Invalid table name format. Table name must start with 'Act_'.");
                }

                if (!await TableExistsAsync(tableName))
                {
                    return NotFound($"Table '{tableName}' does not exist in the database.");
                }

                var data = await GetDataFromTableAsync(tableName) ?? new List<dynamic>();
                Console.WriteLine($"Data count: {data?.Count}");
                return Ok(data ?? new List<dynamic>()); // Will return 200 OK with [] if table is empty
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/GenericActivity/{tableName}/{id}
        [HttpGet("{tableName}/{id}")]
        public async Task<ActionResult<dynamic>> GetById(string tableName, int id)
        {
            try
            {
                // Validate table name format
                if (!IsValidActivityTable(tableName))
                {
                    return BadRequest($"Invalid table name format. Table name must start with 'Act_'.");
                }

                // Check if table exists in database
                if (!await TableExistsAsync(tableName))
                {
                    return NotFound($"Table '{tableName}' does not exist in the database.");
                }

                // Get data from the table by id
                var data = await GetDataByIdFromTableAsync(tableName, id);
                if (data == null)
                {
                    return NotFound($"No record found with id {id} in table '{tableName}'.");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/GenericActivity/{tableName}
        [HttpPost("{tableName}")]
        public async Task<ActionResult<dynamic>> Create(string tableName, [FromBody] JsonElement payload)
        {
            try
            {
                // Validate table name format
                if (!IsValidActivityTable(tableName))
                {
                    return BadRequest($"Invalid table name format. Table name must start with 'Act_'.");
                }

                // Check if table exists in database
                if (!await TableExistsAsync(tableName))
                {
                    return NotFound($"Table '{tableName}' does not exist in the database.");
                }

                // Insert data into the table
                var result = await InsertDataIntoTableAsync(tableName, payload);
                return CreatedAtAction(nameof(GetById), new { tableName, id = result }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/GenericActivity/{tableName}/{id}
        [HttpPut("{tableName}/{id}")]
        public async Task<IActionResult> Update(string tableName, int id, [FromBody] JsonElement payload)
        {
            try
            {
                // Validate table name format
                if (!IsValidActivityTable(tableName))
                {
                    return BadRequest($"Invalid table name format. Table name must start with 'Act_'.");
                }

                // Check if table exists in database
                if (!await TableExistsAsync(tableName))
                {
                    return NotFound($"Table '{tableName}' does not exist in the database.");
                }

                // Check if record exists
                var existingData = await GetDataByIdFromTableAsync(tableName, id);
                if (existingData == null)
                {
                    return NotFound($"No record found with id {id} in table '{tableName}'.");
                }

                // Update data in the table
                await UpdateDataInTableAsync(tableName, id, payload);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/GenericActivity/{tableName}/{id}
        [HttpDelete("{tableName}/{id}")]
        public async Task<IActionResult> Delete(string tableName, int id)
        {
            try
            {
                // Validate table name format
                if (!IsValidActivityTable(tableName))
                {
                    return BadRequest($"Invalid table name format. Table name must start with 'Act_'.");
                }

                // Check if table exists in database
                if (!await TableExistsAsync(tableName))
                {
                    return NotFound($"Table '{tableName}' does not exist in the database.");
                }

                // Check if record exists
                var existingData = await GetDataByIdFromTableAsync(tableName, id);
                if (existingData == null)
                {
                    return NotFound($"No record found with id {id} in table '{tableName}'.");
                }

                // Delete data from the table
                await DeleteDataFromTableAsync(tableName, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Helper Methods

        private bool IsValidActivityTable(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) && tableName.StartsWith("Act_", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<bool> TableExistsAsync(string tableName)
        {
            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT COUNT(1) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = @TableName";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@TableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<List<dynamic>> GetDataFromTableAsync(string tableName)
        {
            var connection = _context.Database.GetDbConnection();
            var result = new List<dynamic>();

            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT top 100 * FROM [PTMS].[dbo].{tableName}";

                using var reader = await command.ExecuteReaderAsync();
                var schemaTable = reader.GetSchemaTable();

                while (await reader.ReadAsync())
                {
                    var item = new ExpandoObject() as IDictionary<string, object>;
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        item[columnName] = value;
                    }
                    result.Add((ExpandoObject)item);
                }

                return result;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<dynamic> GetDataByIdFromTableAsync(string tableName, int id)
        {
            var connection = _context.Database.GetDbConnection();

            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName} WHERE Id = @Id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return null;
                }

                var item = new ExpandoObject() as IDictionary<string, object>;
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    item[columnName] = value;
                }

                return (ExpandoObject)item;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<int> InsertDataIntoTableAsync(string tableName, JsonElement payload)
        {
            // Get table schema
            var tableSchema = await GetTableSchemaAsync(tableName);
            
            // Build SQL insert statement
            var columnNames = new List<string>();
            var parameterNames = new List<string>();
            var parameters = new List<SqlParameter>();
            
            foreach (var property in payload.EnumerateObject())
            {
                var columnName = property.Name;
                // Skip if column doesn't exist in the table
                if (!tableSchema.Any(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase)))
                    continue;
                
                columnNames.Add(columnName);
                parameterNames.Add($"@{columnName}");
                
                var parameter = new SqlParameter($"@{columnName}", GetSqlDbType(property.Value));
                parameter.Value = GetValueFromJsonElement(property.Value);
                parameters.Add(parameter);
            }
            
            if (columnNames.Count == 0)
            {
                throw new InvalidOperationException("No valid columns provided for insert operation.");
            }
            
            var sql = $"INSERT INTO [PTMS].[dbo].{tableName} ({string.Join(", ", columnNames)}) " +
                     $"OUTPUT INSERTED.Id " +
                     $"VALUES ({string.Join(", ", parameterNames)})";
            
            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task UpdateDataInTableAsync(string tableName, int id, JsonElement payload)
        {
            // Get table schema
            var tableSchema = await GetTableSchemaAsync(tableName);
            
            // Build SQL update statement
            var setStatements = new List<string>();
            var parameters = new List<SqlParameter>();
            
            foreach (var property in payload.EnumerateObject())
            {
                var columnName = property.Name;
                // Skip if column doesn't exist in the table or if it's the Id column
                if (!tableSchema.Any(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase)) ||
                    string.Equals(columnName, "Id", StringComparison.OrdinalIgnoreCase))
                    continue;
                
                setStatements.Add($"{columnName} = @{columnName}");
                
                var parameter = new SqlParameter($"@{columnName}", GetSqlDbType(property.Value));
                parameter.Value = GetValueFromJsonElement(property.Value);
                parameters.Add(parameter);
            }
            
            if (setStatements.Count == 0)
            {
                throw new InvalidOperationException("No valid columns provided for update operation.");
            }
            
            var idParameter = new SqlParameter("@Id", SqlDbType.Int) { Value = id };
            parameters.Add(idParameter);
            
            var sql = $"UPDATE {tableName} SET {string.Join(", ", setStatements)} WHERE Id = @Id";
            
            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task DeleteDataFromTableAsync(string tableName, int id)
        {
            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM [PTMS].[dbo].{tableName} WHERE Id = @Id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<List<TableColumn>> GetTableSchemaAsync(string tableName)
        {
            var connection = _context.Database.GetDbConnection();
            var columns = new List<TableColumn>();
            
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 
                        COLUMN_NAME, 
                        DATA_TYPE,
                        IS_NULLABLE,
                        CHARACTER_MAXIMUM_LENGTH,
                        NUMERIC_PRECISION,
                        NUMERIC_SCALE
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@TableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    columns.Add(new TableColumn
                    {
                        ColumnName = reader.GetString(0),
                        DataType = reader.GetString(1),
                        IsNullable = reader.GetString(2) == "YES",
                        MaxLength = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3),
                        NumericPrecision = reader.IsDBNull(4) ? null : (byte?)reader.GetByte(4),
                        NumericScale = reader.IsDBNull(5) ? null : (int?)reader.GetInt32(5)
                    });
                }
                
                return columns;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private SqlDbType GetSqlDbType(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => SqlDbType.NVarChar,
                JsonValueKind.Number => SqlDbType.Decimal,
                JsonValueKind.True or JsonValueKind.False => SqlDbType.Bit,
                JsonValueKind.Null => SqlDbType.NVarChar,
                _ => SqlDbType.NVarChar
            };
        }

        private object GetValueFromJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.TryGetInt64(out var longValue) ? longValue : element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => DBNull.Value,
                _ => DBNull.Value
            };
        }

        #endregion
    }

    public class TableColumn
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
        public byte? NumericPrecision { get; set; }
        public int? NumericScale { get; set; }
    }
}