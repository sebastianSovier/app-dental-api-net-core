using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelo;
using Negocio;
using System.Security.Claims;
using Utilidades;

namespace app_dental_api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ConsultaMedicaController : ControllerBase
    {
        private readonly IAntiforgery antiforgery;
        private readonly IConfiguration _config;
        private readonly UtilidadesApiss utils = new UtilidadesApiss();
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsultaMedicaController"/> class.
        /// </summary>
        /// <param name="antiforgery">antiforgery instance.</param>
        public ConsultaMedicaController(IAntiforgery antiforgery, IConfiguration config)
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
        [ActionName("guardarConsultaMedica")]
        [ProducesResponseType<List<HorasAgendadasDoctorModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarConsultaMedica(
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
                if (profesional == null)
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

    }

}
