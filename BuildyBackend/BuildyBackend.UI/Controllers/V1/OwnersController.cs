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
using BuildyBackend.Infrastructure.MessagesService;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/owners")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OwnersController : CustomBaseController<Owner>
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMessage<Owner> _message;

        public OwnersController(ILogger<OwnersController> logger, IMapper mapper, IOwnerRepository ownerRepository, IMessage<Owner> message)
        : base(mapper, logger, ownerRepository)
        {
            _response = new();
            _ownerRepository = ownerRepository;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetOwner")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            return await Get<Owner, OwnerDTO>(paginationDTO: paginationDTO);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var owners = await _ownerRepository.GetAll();
            _response.Result = _mapper.Map<List<OwnerDTO>>(owners);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/ownersDS/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            return await Get<Owner, OwnerDTO>();
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            return await Delete<Owner>(id);
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] OwnerCreateDTO dto)
        {
            dto.Name = Utils.ToCamelCase(dto.Name);
            return await Put<OwnerCreateDTO, OwnerDTO, Owner>(id, dto);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<OwnerPatchDTO> dto)
        {
            return await Patch<Owner, OwnerPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateOwner")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] OwnerCreateDTO dto)
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
                if (await _ownerRepository.Get(v => v.Name.ToLower() == dto.Name.ToLower()) != null)
                {
                    _logger.LogError(_message.NameAlreadyExists(dto.Name));
                    _response.ErrorMessages = new() { _message.NameAlreadyExists(dto.Name) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NameAlreadyExists(dto.Name));
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                Owner modelo = _mapper.Map<Owner>(dto);
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _ownerRepository.Create(modelo);
                _logger.LogInformation(_message.Created(modelo.Id, modelo.Name));

                _response.Result = _mapper.Map<OwnerDTO>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetOwnerById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = modelo.Id }, _response);
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

