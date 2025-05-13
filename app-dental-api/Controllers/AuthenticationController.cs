using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo;
using Negocio;
using System.Security.Claims;
using Utilidades;
using static Modelo.AuthenticationModel;

namespace app_dental_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAntiforgery antiforgery;
        private readonly IConfiguration _config;
        private readonly UtilidadesApiss utils = new UtilidadesApiss();
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="antiforgery">antiforgery instance.</param>
        public AuthenticationController(IAntiforgery antiforgery, IConfiguration config)
        {
            this.antiforgery = antiforgery;
            _config = config;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("health")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Health()
        {
            return Ok();
        }
        [HttpPost]
        [AllowAnonymous]
        [ActionName("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] EmptyRequest request)
        {
            this.HttpContext.Session.Clear();
            this.Response.Cookies.Delete("Q");
            this.Response.Cookies.Delete("X-Q");

            await Task.CompletedTask;

            return this.NoContent();
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ActionName("loginPaciente")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginPaciente(
            [FromBody] LoginRequest request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();
            PasswordPacienteModel password = new PasswordPacienteModel();
            try
            {
                usuario = await Login.ObtenerPacienteAdmin(request.rut);
                if (usuario.id_perfil == "4")
                {
                    return Ok(new LoginResponse()
                    {
                        access_Token = "",
                        auth = false,
                        id = 0,
                        login = false,
                        message = "usuario inhabilitado."
                    });
                }
                password = await Password.ObtenerPasswordPaciente(usuario.id_paciente.ToString());
                if (!(request.rut == usuario.rut + usuario.dv) || !(utils.ComparePassword(request.password, password.contrasena!)))
                {
                    response.Add("Error", "Invalid username or password");
                    return StatusCode(403, response);
                }
                string token = await Task.Run(() => utils.GenerateJwtToken(request.rut, "Paciente", _config));
                return Ok(new LoginResponse()
                {
                    access_Token = token,
                    auth = true,
                    id = Convert.ToInt64(usuario.id_perfil),
                    login = true
                });
            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ActionName("withoutLoginPaciente")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> withoutLoginPaciente(
            [FromBody] WithoutLoginRequest request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();
            PasswordPacienteModel password = new PasswordPacienteModel();
            try
            {

                usuario = await Login.ObtenerPacienteAdmin(request.rut);
                password = await Password.ObtenerPasswordPaciente(usuario.id_paciente.ToString());
                if (usuario.rut != null)
                {
                    if (usuario.id_perfil == "4")
                    {
                        return Ok(new LoginResponse()
                        {
                            access_Token = "",
                            auth = false,
                            id = 0,
                            login = false,
                            message = "usuario inhabilitado."
                        });

                    }
                    if (password.contrasena != null)
                    {
                        return Ok(new LoginResponse()
                        {
                            access_Token = "",
                            auth = false,
                            id = 0,
                            login = false,
                            message = "Agende hora a traves de su portal privado."
                        });

                    }
                    else
                    {
                        string tokens = await Task.Run(() => utils.GenerateJwtToken(request.rut, "Paciente", _config));
                        return Ok(new LoginResponse()
                        {
                            access_Token = tokens,
                            auth = true,
                            id = 1,
                            login = false,
                            message = "Cree contraseña para continuar."
                        });

                    }

                }
                string token = await Task.Run(() => utils.GenerateJwtToken(request.rut, "Paciente", _config));
                return Ok(new LoginResponse()
                {
                    access_Token = token,
                    auth = true,
                    id = 1,
                    login = false
                });


            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("obtenerDatosPaciente")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerDatosPaciente(
            [FromBody] EmptyRequest request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();
            PasswordPacienteModel password = new PasswordPacienteModel();
            ObtenerPacienteRequest requestLogin = new ObtenerPacienteRequest();

            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                requestLogin.rut = username;
                usuario = await Login.ObtenerPaciente(requestLogin.rut);

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al obtener datos paciente.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ActionName("loginProfesional")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginProfesional(
            [FromBody] LoginRequest request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            ProfesionalModel usuario = new ProfesionalModel();
            ProfesionalModel usuario1 = new ProfesionalModel();
            PasswordProfesionalModel password = new PasswordProfesionalModel();
            try
            {
                string token = "";

                usuario = await Login.ObtenerAdministrador(request.rut);
                usuario1 = await Login.ObtenerProfesionalAdmin(request.rut);
                if (usuario.rut == null && usuario1.rut == null)
                {
                    response.Add("Error", "Invalid username or password");
                    return StatusCode(403, response);
                }

                if (usuario.rut == null)
                {
                    usuario = usuario1;
                }
                if (usuario.id_perfil == "4")
                {
                    return Ok(new LoginResponse()
                    {
                        access_Token = "",
                        auth = false,
                        id = 0,
                        login = false,
                        message = "usuario inhabilitado."
                    });
                }
                password = await Password.ObtenerPasswordProfesional(usuario.id_profesional.ToString());

                if (!(request.rut == usuario.rut + usuario.dv) || !(utils.ComparePassword(request.password, password.contrasena!)))
                {
                    response.Add("Error", "Invalid username or password");
                    return StatusCode(403, response);
                }
                if (usuario.id_perfil.Equals("3"))
                {
                    token = await Task.Run(() => utils.GenerateJwtToken(request.rut, "Administrador", _config));
                }
                else
                {
                    token = await Task.Run(() => utils.GenerateJwtToken(request.rut, "Profesional", _config));
                }
                return Ok(new LoginResponse()
                {
                    access_Token = token,
                    auth = true,
                    id = Convert.ToInt64(usuario.id_perfil),
                    login = true

                });
            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al validar profesional.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("crearPaciente")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearPaciente(PacienteModel request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                request.rut = username;
                usuario = await Login.ObtenerPaciente(request.rut);
                if (usuario.nombres == null)
                {
                    Login.CrearPaciente(request);
                    return Ok(new LoginResponse
                    {
                        auth = true

                    });
                }
                else
                {
                    return Ok(new LoginResponse
                    {
                        auth = false

                    });
                }

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("crearPasswordPaciente")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearPasswordPaciente(PacienteModel request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                request.rut = username;
                usuario = await Login.ObtenerPaciente(request.rut);
                if (usuario.nombres != null)
                {
                    PasswordPacienteModel paswordPaciente = await Password.ObtenerPasswordPaciente(usuario.id_paciente.ToString());
                    if (paswordPaciente.id_contrasena > 0)
                    {
                        response.Add("Error", "Usuario ya posee contraseña.");
                        return StatusCode(409, response);
                    }
                    PasswordPacienteModel passwordRequest = new PasswordPacienteModel();
                    passwordRequest.id_paciente = usuario.id_paciente;
                    passwordRequest.contrasena = request.contrasena;
                    Password.CrearPasswordPaciente(passwordRequest);
                    return Ok(new LoginResponse
                    {
                        auth = true

                    });
                }
                else
                {
                    response.Add("Error", "Usuario no registrado.");
                    return StatusCode(403, response);
                }

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("crearProfesional")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearProfesional(ProfesionalModel request)
        {
            LoginBo Login = new LoginBo(_config);
            PasswordBo Password = new PasswordBo(_config);
            var response = new Dictionary<string, string>();
            ProfesionalModel usuario = new ProfesionalModel();
            ProfesionalModel administrador = new ProfesionalModel();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            try
            {
                administrador = await Login.ObtenerAdministrador(username);
                if (administrador.nombres != null && administrador.id_perfil.Equals("3"))
                {
                    if (perfil.Equals("Administrador"))
                    {
                        usuario = await Login.ObtenerProfesional(request.rut!);
                        if (usuario.nombres == null)
                        {
                            long idGenerado = await Login.CrearProfesional(request);
                            PasswordProfesionalModel passwordRequest = new PasswordProfesionalModel();
                            passwordRequest.id_profesional = idGenerado;
                            passwordRequest.contrasena = request.rut;
                            await Password.CrearPasswordProfesional(passwordRequest);
                            return Ok(new LoginResponse
                            {
                                auth = true

                            });
                        }
                        else
                        {
                            return Ok(new LoginResponse
                            {
                                auth = false

                            });
                        }
                    }
                }
                response.Add("Error", "Hubo un problema al validar usuario.");
                return StatusCode(403, response);

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear profesional.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// GetXsrfToken.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("token")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetXsrfToken([FromBody] EmptyRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username != null)
            {
                var tokens = this.antiforgery.GetAndStoreTokens(this.HttpContext);
                this.Response.Cookies.Append("Q", tokens.RequestToken!, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.None,
                });

                return await Task.FromResult<IActionResult>(this.Ok(new { Token = tokens.RequestToken }));
            }
            else
            {
                response.Add("Error", "Hubo un problema al validar sesion.");
                return StatusCode(403, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("obtenerUsuarios")]
        [ProducesResponseType<List<ProfesionalesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> obtenerUsuarios(
            [FromBody] EmptyRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            List<ProfesionalModel> profesionales = new List<ProfesionalModel>();
            List<PacienteModel> pacientes = new List<PacienteModel>();
            try
            {
                if (username == null)
                {
                    response.Add("Error", "Hubo un problema al validar usuario.");
                    return StatusCode(403, response);
                }
                if (perfil.Equals("Administrador"))
                {
                    profesionales = await loginBo.ObtenerTodosProfesionales();
                    pacientes = await loginBo.ObtenerTodosPacientes();
                    if (profesionales == null && pacientes == null)
                    {
                        response.Add("Error", "Hubo un problema al obtener data.");
                        return StatusCode(403, response);
                    }

                    return Ok(new { Profesionales = profesionales, Pacientes = pacientes });
                }
                response.Add("Error", "Hubo un problema al validar usuario.");
                return StatusCode(403, response);

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("eliminarUsuario")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> eliminarUsuario(
            [FromBody] ObtenerPacienteRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            ProfesionalModel profesional = new ProfesionalModel();
            PacienteModel paciente = new PacienteModel();
            try
            {
                if (username == null)
                {
                    response.Add("Error", "Hubo un problema al validar usuario.");
                    return StatusCode(403, response);
                }
                if (perfil.Equals("Administrador"))
                {
                    string rutdv = request.rut;
                    paciente = await loginBo.ObtenerPacienteAdmin(rutdv);
                    profesional = await loginBo.ObtenerProfesionalAdmin(rutdv);
                    if (profesional.rut == null && paciente.rut == null)
                    {
                        response.Add("Error", "Hubo un problema al obtener data.");
                        return StatusCode(403, response);
                    }
                    if (profesional.rut != null)
                    {
                        await loginBo.EliminarProfesional(profesional.id_profesional.ToString());
                    }
                    else
                    {
                        await loginBo.EliminarPaciente(paciente.id_paciente.ToString());
                    }

                    return Ok(new LoginResponse
                    {
                        auth = true

                    });

                }
                else
                {
                    response.Add("Error", "Hubo un problema al validar usuario.");
                    return StatusCode(403, response);
                }

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("modificarPerfilUsuario")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> modificarPerfilUsuario(
            [FromBody] ModificarPerfilRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            ProfesionalModel profesional = new ProfesionalModel();
            ProfesionalModel administrador = new ProfesionalModel();
            PacienteModel paciente = new PacienteModel();
            try
            {
                if (username == null)
                {
                    response.Add("Error", "Hubo un problema al validar usuario.");
                    return StatusCode(403, response);
                }
                if (perfil.Equals("Administrador"))
                {
                    string rutdv = request.rut;
                    paciente = await loginBo.ObtenerPacienteAdmin(rutdv);
                    profesional = await loginBo.ObtenerProfesionalAdmin(rutdv);
                    administrador = await loginBo.ObtenerAdministrador(username);
                    if (administrador.rut == null)
                    {
                        response.Add("Error", "Hubo un problema al obtener data.");
                        return StatusCode(403, response);
                    }
                    if (profesional.rut == null && paciente.rut == null)
                    {
                        response.Add("Error", "Hubo un problema al obtener data.");
                        return StatusCode(403, response);
                    }
                    string id_perfil = request.perfil == "Profesional" ? "2" : request.perfil == "Paciente" ? "1" : request.perfil == "Administrador" ? "3" : "4";
                    if (profesional.rut != null)
                    {
                        await loginBo.ModificarPerfilProfesional(profesional.id_profesional.ToString(), id_perfil);
                    }
                    else
                    {
                        await loginBo.ModificarPerfilPaciente(paciente.id_paciente.ToString(), id_perfil);
                    }

                    return Ok(new LoginResponse
                    {
                        auth = true

                    });

                }
                else
                {
                    response.Add("Error", "Hubo un problema al validar usuario.");
                    return StatusCode(403, response);
                }

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema.");
                return StatusCode(500, response);
            }
        }
    }

}
