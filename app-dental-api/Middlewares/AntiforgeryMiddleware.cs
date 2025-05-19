using Microsoft.AspNetCore.Antiforgery;

namespace app_dental_api.Middlewares
{
    public class AntiforgeryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<AntiforgeryMiddleware> _logger;
        private static readonly HashSet<string> _excludedPaths = new()
    {
        "/api/Authentication/logout",
        "/api/Authentication/loginPaciente",
        "/api/Authentication/loginProfesional",
        "/api/Authentication/withoutLoginPaciente",
        "/api/Authentication/token",
        "/api/Profesional/obtenerProfesionalesPuntuacion"
    };

        public AntiforgeryMiddleware(RequestDelegate next, IAntiforgery antiforgery, ILogger<AntiforgeryMiddleware> logger)
        {
            _next = next;
            _antiforgery = antiforgery;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context.Request.Method.Equals(HttpMethods.Post) || context.Request.Method.Equals(HttpMethods.Put)) &&
                !_excludedPaths.Contains(context.Request.Path))
            {
                try
                {
                    var tokens = _antiforgery.GetAndStoreTokens(context);
                    if (!context.Request.Cookies.ContainsKey("Q"))
                    {
                        context.Response.Cookies.Append("Q", tokens.RequestToken!, new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = false,
                            SameSite = SameSiteMode.None,
                            MaxAge = TimeSpan.FromMinutes(30),
                        });
                    }

                    await _antiforgery.ValidateRequestAsync(context);
                }
                catch (AntiforgeryValidationException ex)
                {
                    _logger.LogError(ex, "Error en AntiforgeryMiddleware.");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
