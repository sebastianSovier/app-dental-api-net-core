using app_dental_api.ConfigurationInjection;
using app_dental_api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;


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

    //options.Configure(builder.Configuration.GetSection("Kestrel"));


    options.AddServerHeader = false;
});
builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtIssuer"],
                        ValidAudience = builder.Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
                    };
                });


builder.Services.AddAuthorization();

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

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandlingPath = "/error",
    AllowStatusCode404Response = true
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

    await next();
});

app.UseSession();
app.UseAntiforgery();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<EncryptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.MapControllers();

app.Run();
