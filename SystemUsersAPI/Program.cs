using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SystemUsersAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext configuration - Updated with more explicit error handling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    options.UseSqlServer(connectionString);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "System Users API", Version = "v1" });
});

// Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure middleware pipeline in EXACT ORDER
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "System Users API v1");
    c.RoutePrefix = string.Empty; // Makes Swagger UI the root page
});

app.UseCors("AllowFrontend"); // Apply CORS before routing


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();