using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BuildyBackend.Core.Helpers.HATEOAS;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        #region Endpoints

        [HttpGet("GetRootv1")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> Get()
        {
            var isAdmin = await _authorizationService.AuthorizeAsync(User, "IsAdmin");
            var dataHateoas = new List<DataHATEOAS>();
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetRootv1", new { }), description: "self", method: "GET"));
            dataHateoas.Add(new DataHATEOAS(link: Url.Link("GetAuthorsv1", new { }), description: "Autores", method: "GET"));

            if (isAdmin.Succeeded)
            {
                dataHateoas.Add(new DataHATEOAS(link: Url.Link("CreateAuthorv1", new { }), description: "Crear autor", method: "POST"));
                dataHateoas.Add(new DataHATEOAS(link: Url.Link("CreateBookv1", new { }), description: "Crear libro", method: "POST"));
            }
            return dataHateoas;
        }

        #endregion

        #region Private methods

        #endregion

    }
}