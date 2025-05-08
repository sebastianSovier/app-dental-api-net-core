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
    public class ProfesionalController : ControllerBase
    {
        private readonly IAntiforgery antiforgery;
        private readonly IConfiguration _config;
        private readonly UtilidadesApiss utils = new UtilidadesApiss();
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfesionalController"/> class.
        /// </summary>
        /// <param name="antiforgery">antiforgery instance.</param>
        public ProfesionalController(IAntiforgery antiforgery, IConfiguration config)
        {
            this.antiforgery = antiforgery;
            _config = config;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("obtenerProfesionales")]
        [ProducesResponseType<List<ProfesionalesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> obtenerProfesionales(
            [FromBody] EmptyRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }


            UtilitarioBo UtilitarioBo = new UtilitarioBo(_config);

            try
            {
                return Ok(await UtilitarioBo.ObtenerProfesionales());
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerRecomendacionesProfesionales")]
        [ProducesResponseType<List<ProfesionalesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> obtenerRecomendacionesProfesionales(
            [FromBody] EmptyRequest request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar profesional.");
                return StatusCode(403, response);
            }


            UtilitarioBo UtilitarioBo = new UtilitarioBo(_config);

            try
            {
                return Ok(await UtilitarioBo.ObtenerRecomendacionesProfesionales(username));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
                response.Add("Error", "Hubo un problema al validar profesional.");
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
        [ActionName("obtenerProfesionalesPuntuacion")]
        [ProducesResponseType<List<ProfesionalesPuntuacionesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerProfesionalesPuntuacion(
            [FromBody] EmptyRequest request)
        {
            var response = new Dictionary<string, string>();

            UtilitarioBo UtilitarioBo = new UtilitarioBo(_config);

            try
            {
                return Ok(await UtilitarioBo.ObtenerProfesionalesPuntuacion());
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerHorasAgendadasPorProfesional")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHorasAgendadasPorProfesional(
            [FromBody] HorasAgendadasRequestModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                request.id_profesional = profesional.id_profesional.ToString();

            }
            request.tipoUsuario = perfil;

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerHorasAgendadasPorDoctor(request));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerHorasProximasAgendadasPorProfesional")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHorasProximasAgendadasPorProfesional(
            [FromBody] HorasAgendadasRequestModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                request.id_profesional = profesional.id_profesional.ToString();

            }
            request.tipoUsuario = perfil;

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerHorasProximasAgendadasPorDoctor(request));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerDiaSinDisponibilidadPorDoctor")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerDiaSinDisponibilidadPorDoctor(
            [FromBody] HorasAgendadasRequestModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar usuario.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                request.id_profesional = profesional.id_profesional.ToString();

            }
            request.tipoUsuario = perfil;

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerDiaSinDisponibilidadPorDoctor(request));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerHistoricoHorasAgendadasPorProfesional")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerHistoricoHorasAgendadasPorProfesional(
            [FromBody] HorasAgendadasRequestModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                request.id_profesional = profesional.id_profesional.ToString();

            }
            request.tipoUsuario = perfil;

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerHistoricoHorasAgendadasPorDoctor(request));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("modificarDisponibilidadPorProfesional")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ModificarDisponibilidadPorProfesional(
            [FromBody] ModificarAgendamientoProfesionalModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                request.id_profesional = profesional.id_profesional.ToString();

            }

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ModificarAgendamientoPorProfesional(request));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("obtenerExamenesConsulta")]
        [ProducesResponseType<List<ObtenerImagenExamenModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerExamenesConsulta(
            [FromBody] ObtenerImagenExamenConsultaModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }

            }
            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

            try
            {
                return Ok(await AgendamientoBo.ObtenerImagenesExamenesCargadas(request.id_agendamiento));
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
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
        [ActionName("guardarConsultaMedica")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarConsultaMedica(
            [FromBody] GuardarConsultaMedicaModel request)
        {
            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar usuario.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }
                AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);

                try
                {
                    return Ok(await AgendamientoBo.CrearConsultaMedica(request, profesional.rut + profesional.dv));
                }
                catch (Exception ex)
                {
                    utils.createlogFile(ex.Message);
                    response.Add("Error", "Hubo un problema al validar paciente.");
                    return StatusCode(500, response);
                }

            }
            return StatusCode(403, response);


        }
        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">LoginRequest.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [ActionName("cargarImagenExamen")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CargarImagenExamen(
            [FromQuery] int id_agendamiento, [FromForm] List<IFormFile> imagenes)
        {
            var files = Request.Form.Files;
            if (files == null || files.Count == 0)
                return BadRequest("No se recibieron archivos");

            var response = new Dictionary<string, string>();
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var perfil = User.FindFirst(ClaimTypes.Role)?.Value;
            LoginBo loginBo = new(_config);
            if (username == null)
            {
                response.Add("Error", "Hubo un problema al validar usuario.");
                return StatusCode(403, response);
            }
            if (perfil.Equals("Profesional"))
            {
                ProfesionalModel profesional = await loginBo.ObtenerProfesional(username);
                if (profesional.rut == null)
                {
                    response.Add("Error", "Hubo un problema al validar profesional.");
                    return StatusCode(403, response);
                }

            }

            AgendamientoBo AgendamientoBo = new AgendamientoBo(_config);
            List<CargarImagenExamenModel> listaCargaImagenes = new List<CargarImagenExamenModel>();
            try
            {
                foreach (var imagen in imagenes)
                {
                    using var ms = new MemoryStream();
                    await imagen.CopyToAsync(ms);


                    CargarImagenExamenModel imagenRequest = new();
                    imagenRequest.imagen = ms.ToArray();
                    imagenRequest.nombre_examen = imagen.FileName;
                    imagenRequest.id_agendamiento = id_agendamiento.ToString();
                    imagenRequest.mime_type = imagen.ContentType;
                    listaCargaImagenes.Add(imagenRequest);

                }
                await AgendamientoBo.CargarImagenesExamenes(listaCargaImagenes, id_agendamiento.ToString());
                return Ok(new LoginResponse
                {
                    auth = true

                });
            }
            catch (Exception ex)
            {
                utils.createlogFile(ex.Message);
                response.Add("Error", "Hubo un problema al validar paciente.");
                return StatusCode(500, response);
            }
        }
    }

}
