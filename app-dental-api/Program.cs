using app_dental_api.ConfigurationInjection;
using app_dental_api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using Utilidades;


const string corsPolicy = "_appdentalapiOrigin";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetupCorsService(builder, corsPolicy);

builder.Host.ConfigureSerilog();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder
    .Services
    .AddControllers()
    .AddJsonOptions(
        static option =>
        {
            option
                .JsonSerializerOptions
                .Converters
                .Add(new JsonStringEnumConverter());

            option
                .JsonSerializerOptions
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

builder
    .Services
    .AddHsts(
        static options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(60);
        });
builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "Q";
    options.HeaderName = "X-Q";
    options.Cookie.HttpOnly = false;
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
    options.Configure(builder.Configuration.GetSection("Kestrel"));


    options.AddServerHeader = false;
});
builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtIssuer"],
                        ValidAudience = builder.Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
                    };
                });


builder.Services.AddAuthorization();
builder.Services.AddSingleton<UtilidadesApiss>();

var app = builder.Build();
var culture = CultureInfo.CreateSpecificCulture("es-CL");
var dateformat = new DateTimeFormatInfo
{
    ShortDatePattern = "dd/MM/yyyy",
    LongDatePattern = "dd/MM/yyyy HH:mm:ss tt",
};
culture.DateTimeFormat = dateformat;

var supportedCultures = new[]
{
    culture,
};
app.UseRequestLocalization(
    new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture(culture),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures,
    });
app.UseCors(corsPolicy);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var error = new { error = "Ocurrió un error", detail = exceptionHandlerPathFeature?.Error.Message };

        await context.Response.WriteAsJsonAsync(error);
    });
});
// Seguridad antes que nada
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cache-Control", "max-age=604800");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    context.Response.Headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload";
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
    var origin = context.Request.Headers.Origin.FirstOrDefault();

    if (!string.IsNullOrEmpty(origin) && allowedOrigins != null && !allowedOrigins.Contains(origin))
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Forbidden\"}");
        return;
    }

    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Endpoint no encontrado\"}");
    }
});


app.UseSession();
app.UseAntiforgery();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

if (builder.Configuration["UseEncrypt"] == "true")
{
    var encryptionService = new EncryptionService(builder.Configuration["KeyCripto"]);


    app.UseMiddleware<AntiforgeryMiddleware>();
    app.UseMiddleware<RequestDecryptionMiddleware>(encryptionService);
    app.UseMiddleware<JsonSanitizationMiddleware>();
    app.UseMiddleware<ResponseEncryptionMiddleware>(encryptionService);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();

}

app.MapControllers();

app.Run();
