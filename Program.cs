using UserManagementAPI.Middleware;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// Register Services (Dependency Injection)
// =============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register UserService as Scoped (new instance per HTTP request)
builder.Services.AddScoped<IUserService, UserService>();

// Add Swagger/OpenAPI support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "User Management API",
        Version = "v1",
        Description = "A simple ASP.NET Core Web API for managing users. Built with Copilot."
    });

    // Add API Key security definition to Swagger
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key needed to access endpoints. Enter: my-secret-api-key-12345",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// =============================================
// Middleware Pipeline (ORDER MATTERS!)
// =============================================

// 1. Error handling middleware (outermost - catches all errors)
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Authentication middleware
app.UseMiddleware<ApiKeyAuthMiddleware>();

// 4. Swagger UI (only in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        c.RoutePrefix = "swagger";
    });
}

// 5. Routing and controllers
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Welcome endpoint
app.MapGet("/", () => Results.Ok(new
{
    message = "Welcome to the User Management API!",
    swagger = "/swagger",
    endpoints = new[]
    {
        "GET    /api/users         - Get all users",
        "GET    /api/users/{id}    - Get user by ID",
        "POST   /api/users         - Create new user",
        "PUT    /api/users/{id}    - Update user",
        "DELETE /api/users/{id}    - Delete user"
    },
    note = "Add header: X-API-Key: my-secret-api-key-12345"
}));

app.Run();