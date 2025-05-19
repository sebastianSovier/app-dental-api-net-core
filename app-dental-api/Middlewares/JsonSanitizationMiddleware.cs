using System.Text;
using System.Text.Json;
using Utilidades;

namespace app_dental_api.Middlewares
{
    public class JsonSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JsonSanitizationMiddleware> _logger;
        private readonly UtilidadesApiss _utils;

        public JsonSanitizationMiddleware(RequestDelegate next, ILogger<JsonSanitizationMiddleware> logger, UtilidadesApiss utils)
        {
            _next = next;
            _logger = logger;
            _utils = utils;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType?.Contains("application/json") == true)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    try
                    {
                        var json = JsonDocument.Parse(body);
                        var cleanedJson = CleanJson(json.RootElement);
                        var cleanedBody = JsonSerializer.Serialize(cleanedJson);

                        var bytes = Encoding.UTF8.GetBytes(cleanedBody);
                        context.Request.Body = new MemoryStream(bytes);
                        context.Request.ContentLength = bytes.Length;
                        context.Request.Body.Position = 0;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Error al analizar JSON en JsonSanitizationMiddleware.");
                    }
                }
            }

            await _next(context);
        }

        private object CleanJson(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => element.EnumerateObject().ToDictionary(
                    property => property.Name,
                    property => CleanJson(property.Value)),
                JsonValueKind.Array => element.EnumerateArray().Select(CleanJson).ToList(),
                JsonValueKind.String => _utils.LimpiaInyection(element.GetString() ?? string.Empty),
                _ => element.GetRawText()
            };
        }
    }
}
