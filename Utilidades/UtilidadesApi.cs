using ExcelDataReader;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelo;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using System.Globalization;
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
        public int? ValidateToken(string token, IConfiguration _config)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtKey"]!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
        public void createlogFile(string logMessage)
        {
            using (StreamWriter w = File.AppendText("logs//Log-" + DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".txt"))
            {
                WriteLog(logMessage, w);
            }

            using (StreamReader r = File.OpenText("logs//Log-" + DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".txt"))
            {
                ImprmLog(r);
                Console.WriteLine(logMessage);
            }
        }
        public static void WriteLog(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void ImprmLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()!) != null)
            {
                Console.WriteLine(line);
            }
        }

        public DataTable ConvertExcel(string file)
        {
            byte[] byteArray = Convert.FromBase64String(file);
            using MemoryStream memoryStream = new MemoryStream(byteArray);
            using BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8);
            byte[] data = binaryReader.ReadBytes(byteArray.Length);
            Console.WriteLine(BitConverter.ToString(data));
            Stream stream = new MemoryStream(data);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet();
            DataTable dataExcel = result.Tables[0];
            return dataExcel;
        }
        public bool validTimeSession(string fecha_actividad)
        {

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo chileTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");
            DateTime chileTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, chileTimeZone);
            TimeSpan timeDifference = chileTime - DateTime.Parse(fecha_actividad!);
            double minutesPassed = timeDifference.TotalMinutes;
            double secondsPassed = timeDifference.TotalSeconds;
            if (minutesPassed > 5 || (minutesPassed == 5 && secondsPassed > 0))
            {
                return true;
            }



            else
            {
                return false;
            }
        }
        public string generateRandomNumber()
        {

            Random rnd = new Random();
            Int64 number = rnd.NextInt64(1000000000, 9999999999);
            return number.ToString();
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
