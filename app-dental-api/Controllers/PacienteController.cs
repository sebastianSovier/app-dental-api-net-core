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
    public class PacienteController : BaseController
    {
        private readonly IAntiforgery antiforgery;
        private readonly IConfiguration _config;
        private readonly UtilidadesApiss utils = new UtilidadesApiss();
        /// <summary>
        /// Initializes a new instance of the <see cref="PacienteController"/> class.
        /// </summary>
        /// <param name="antiforgery">antiforgery instance.</param>
        public PacienteController(IAntiforgery antiforgery, IConfiguration config)
        {
            this.antiforgery = antiforgery;
            _config = config;
        }
        [HttpPost]
        [ActionName("guardarRespuestasPersonales")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarRespuestasPersonales(List<RespuestasInicialesModel> request)
        {
            LoginBo Login = new LoginBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                usuario = await Login.ObtenerPaciente(userSesion!);
                if (usuario.rut == null)
                {

                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                else
                {
                    foreach (var item in request)
                    {
                        item.id_paciente = usuario.id_paciente;
                        Login.GuardarRespuestasIniciales(item);

                    }
                    return Ok(new OkResponse
                    {
                        auth = true

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
        [ActionName("agendamientoPaciente")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> agendamientoPaciente(CrearAgendamientoModel request)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                usuario = await Login.ObtenerPaciente(userSesion!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                request.id_paciente = usuario.id_paciente.ToString();
                await AgendamientoBo.CrearAgendamiento(request, usuario.correo);



                return Ok(new OkResponse
                {
                    auth = true

                });


            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("modificarAgendamientoPaciente")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> modificarAgendamientoPaciente(ModificarAgendamientoModel request)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                request.id_paciente = usuario.id_paciente.ToString();
                await AgendamientoBo.ModificarAgendamiento(request, usuario.correo);



                return Ok(new OkResponse
                {
                    auth = true

                });


            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("modificarDerivacionAgendamientoPaciente")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> modificarDerivacionAgendamientoPaciente(ModificarAgendamientoModel request)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            ProfesionalModel usuario = new ProfesionalModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                usuario = await Login.ObtenerProfesional(userSesion!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                await AgendamientoBo.ModificarDerivacionAgendamiento(request, usuario.correo);



                return Ok(new OkResponse
                {
                    auth = true

                });


            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("obtenerHorasAgendadasPorPaciente")]
        [ProducesResponseType<ObtenerAgendamientoPacienteModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHorasAgendadasPorPaciente()
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                ObtenerAgendamientoPorPacienteModel request = new();
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                request.id_paciente = usuario.id_paciente.ToString();
                return Ok(await AgendamientoBo.ObtenerAgendamientoPorPaciente(request));

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("obtenerHistoricoHorasAgendadasPorPaciente")]
        [ProducesResponseType<ObtenerAgendamientoPacienteModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHistoricoHorasAgendadasPorPaciente()
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                var userSesion = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userSesion == null)
                {
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(403, response);
                }
                ObtenerAgendamientoPorPacienteModel request = new();
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                request.id_paciente = usuario.id_paciente.ToString();
                return Ok(await AgendamientoBo.ObtenerHistoricoAgendamientoPorPaciente(request));

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("obtenerExamenesConsultaPaciente")]
        [ProducesResponseType<List<ObtenerImagenExamenModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerExamenesConsultaPaciente(
            [FromBody] ObtenerImagenExamenConsultaModel request)
        {
            var response = new Dictionary<string, string>();
            var username = Username;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }

            PacienteModel paciente = await loginBo.ObtenerPaciente(username);
            if (paciente.rut == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }


            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerImagenesExamenesCargadas(request.id_agendamiento));
            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("guardarPuntuacionAtencionDoctor")]
        [ProducesResponseType<ObtenerAgendamientoPacienteModel>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarPuntuacionAtencionDoctor(CrearPuntuacionAtencionDoctorModel puntuacionRequest)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                ObtenerAgendamientoPorPacienteModel request = new();
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                request.id_paciente = usuario.id_paciente.ToString();
                return Ok(await AgendamientoBo.CrearPuntuacionAtencionDoctor(puntuacionRequest));

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("eliminarAgendamientoPaciente")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarAgendamientoPaciente(EliminarAgendamientoRequestModel request)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                EliminarAgendamientoPacienteModel requestPaciente = new();
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario.rut == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                requestPaciente.id_agendamiento = request.id_agendamiento;
                requestPaciente.rut = usuario.rut;
                requestPaciente.id_paciente = usuario.id_paciente.ToString();
                await AgendamientoBo.EliminarAgendamientoPaciente(requestPaciente);
                return Ok(new OkResponse
                {
                    auth = true

                });

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
        [HttpPost]
        [ActionName("confirmarAgendamientoPaciente")]
        [ProducesResponseType<OkResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarAgendamientoPaciente(EliminarAgendamientoRequestModel request)
        {
            LoginBo Login = new LoginBo(_config);
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            var response = new Dictionary<string, string>();
            PacienteModel usuario = new PacienteModel();

            try
            {
                EliminarAgendamientoPacienteModel requestPaciente = new();
                var username = Username;
                usuario = await Login.ObtenerPaciente(username!);
                if (usuario == null)
                {
                    response.Add("Error", "Invalid username");
                    return StatusCode(403, response);
                }
                ObtenerAgendamientoPorPacienteModel requestAgendamiento = new();
                requestAgendamiento.id_paciente = usuario.id_paciente.ToString();
                List<ObtenerAgendamientoPacienteModel> agendamientoList = await AgendamientoBo.ObtenerAgendamientoPorPaciente(requestAgendamiento!);
                if (agendamientoList == null || agendamientoList.FindAll(e => e.id_agendamiento.ToString() == request.id_agendamiento).Count == 0)
                {
                    response.Add("Error", "Invalid operation");
                    return StatusCode(403, response);
                }
                requestPaciente.id_agendamiento = request.id_agendamiento;
                requestPaciente.rut = usuario.rut;
                requestPaciente.id_paciente = usuario.id_paciente.ToString();
                await AgendamientoBo.ConfirmarAgendamientoPaciente(requestPaciente);
                return Ok(new OkResponse
                {
                    auth = true

                });

            }
            catch (Exception ex)
            {
                await utils.CreateLogFileAsync(ex.Message);
                response.Add("Error", "Hubo un problema al crear paciente.");
                return StatusCode(500, response);
            }
        }
    }

}
