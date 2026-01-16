using TodayApi.Data;
using TodayApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services using dependency injection.
// This makes it easy to swap implementations (e.g., use a database instead of in-memory data).
builder.Services.AddSingleton<IEventDataSource, InMemoryEventDataSource>();
builder.Services.AddSingleton<IDateInfoService, DateInfoService>();

// Add OpenAPI/Swagger support for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Today API",
        Version = "v1",
        Description = "An API that tells you what's special about today's date (Eastern Time)"
    });
});

var app = builder.Build();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal API endpoint: GET /today
// Returns information about what's special about today in Eastern Time.
app.MapGet("/today", (IDateInfoService dateInfoService) =>
{
    var result = dateInfoService.GetTodayInfo();
    return Results.Ok(result);
})
.WithName("GetToday")
.WithOpenApi()
.Produces<TodayApi.Models.TodayResponse>(StatusCodes.Status200OK)
.WithDescription("Gets information about what's special about today's date in Eastern Time (America/New_York)");

// Health check endpoint for production deployments
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .ExcludeFromDescription();

app.Run();
