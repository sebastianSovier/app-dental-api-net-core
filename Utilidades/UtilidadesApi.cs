using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelo;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Utilidades
{
    public class UtilidadesApiss
    {
        public UtilidadesApiss()
        {

        }
        public string HashPassword(string paswordReq)
        {
            return BCrypt.Net.BCrypt.HashPassword(paswordReq);

        }
        public bool ComparePassword(string paswordReq, string passwordhash)
        {
            bool result = BCrypt.Net.BCrypt.Verify(paswordReq, passwordhash);

            return result;
        }

        public string GenerateJwtToken(string username, string perfil, IConfiguration _config)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, username)
        };
            if (perfil == "Paciente")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Paciente"));
            }
            else if (perfil == "Profesional")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Profesional"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToInt64(_config["JwtExpireDays"]!));
            DateTime utcNow = DateTime.UtcNow;

            DateTime chileanLocalTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcNow, "Pacific SA Standard Time");
            DateTime timeExpiry = chileanLocalTime.AddMinutes(Convert.ToInt64(_config["JwtDurationInMinutes"]));
            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtIssuer"],
                claims,
                expires: timeExpiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task CreateLogFileAsync(string logMessage)
        {
            string logFileName = $"Log-{DateTime.Now:dd-MM-yyyy}.txt";
            string logFilePath = Path.Combine("logs", logFileName);

            Directory.CreateDirectory("logs");

            await using (StreamWriter w = File.AppendText(logFilePath))
            {
                await WriteLogAsync(logMessage, w);
            }

            using (StreamReader r = File.OpenText(logFilePath))
            {
                await PrintLogAsync(r);
            }
        }

        public static async Task WriteLogAsync(string logMessage, TextWriter w)
        {
            await w.WriteLineAsync();
            await w.WriteLineAsync($"Log Entry : {DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            await w.WriteLineAsync("  :");
            await w.WriteLineAsync($"  :{logMessage}");
            await w.WriteLineAsync("-------------------------------");
        }

        public static async Task PrintLogAsync(StreamReader r)
        {
            string? line;
            while ((line = await r.ReadLineAsync()) != null)
            {
                Console.WriteLine(line);
            }
        }



        public string LimpiaInyection(string valor)
        {
            if (valor != string.Empty)
            {
                if (valor != null)
                {
                    if (valor != "null")
                    {
                        string[] array =
                        {
                        "CREATE", "ALTER", "DROP", "TRUNCATE", "INSERT", "UPDATE", "DELETE", "SELECT", "&",
                        "FROM", "WHERE", "REPLACE", "FUNCTION", "TABLE", "COLUMN", "ROW", "DATABASE",
                        "%", "!", "$", "(", ")", "'", "[", "]", "{", "}", "+", "*", "=", "#",
                        "%", "!", "$", "(", ")", "'",
                        "[", "]", "+", "*", "=", "#",
                        "DOCTYPE", "applet", "basefont", "body", "button", "frame", "frameset", "head",
                        "html", "iframe", "img", "input", "label", "object", "script", "style", "table",
                        "textarea", "title", "javascript", "prompt", "read", "write", "cookie", "&#",
                        "vbscript", "language", "confirm", "alert", ">", "<", "'\'", "&lt", "&gt",
                        "msgbox", "USERNAME", "USER_ACCOUNT_URL", " USER_ID", "fromCharCode",
                        "SRC", "HTTP", "ONLOAD", "TYPE", "TEXT", "stylesheet", "CSS", "HREF", "LINK",
                        "BACKGROUND", "NAMESPACE", "XML", "DATASRC", "DATAFLD", "CurrentPage", "parameters",
                        "document", "querySelector", "jquery", "innerHTML", "outerHTML", "parseHTML", "insertAdjacentHTML",
                        "ONCLICK", "ONMOUSEOVER", "prepend", "wrapAll", "writeln", "QUERY", "showMessage",
                    };

                        foreach (string s in array)
                        {
                            bool b = valor.ToUpper().Contains(s);

                            if (b)
                            {
                                valor = valor.Replace(s.ToUpper(), string.Empty);
                            }
                        }

                        return valor.Trim();
                    }
                    else
                    {
                        return valor;
                    }
                }
                else
                {
                    return valor!;
                }
            }
            else
            {
                return valor;
            }
        }


        public T CleanObject<T>(T obj)
            where T : class
        {
            Type type = obj.GetType();

            foreach (PropertyInfo property in type.GetProperties())
            {
                // Ensure that the property is a string or a type that can be cleaned
                if (property.PropertyType == typeof(string))
                {
                    var currentValue = property.GetValue(obj)?.ToString();

                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        // Perform the cleaning operation
                        var cleanedValue = LimpiaInyection(currentValue);

                        // Set the cleaned value back into the property
                        property.SetValue(obj, cleanedValue);
                    }
                }
            }

            // Return the modified object
            return obj;
        }
        public string getDv(string rut)
        {
            return rut.Replace(".", "").Replace("-", "").Substring(rut.Length - 1, 1);
        }
        public string getRutWithoutDv(string rut)
        {
            return rut.Replace(".", "").Replace("-", "").Substring(0, rut.Length - 1);
        }
        public byte[] GeneratePdf(ProfesionalModel profesional, PacienteModel paciente, GuardarConsultaMedicaModel consultaMedica)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Consulta médica")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                          .Column(x =>
                          {
                              x.Item().Text($"Paciente: {paciente?.nombres ?? ""} {paciente?.apellido_paterno ?? ""} {paciente?.apellido_materno ?? ""}");
                              x.Item().Text($"Fecha de atención: {DateTime.Now.ToString("dd/MM/yyyy")}");
                              x.Item().Text($"Doctor: {profesional?.nombres ?? ""} {profesional?.apellido_paterno ?? ""} {profesional?.apellido_materno ?? ""}");
                              x.Item().Text($"Motivo consulta: {consultaMedica?.motivo_consulta ?? ""}");
                              x.Item().Text($"Observaciones: {consultaMedica?.observaciones ?? ""}");
                              x.Item().PaddingVertical(10).Text("Tratamientos:");

                              // Recorrer medicamentos
                              foreach (var tratamiento in consultaMedica.tratamientos)
                              {
                                  x.Item().Text($"• {tratamiento?.nombre_tratamiento ?? ""}").FontSize(12);
                                  x.Item().Text($"• {tratamiento?.descripcion ?? ""}").FontSize(12);
                                  x.Item().Text($"• {tratamiento?.valor?.ToString() ?? ""}").FontSize(12);
                              }
                          });

                    page.Footer()
            .AlignCenter()
             .Text(x =>
             {
                 x.Span("Página ");
                 x.CurrentPageNumber();
                 x.Span(" de ");
                 x.TotalPages();
             });
                });
            });

            // Devuelve el PDF como arreglo de bytes
            return document.GeneratePdf();
        }
    }
}
