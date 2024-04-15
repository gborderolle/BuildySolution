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
using BuildyBackend.Infrastructure.MessagesService;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/provinces")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProvincesController : CustomBaseController<Province>
    {
        private readonly IProvinceRepository _provinceRepository;
        private readonly ContextDB _dbContext;
        private readonly IMessage<Province> _message;

        public ProvincesController(ILogger<ProvincesController> logger, IMapper mapper, IProvinceRepository workerRepository, ContextDB dbContext, IMessage<Province> message)
        : base(mapper, logger, workerRepository)
        {
            _response = new();
            _provinceRepository = workerRepository;
            _dbContext = dbContext;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetProvince")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<Province>>
            {
                    new IncludePropertyConfiguration<Province>
                    {
                        IncludeExpression = b => b.Country
                    },
                };
            return await Get<Province, ProvinceDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var provinces = await _provinceRepository.GetAll();
            _response.Result = _mapper.Map<List<ProvinceDTO>>(provinces);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/Provinces/1
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            return await Get<Province, ProvinceDTO>();
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            return await Delete<Province>(id);
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] ProvinceDSCreateDTO dto)
        {
            dto.Name = Utils.ToCamelCase(dto.Name);
            return await Put<ProvinceDSCreateDTO, ProvinceDTO, Province>(id, dto);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<ProvinceDSPatchDTO> dto)
        {
            return await Patch<Province, ProvinceDSPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] ProvinceDSCreateDTO dto)
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
                if (await _provinceRepository.Get(v => v.Name.ToLower() == dto.Name.ToLower()) != null)
                {
                    _logger.LogError(_message.NameAlreadyExists(dto.Name));
                    _response.ErrorMessages = new() { _message.NameAlreadyExists(dto.Name) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NameAlreadyExists(dto.Name));
                    return BadRequest(ModelState);
                }

                var country = await _dbContext.Country.FindAsync(dto.CountryId);
                if (country == null)
                {
                    _logger.LogError($"El país ID={dto.CountryId} no existe en el sistema");
                    _response.ErrorMessages = new() { $"El país ID={dto.CountryId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El país ID={dto.CountryId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                Province model = _mapper.Map<Province>(dto);
                model.Country = country; // Asigna el objeto Country resuelto
                model.Creation = DateTime.Now;
                model.Update = DateTime.Now;

                await _provinceRepository.Create(model);
                _logger.LogInformation(_message.Created(model.Id, model.Name));

                _response.Result = _mapper.Map<ProvinceDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetProvinceById
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

