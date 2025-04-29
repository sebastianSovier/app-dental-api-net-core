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
    public class UtilitarioController : ControllerBase
    {
        private readonly IAntiforgery antiforgery;
        private readonly IConfiguration _config;
        private readonly UtilidadesApiss utils = new UtilidadesApiss();
        /// <summary>
        /// Initializes a new instance of the <see cref="UtilitarioController"/> class.
        /// </summary>
        /// <param name="antiforgery">antiforgery instance.</param>
        public UtilitarioController(IAntiforgery antiforgery, IConfiguration config)
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
        [ActionName("obtenerPreguntasIniciales")]
        [ProducesResponseType<List<PreguntasInicialesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> obtenerPreguntasIniciales(
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
                return Ok(await UtilitarioBo.ObtenerPreguntasIniciales());
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
        [ActionName("obtenerAlternativaPreguntasIniciales")]
        [ProducesResponseType<List<AlternativaPreguntasInicialesModel>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> obtenerAlternativaPreguntasIniciales(
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
                return Ok(await UtilitarioBo.ObtenerAlternativaPreguntasIniciales());
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
