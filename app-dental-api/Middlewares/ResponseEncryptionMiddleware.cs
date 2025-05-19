using System.Text;
using System.Text.Json;

namespace app_dental_api.Middlewares
{
    public class ResponseEncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EncryptionService _encryptionService;
        private readonly ILogger<ResponseEncryptionMiddleware> _logger;

        public ResponseEncryptionMiddleware(RequestDelegate next, EncryptionService encryptionService, ILogger<ResponseEncryptionMiddleware> logger)
        {
            _next = next;
            _encryptionService = encryptionService;
            _logger = logger;
        }
        private bool IsExcludedPath(PathString path)
        {
            return path.Value?.Contains("swagger") == true || path.Value?.Contains("index") == true;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (this.IsExcludedPath(context.Request.Path))
            {
                await _next(context);
                return;
            }
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);



            if (context.Response.StatusCode == StatusCodes.Status204NoContent)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await context.Response.Body.CopyToAsync(originalBodyStream);
                return;
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var plainText = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var encryptedText = _encryptionService.Encrypt(plainText);
            var jsonResponse = JsonSerializer.Serialize(new { data = encryptedText });
            var jsonBytes = Encoding.UTF8.GetBytes(jsonResponse);

            context.Response.ContentLength = jsonBytes.Length;
            context.Response.ContentType = "application/json";
            context.Response.Body = originalBodyStream;
            await context.Response.Body.WriteAsync(jsonBytes, 0, jsonBytes.Length);
        }
    }
}
