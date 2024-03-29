﻿using AutoMapper;
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

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/provinces")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProvincesController : CustomBaseController<Province> // Notice <ProvinceDS> here
    {
        private readonly IProvinceRepository _provinceRepository; // Servicio que contiene la lógica principal de negocio para Provinces.
        private readonly ContextDB _dbContext;

        public ProvincesController(ILogger<ProvincesController> logger, IMapper mapper, IProvinceRepository workerRepository, ContextDB dbContext)
        : base(mapper, logger, workerRepository)
        {
            _response = new();
            _provinceRepository = workerRepository;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetProvince")]
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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] ProvinceDSCreateDTO workerCreateDTO)
        {
            workerCreateDTO.Name = Utils.ToCamelCase(workerCreateDTO.Name);
            return await Put<ProvinceDSCreateDTO, ProvinceDTO, Province>(id, workerCreateDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<ProvinceDSPatchDTO> patchDto)
        {
            return await Patch<Province, ProvinceDSPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateProvince")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] ProvinceDSCreateDTO provinceCreateDto)
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
                if (await _provinceRepository.Get(v => v.Name.ToLower() == provinceCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {provinceCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {provinceCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {provinceCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                var country = await _dbContext.Country.FindAsync(provinceCreateDto.CountryId);
                if (country == null)
                {
                    _logger.LogError($"El país ID={provinceCreateDto.CountryId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El país ID={provinceCreateDto.CountryId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El país ID={provinceCreateDto.CountryId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                provinceCreateDto.Name = Utils.ToCamelCase(provinceCreateDto.Name);
                Province modelo = _mapper.Map<Province>(provinceCreateDto);
                modelo.Country = country; // Asigna el objeto Country resuelto
                modelo.Creation = DateTime.Now;
                modelo.Update = DateTime.Now;

                await _provinceRepository.Create(modelo);
                _logger.LogInformation($"Se creó correctamente la propiedad Id:{modelo.Id}.");

                _response.Result = _mapper.Map<ProvinceDTO>(modelo);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetProvinceById
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

