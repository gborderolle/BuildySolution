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

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/countries")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CountriesController : CustomBaseController<CountryDS> // Notice <CountryDS> here
    {
        private readonly ICountryDSRepository _countryRepository; // Servicio que contiene la lógica principal de negocio para CountriesDS.

        public CountriesController(ILogger<CountriesController> logger, IMapper mapper, ICountryDSRepository countryRepository)
        : base(mapper, logger, countryRepository)
        {
            _response = new();
            _countryRepository = countryRepository;
        }

        #region Endpoints genéricos

        [HttpGet("GetCountry")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            return await Get<CountryDS, CountryDSDTO>(paginationDTO: paginationDTO);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var countries = await _countryRepository.GetAll();
            _response.Result = _mapper.Map<List<CountryDSDTO>>(countries);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/CountriesDS/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            return await Get<CountryDS, CountryDSDTO>();
        }

        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            return await Delete<CountryDS>(id);
        }

        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] CountryDSCreateDTO countryCreateDTO)
        {
            countryCreateDTO.Name = Utils.ToCamelCase(countryCreateDTO.Name);
            return await Put<CountryDSCreateDTO, CountryDSDTO, CountryDS>(id, countryCreateDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<CountryDSPatchDTO> patchDto)
        {
            return await Patch<CountryDS, CountryDSPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateCountry")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] CountryDSCreateDTO countryCreateDto)
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
                if (await _countryRepository.Get(v => v.Name.ToLower() == countryCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {countryCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {countryCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {countryCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                countryCreateDto.Name = Utils.ToCamelCase(countryCreateDto.Name);
                CountryDS modelo = _mapper.Map<CountryDS>(countryCreateDto);
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _countryRepository.Create(modelo);
                _logger.LogInformation($"Se creó correctamente la propiedad Id:{modelo.Id}.");

                _response.Result = _mapper.Map<CountryDSDTO>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCountryDSById
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

