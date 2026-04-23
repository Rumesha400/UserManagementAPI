namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Logging Middleware - logs all incoming requests and outgoing responses
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            _logger.LogInformation(
                "[REQUEST] {Method} {Path} started at {Time}",
                context.Request.Method,
                context.Request.Path,
                startTime.ToString("yyyy-MM-dd HH:mm:ss")
            );

            await _next(context);

            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "[RESPONSE] {Method} {Path} completed with status {StatusCode} in {Elapsed}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                elapsed
            );
        }
    }

    /// <summary>
    /// Authentication Middleware - validates API key on all requests
    /// </summary>
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyAuthMiddleware> _logger;
        private const string ApiKeyHeader = "X-API-Key";
        private const string ValidApiKey = "my-secret-api-key-12345";

        public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for Swagger UI
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey))
            {
                _logger.LogWarning("[AUTH] Missing API key for {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "API key is missing. Add 'X-API-Key' header."
                });
                return;
            }

            if (apiKey != ValidApiKey)
            {
                _logger.LogWarning("[AUTH] Invalid API key attempt for {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Invalid API key."
                });
                return;
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Global Error Handling Middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Unhandled exception for {Method} {Path}", 
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal Server Error",
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
