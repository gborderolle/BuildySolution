using AutoMapper;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Core.DTO;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Core.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BuildyBackend.Infrastructure.MessagesService;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/cities")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CitiesController : CustomBaseController<City>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMessage<City> _message;
        private readonly ContextDB _dbContext;

        public CitiesController(ILogger<CitiesController> logger, IMapper mapper, ICityRepository workerRepository, ContextDB dbContext, IMessage<City> message)
        : base(mapper, logger, workerRepository)
        {
            _response = new();
            _cityRepository = workerRepository;
            _dbContext = dbContext;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetCity")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<City>>
            {
                    new IncludePropertyConfiguration<City>
                    {
                        IncludeExpression = b => b.Province
                    },
                };
            return await Get<City, CityDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var workers = await _cityRepository.GetAll();
            _response.Result = _mapper.Map<List<CityDTO>>(workers);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/Cities/1
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // n..1
            var includes = new List<IncludePropertyConfiguration<City>>
            {
                    new IncludePropertyConfiguration<City>
                    {
                        IncludeExpression = b => b.Province
                    },
                };
            return await Get<City, CityDTO>(includes: includes);
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            return await Delete<City>(id);
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] CityCreateDTO dto)
        {
            dto.Name = Utils.ToCamelCase(dto.Name);
            return await Put<CityCreateDTO, CityDTO, City>(id, dto);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<CityPatchDTO> dto)
        {
            return await Patch<City, CityPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] CityCreateDTO dto)
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
                if (await _cityRepository.Get(v => v.Name.ToLower() == dto.Name.ToLower()) != null)
                {
                    _logger.LogError(_message.NameAlreadyExists(dto.Name));
                    _response.ErrorMessages = new() { _message.NameAlreadyExists(dto.Name) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NameAlreadyExists(dto.Name));
                    return BadRequest(ModelState);
                }

                var province = await _dbContext.Province.FindAsync(dto.ProvinceId);
                if (province == null)
                {
                    _logger.LogError(_message.NotFound(dto.ProvinceId));
                    _response.ErrorMessages = new() { _message.NotFound(dto.ProvinceId) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NotFound(dto.ProvinceId));
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                City model = _mapper.Map<City>(dto);
                model.Province = province; // Asigna el objeto Country resuelto
                model.Creation = DateTime.Now;
                model.Update = DateTime.Now;

                await _cityRepository.Create(model);
                _logger.LogInformation(_message.Created(model.Id, model.Name));

                _response.Result = _mapper.Map<CityDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCityById
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