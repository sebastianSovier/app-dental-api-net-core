using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace app_dental_api.Controllers
{
    [Authorize]
    public abstract class BaseController : ControllerBase
    {

        public override ActionResult ValidationProblem(
        ModelStateDictionary modelStateDictionary)
        {
            return BadRequest("Datos inválidos");
        }
        protected string Username =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Usuario no autenticado");

    }
}
