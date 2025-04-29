// <copyright file="EncryptionMiddleware.cs" company="NeoSolTec Spa">
// Copyright (c) NeoSolTec Spa. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// More info about MIT license in https://opensource.org/license/mit
// </copyright>

namespace app_dental_api.Middlewares;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Utilidades;

#pragma warning disable SA1600 // Elements should be documented
public class EncryptionMiddleware
#pragma warning restore SA1600 // Elements should be documented
{
    private readonly RequestDelegate next;
    private readonly IAntiforgery antiforgery;
    private readonly ILogger<EncryptionMiddleware> logger;
    private readonly UtilidadesApiss utils = new UtilidadesApiss();
#pragma warning disable SA1600 // Elements should be documented
    public EncryptionMiddleware(RequestDelegate next, IAntiforgery antiforgery, ILogger<EncryptionMiddleware> logger)
#pragma warning restore SA1600 // Elements should be documented
    {
        this.next = next;
        this.antiforgery = antiforgery;
        this.logger = logger;
    }

#pragma warning disable SA1600 // Elements should be documented
    public async Task Invoke(HttpContext context)
#pragma warning restore SA1600 // Elements should be documented
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

            int maxRequestBodySize = 1024 * 1024; // 1 MB
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

            context.Request.EnableBuffering(); // permite leer el body varias veces
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // vuelve al inicio

            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(body);

                    var cleanedJson = CleanJson(json.RootElement);

                    var cleanedBody = System.Text.Json.JsonSerializer.Serialize(cleanedJson);

                    // Reemplaza el body
                    var bytes = System.Text.Encoding.UTF8.GetBytes(cleanedBody);
                    context.Request.Body = new MemoryStream(bytes);
                    context.Request.ContentLength = bytes.Length;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    this.logger.LogError(ex, "Error parsing JSON en EncryptionMiddleware.");
                }
            }

            await this.next(context);
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
                return element.GetRawText(); // Para valores numéricos, booleanos, null
        }
    }
}