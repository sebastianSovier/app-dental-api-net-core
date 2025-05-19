using System.Text;
using System.Text.Json;

namespace app_dental_api.Middlewares
{
    public class RequestDecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EncryptionService _encryptionService;
        private readonly ILogger<RequestDecryptionMiddleware> _logger;

        public RequestDecryptionMiddleware(RequestDelegate next, EncryptionService encryptionService, ILogger<RequestDecryptionMiddleware> logger)
        {
            _next = next;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType != null &&
    context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (context.Request.Method.Equals(HttpMethods.Post) || context.Request.Method.Equals(HttpMethods.Put))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var encryptedBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(encryptedBody))
                {
                    string decryptedJson;
                    try
                    {
                        decryptedJson = _encryptionService.Decrypt(encryptedBody);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fallo en la desencriptación.");
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Solicitud inválida: fallo en la desencriptación.");
                        return;
                    }

                    try
                    {
                        JsonDocument.Parse(decryptedJson);
                    }
                    catch (JsonException je)
                    {
                        _logger.LogError(je, "JSON inválido después de la desencriptación.");
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Solicitud inválida: JSON inválido.");
                        return;
                    }

                    var bytes = Encoding.UTF8.GetBytes(decryptedJson);
                    context.Request.Body = new MemoryStream(bytes);
                    context.Request.ContentLength = bytes.Length;
                    context.Request.Body.Position = 0;
                }
            }

            await _next(context);
        }
    }
}
