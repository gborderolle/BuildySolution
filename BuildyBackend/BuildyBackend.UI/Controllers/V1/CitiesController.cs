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

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/cities")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CitiesController : CustomBaseController<City> // Notice <City> here
    {
        private readonly ICityRepository _cityRepository; // Servicio que contiene la lógica principal de negocio para Cities.
        private readonly ContextDB _dbContext;

        public CitiesController(ILogger<CitiesController> logger, IMapper mapper, ICityRepository workerRepository, ContextDB dbContext)
        : base(mapper, logger, workerRepository)
        {
            _response = new();


            _cityRepository = workerRepository;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetCity")]
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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] CityCreateDTO workerCreateDTO)
        {
            workerCreateDTO.Name = Utils.ToCamelCase(workerCreateDTO.Name);
            return await Put<CityCreateDTO, CityDTO, City>(id, workerCreateDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<CityPatchDTO> patchDto)
        {
            return await Patch<City, CityPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateCity")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] CityCreateDTO cityCreateDto)
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
                if (await _cityRepository.Get(v => v.Name.ToLower() == cityCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {cityCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {cityCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {cityCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                var province = await _dbContext.Province.FindAsync(cityCreateDto.ProvinceId);
                if (province == null)
                {
                    _logger.LogError($"El departamento ID={cityCreateDto.ProvinceId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El departamento ID={cityCreateDto.ProvinceId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El departamento ID={cityCreateDto.ProvinceId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                cityCreateDto.Name = Utils.ToCamelCase(cityCreateDto.Name);
                City modelo = _mapper.Map<City>(cityCreateDto);
                modelo.Province = province; // Asigna el objeto Country resuelto
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _cityRepository.Create(modelo);
                _logger.LogInformation($"Se creó correctamente la propiedad Id:{modelo.Id}.");

                _response.Result = _mapper.Map<CityDTO>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCityById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = modelo.Id }, _response);
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

