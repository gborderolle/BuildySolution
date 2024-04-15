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
using BuildyBackend.Infrastructure.MessagesService;

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
        private readonly IMessage<Tenant> _message;

        public TenantsController(ILogger<TenantsController> logger, IMapper mapper, ITenantRepository tenantRepository, ILogService logService, ContextDB dbContext, IMessage<Tenant> message)
        : base(mapper, logger, tenantRepository)
        {
            _response = new();
            _tenantRepository = tenantRepository;
            _logService = logService;
            _dbContext = dbContext;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetTenant")]
        [ResponseCache(Duration = 60)]
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
        [ResponseCache(Duration = 60)]
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var model = await _tenantRepository.Get(x => x.Id == id, tracked: true);
                if (model == null)
                {
                    _logger.LogError($"Inquilino no encontrado ID = {id}.");
                    _response.ErrorMessages = new() { $"Inquilino no encontrado ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Inquilino no encontrado ID = {id}.");
                }
                await _tenantRepository.Remove(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Tenant", "Delete", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] TenantCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(_message.NotValid());
                    _response.ErrorMessages = new() { _message.NotValid() };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (id <= 0)
                {
                    _logger.LogError(_message.NotValid());
                    _response.ErrorMessages = new() { _message.NotValid() };
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

                var model = await _tenantRepository.Get(filter: v => v.Id == id, includes: includes);
                if (model == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new() { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                model.Name = Utils.ToCamelCase(dto.Name);
                model.Comments = Utils.ToCamelCase(dto.Comments);
                model.Phone1 = dto.Phone1;
                model.Phone2 = dto.Phone2;
                model.Email = dto.Email;
                model.IdentityDocument = dto.IdentityDocument;
                model.Comments = dto.Comments;
                model.RentId = dto.RentId;
                model.Rent = await _dbContext.Rent.FindAsync(dto.RentId);
                model.Update = DateTime.Now;

                var updatedTenant = await _tenantRepository.Update(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Tenant", "Update", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<EstateDTO>(updatedTenant);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<TenantPatchDTO> dto)
        {
            return await Patch<Tenant, TenantPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost]
        public async Task<ActionResult<APIResponse>> Post([FromBody] TenantCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(_message.NotValid());
                    _response.ErrorMessages = new() { _message.NotValid() };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (await _tenantRepository.Get(v => v.Name.ToLower() == dto.Name.ToLower()) != null)
                {
                    _logger.LogError(_message.NameAlreadyExists(dto.Name));
                    _response.ErrorMessages = new() { _message.NameAlreadyExists(dto.Name) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NameAlreadyExists(dto.Name));
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                dto.Comments = Utils.ToCamelCase(dto.Comments);
                Tenant model = _mapper.Map<Tenant>(dto);
                model.Creation = DateTime.Now;
                model.Update = DateTime.Now;

                await _tenantRepository.Create(model);

                _logger.LogInformation(_message.Created(model.Id, model.Name));
                await _logService.LogAction("Tenant", "Create", _message.ActionLog(model.Id, model.Name), User.Identity.Name);

                _response.Result = _mapper.Map<TenantDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCountryById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return _response;
        }

        #endregion

        #region Private methods

        #endregion

    }
}

