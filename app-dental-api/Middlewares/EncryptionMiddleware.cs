namespace app_dental_api.Middlewares;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utilidades;

public class EncryptionMiddleware
{
    private readonly EncryptionService encryptionService;
    private readonly RequestDelegate next;
    private readonly IAntiforgery antiforgery;
    private readonly ILogger<EncryptionMiddleware> logger;
    private readonly UtilidadesApiss utils = new UtilidadesApiss();
    public EncryptionMiddleware(RequestDelegate next, IAntiforgery antiforgery, ILogger<EncryptionMiddleware> logger,
        EncryptionService encryptionService)
    {
        this.next = next;
        this.antiforgery = antiforgery;
        this.encryptionService = encryptionService;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (this.IsExcludedPath(context.Request.Path))
        {
            await this.next(context);
            return;
        }

        try
        {
            if (context.Request.Method.Equals(HttpMethods.Post) || context.Request.Method.Equals(HttpMethods.Put))
            {
                var excludedPaths = new HashSet<string>
            {
                "/api/Authentication/logout",
                "/api/Authentication/loginPaciente",
                "/api/Authentication/loginProfesional",
                "/api/Authentication/withoutLoginPaciente",
                "/api/Authentication/token",
                "/api/Profesional/obtenerProfesionalesPuntuacion"
            };

                if (!excludedPaths.Contains(context.Request.Path))
                {
                    var antiForgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                    var tokens = antiForgery.GetTokens(context);

                    try
                    {
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

                        await antiForgery.ValidateRequestAsync(context);
                    }
                    catch (AntiforgeryValidationException ex)
                    {
                        this.logger.LogError(ex, "Error en EncryptionMiddleware antiForgery.");
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden.");
                        return;
                    }
                }
            }

            int maxRequestBodySize = 1024 * 1024;
            if (context.Request.Path.Equals("/api/Profesional/cargarImagenExamen"))
            {
                maxRequestBodySize = 50 * 1024 * 1024;
            }
            if (context.Request.ContentLength > maxRequestBodySize)
            {
                context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
                await context.Response.WriteAsync("Request body too large.");
                return;
            }

            var decryptedStream = await DecryptRequestBody(context.Request);
            if (decryptedStream == Stream.Null) return;
            context.Request.Body = decryptedStream;
            context.Request.Body.Position = 0;

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
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
                }
                catch (JsonException ex)
                {
                    this.logger.LogError(ex, "Error parsing JSON en EncryptionMiddleware.");
                }
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await this.next(context);

            await EncryptResponseBody(context.Response, originalBodyStream);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error en EncryptionMiddleware.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Internal Server Error");
        }
    }


    private bool IsExcludedPath(PathString path)
    {
        return path.Value?.Contains("swagger") == true || path.Value?.Contains("index") == true;
    }
    private object CleanJson(System.Text.Json.JsonElement element)
    {
        switch (element.ValueKind)
        {
            case System.Text.Json.JsonValueKind.Object:
                var dict = new Dictionary<string, object>();
                foreach (var property in element.EnumerateObject())
                {
                    dict[property.Name] = CleanJson(property.Value);
                }
                return dict;
            case System.Text.Json.JsonValueKind.Array:
                var list = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(CleanJson(item));
                }
                return list;
            case System.Text.Json.JsonValueKind.String:
                return this.utils.LimpiaInyection(element.GetString() ?? string.Empty);
            default:
                return element.GetRawText();
        }
    }
    private async Task<Stream> DecryptRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        string encryptedBody = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        if (string.IsNullOrEmpty(encryptedBody))
            return request.Body;

        string decryptedJson;
        try
        {
            decryptedJson = encryptionService.Decrypt(encryptedBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Decryption failed");
            request.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await request.HttpContext.Response.WriteAsync("Bad request: Decryption failed.");
            return Stream.Null;
        }

        try { JsonDocument.Parse(decryptedJson); }
        catch (JsonException je)
        {
            logger.LogError(je, "Invalid JSON after decryption");
            request.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await request.HttpContext.Response.WriteAsync("Bad request: Invalid JSON.");
            return Stream.Null;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(decryptedJson);
        var ms = new MemoryStream(bytes);

        request.ContentType = "application/json";
        request.Headers["Content-Type"] = "application/json";
        request.ContentLength = bytes.Length;

        return ms;
    }



    private async Task EncryptResponseBody(HttpResponse response, Stream originalBodyStream)
    {
        if (response.StatusCode == StatusCodes.Status204NoContent)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            await response.Body.CopyToAsync(originalBodyStream);
            return;
        }
        response.Body.Seek(0, SeekOrigin.Begin);
        var plainText = await new StreamReader(response.Body).ReadToEndAsync();

        var encryptedText = this.encryptionService.Encrypt(plainText);

        var jsonResponse = new { data = encryptedText };

        var jsonString = JsonSerializer.Serialize(jsonResponse);
        var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

        response.ContentLength = jsonBytes.Length;

        response.Body.Seek(0, SeekOrigin.Begin);
        response.ContentType = "application/json";
        await originalBodyStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
    }
}