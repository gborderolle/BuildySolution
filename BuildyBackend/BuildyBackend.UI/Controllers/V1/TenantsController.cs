using AutoMapper;
using BuildyBackend.Core.DTO;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BuildyBackend.Core.Helpers;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Infrastructure.Services;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/tenants")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TenantsController : CustomBaseController<Tenant> // Notice <Tenant> here
    {
        private readonly ITenantRepository _tenantRepository; // Servicio que contiene la lógica principal de negocio para Tenants.
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;

        public TenantsController(ILogger<TenantsController> logger, IMapper mapper, ITenantRepository tenantRepository, ILogService logService, ContextDB dbContext)
        : base(mapper, logger, tenantRepository)
        {
            _response = new();
            _tenantRepository = tenantRepository;
            _logService = logService;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetTenant")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<Tenant>>
            {
                 new IncludePropertyConfiguration<Tenant>
                    {
                        IncludeExpression = b => b.Rent
                    },
                };
            return await Get<Tenant, TenantDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var tenants = await _tenantRepository.GetAll();
            _response.Result = _mapper.Map<List<TenantDTO>>(tenants);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id}", Name = "GetTenantById")] // url completa: https://localhost:7003/api/Estates/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            var includes = new List<IncludePropertyConfiguration<Tenant>>
            {
                 new IncludePropertyConfiguration<Tenant>
                    {
                        IncludeExpression = b => b.Rent
                    },
                };
            return await Get<Tenant, TenantDTO>(includes: includes);
        }

        [HttpDelete("{id:int}", Name = "DeleteTenant")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var tenant = await _tenantRepository.Get(x => x.Id == id, tracked: true);
                if (tenant == null)
                {
                    _logger.LogError($"Inquilino no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Inquilino no encontrado ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Inquilino no encontrado ID = {id}.");
                }
                await _tenantRepository.Remove(tenant);

                _logger.LogInformation($"Se eliminó correctamente el inquilino Id:{id}.");
                await _logService.LogAction("Tenant", "Delete", $"Id:{tenant.Id}, Nombre: {tenant.Name}.", User.Identity.Name);

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] TenantCreateDTO tenantCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Ocurrió un error en el servidor.");
                    _response.ErrorMessages = new List<string> { $"Ocurrió un error en el servidor." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
                }
                if (id <= 0)
                {
                    _logger.LogError($"Datos de entrada inválidos.");
                    _response.ErrorMessages = new List<string> { $"Datos de entrada inválidos." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                // 1..n
                var includes = new List<IncludePropertyConfiguration<Tenant>>
                {
                 new IncludePropertyConfiguration<Tenant>
                    {
                        IncludeExpression = b => b.Rent
                    },
                };

                var tenant = await _tenantRepository.Get(filter: v => v.Id == id, includes: includes);
                if (tenant == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                tenant.Name = Utils.ToCamelCase(tenantCreateDto.Name);
                tenant.Comments = Utils.ToCamelCase(tenantCreateDto.Comments);
                tenant.Phone1 = tenantCreateDto.Phone1;
                tenant.Phone2 = tenantCreateDto.Phone2;
                tenant.Email = tenantCreateDto.Email;
                tenant.IdentityDocument = tenantCreateDto.IdentityDocument;
                tenant.Comments = tenantCreateDto.Comments;
                tenant.RentId = tenantCreateDto.RentId;
                tenant.Rent = await _dbContext.Rent.FindAsync(tenantCreateDto.RentId);
                tenant.Update = DateTime.Now;

                var updatedTenant = await _tenantRepository.Update(tenant);

                _logger.LogInformation($"Se actualizó correctamente el inquilino Id:{id}.");
                await _logService.LogAction("Tenant", "Update", $"Id:{tenant.Id}, Nombre: {tenant.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<EstateDTO>(updatedTenant);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<TenantPatchDTO> patchDto)
        {
            return await Patch<Tenant, TenantPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateTenant")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] TenantCreateDTO tenantCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Ocurrió un error en el servidor.");
                    _response.ErrorMessages = new List<string> { $"Ocurrió un error en el servidor." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
                }
                if (await _tenantRepository.Get(v => v.Name.ToLower() == tenantCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {tenantCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {tenantCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {tenantCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                tenantCreateDto.Name = Utils.ToCamelCase(tenantCreateDto.Name);
                tenantCreateDto.Comments = Utils.ToCamelCase(tenantCreateDto.Comments);
                Tenant tenant = _mapper.Map<Tenant>(tenantCreateDto);
                tenant.Creation = DateTime.Now;
                tenant.Update = DateTime.Now;

                await _tenantRepository.Create(tenant);

                _logger.LogInformation($"Se creó correctamente el inquilino Id:{tenant.Id}.");
                await _logService.LogAction("Tenant", "Create", $"Id:{tenant.Id}, Nombre: {tenant.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<TenantDTO>(tenant);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCountryDSById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = tenant.Id }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        #endregion

        #region Private methods

        #endregion

    }
}

