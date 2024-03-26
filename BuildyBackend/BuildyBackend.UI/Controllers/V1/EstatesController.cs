using AutoMapper;
using BuildyBackend.Core.DTO;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Core.Helpers;
using BuildyBackend.Services;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/estates")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EstatesController : CustomBaseController<Estate> // Notice <Estate> here
    {
        private readonly IEstateRepository _estateRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IRentRepository _rentRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;

        public EstatesController(ILogger<EstatesController> logger, IMapper mapper, IEstateRepository estateRepository, IReportRepository reportRepository, IJobRepository jobRepository, IRentRepository rentRepository, IWorkerRepository workerRepository, ITenantRepository tenantRepository, IPhotoRepository photoRepository, ILogService logService, ContextDB dbContext, IFileRepository fileRepository)
        : base(mapper, logger, estateRepository)
        {
            _response = new();
            _estateRepository = estateRepository;
            _reportRepository = reportRepository;
            _jobRepository = jobRepository;
            _rentRepository = rentRepository;
            _workerRepository = workerRepository;
            _tenantRepository = tenantRepository;
            _photoRepository = photoRepository;
            _fileRepository = fileRepository;
            _logService = logService;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetEstate")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Estate>>
            {
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.CityDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.OwnerDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListReports
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListJobs
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents
                    }
                };
            // 1..n..n
            var thenIncludes = new List<ThenIncludePropertyConfiguration<Estate>>
            {
                    // actores
                    new ThenIncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents,
                        ThenIncludeExpression = ab => ((Rent)ab).ListFiles
                    },
                    new ThenIncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents,
                        ThenIncludeExpression = ab => ((Rent)ab).ListTenants
                    },
            };
            return await Get<Estate, EstateDTO>(paginationDTO: paginationDTO, includes: includes, thenIncludes: thenIncludes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var estates = await _estateRepository.GetAll();
            _response.Result = _mapper.Map<List<EstateDTO>>(estates);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id}", Name = "GetEstateById")] // url completa: https://localhost:7003/api/Estates/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Estate>>
            {
                 new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.CityDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.OwnerDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListReports
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListJobs
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents
                    }
                };
            // 1..n..n
            var thenIncludes = new List<ThenIncludePropertyConfiguration<Estate>>
            {
                    // actores
                    new ThenIncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents,
                        ThenIncludeExpression = ab => ((Rent)ab).ListFiles
                    },
                    new ThenIncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents,
                        ThenIncludeExpression = ab => ((Rent)ab).ListTenants
                    },
            };
            return await Get<Estate, EstateDTO>(includes: includes, thenIncludes: thenIncludes);
        }

        [HttpDelete("{id:int}", Name = "DeleteEstate")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var estate = await _estateRepository.Get(
                        x => x.Id == id,
                        tracked: true,
                        includes: new List<IncludePropertyConfiguration<Estate>>
                        {
                    new IncludePropertyConfiguration<Estate> { IncludeExpression = j => j.ListRents },
                    new IncludePropertyConfiguration<Estate> { IncludeExpression = j => j.ListJobs },
                    new IncludePropertyConfiguration<Estate> { IncludeExpression = j => j.ListReports },
                        });

                    if (estate == null)
                    {
                        _logger.LogError($"Propiedad no encontrada ID = {id}.");
                        _response.ErrorMessages = new List<string> { $"Propiedad no encontrada ID = {id}." };
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        return NotFound($"Propiedad no encontrada ID = {id}.");
                    }

                    // Asumiendo que tienes un repositorio para los trabajadores (Worker)
                    foreach (var job in estate.ListJobs.ToList())
                    {
                        var workersRelatedToJob = await _workerRepository.FindWorkersByJobId(job.Id);
                        foreach (var worker in workersRelatedToJob)
                        {
                            // Actualiza o elimina los registros de Worker
                            worker.JobId = null; // o asignar a otro JobId según tu lógica de negocio
                            await _workerRepository.Update(worker);
                        }
                        var photosRelatedToJob = await _photoRepository.FindPhotosByJobId(job.Id);
                        foreach (var photo in photosRelatedToJob)
                        {
                            // Actualiza o elimina los registros de Worker
                            photo.JobId = null; // o asignar a otro JobId según tu lógica de negocio
                            await _photoRepository.Update(photo);
                        }

                        await _jobRepository.Remove(job);
                    }

                    foreach (var rent in estate.ListRents.ToList())
                    {
                        var tenantsRelatedToRent = await _tenantRepository.FindTenantsByRentId(rent.Id);
                        foreach (var tenant in tenantsRelatedToRent)
                        {
                            // Actualiza o elimina los registros de Worker
                            tenant.RentId = null; // o asignar a otro JobId según tu lógica de negocio
                            await _tenantRepository.Update(tenant);
                        }

                        var filesRelatedToRent = await _fileRepository.FindFilesByRentId(rent.Id);
                        foreach (var file in filesRelatedToRent)
                        {
                            file.RentId = null; // o asignar a otro JobId según tu lógica de negocio
                            await _fileRepository.Update(file);
                        }

                        await _rentRepository.Remove(rent);
                    }

                    // Asumiendo que tienes un repositorio para los trabajadores (Worker)
                    foreach (var report in estate.ListReports.ToList())
                    {
                        var photosRelatedToReport = await _photoRepository.FindPhotosByReportId(report.Id);
                        foreach (var photo in photosRelatedToReport)
                        {
                            // Actualiza o elimina los registros de Worker
                            photo.ReportId = null; // o asignar a otro JobId según tu lógica de negocio
                            await _photoRepository.Update(photo);
                        }

                        await _reportRepository.Remove(report);
                    }

                    await _estateRepository.Remove(estate);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Se eliminó correctamente la propiedad {estate.Name}.");
                    await _logService.LogAction("Estate", "Delete", $"Id:{estate.Id}, Nombre: {estate.Name}.", User.Identity.Name);

                    _response.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_response);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex.ToString());
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages = new List<string> { ex.ToString() };
                    return BadRequest(_response);
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] EstateCreateDTO estateCreateDto)
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
                var includes = new List<IncludePropertyConfiguration<Estate>>
                {
                 new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.CityDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.OwnerDS
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListReports
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListJobs
                    },
                    new IncludePropertyConfiguration<Estate>
                    {
                        IncludeExpression = b => b.ListRents
                    }
                };
                var estate = await _estateRepository.Get(v => v.Id == id, includes: includes);
                if (estate == null)
                {
                    _logger.LogError($"Propiedad no encontrada ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Propiedad no encontrada ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                estate.Name = Utils.ToCamelCase(estateCreateDto.Name);
                estate.Address = Utils.ToCamelCase(estateCreateDto.Address);
                estate.Comments = Utils.ToCamelCase(estateCreateDto.Comments);
                estate.LatLong = estateCreateDto.LatLong;
                estate.GoogleMapsURL = estateCreateDto.GoogleMapsURL;
                estate.CityDS = await _dbContext.CityDS.FindAsync(estateCreateDto.CityDSId);
                estate.OwnerDS = await _dbContext.OwnerDS.FindAsync(estateCreateDto.OwnerDSId);
                estate.Comments = estateCreateDto.Comments;
                estate.Update = DateTime.Now;

                var updatedEstate = await _estateRepository.Update(estate);

                _logger.LogInformation($"Se actualizó correctamente la propiedad {estate.Name}.");
                await _logService.LogAction("Estate", "Update", $"Id:{estate.Id}, Nombre: {estate.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<EstateDTO>(updatedEstate);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<EstatePatchDTO> patchDto)
        {
            return await Patch<Estate, EstatePatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateEstate")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] EstateCreateDTO estateCreateDto)
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
                if (await _estateRepository.Get(v => v.Name.ToLower() == estateCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {estateCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {estateCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {estateCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                var city = await _dbContext.CityDS.FindAsync(estateCreateDto.CityDSId);
                if (city == null)
                {
                    _logger.LogError($"La ciudad ID={estateCreateDto.CityDSId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"La ciudad ID={estateCreateDto.CityDSId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La ciudad ID={estateCreateDto.CityDSId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }
                var owner = await _dbContext.OwnerDS.FindAsync(estateCreateDto.OwnerDSId);
                if (owner == null)
                {
                    _logger.LogError($"La ciudad ID={estateCreateDto.OwnerDSId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El dueño ID={estateCreateDto.OwnerDSId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El dueño ID={estateCreateDto.OwnerDSId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                estateCreateDto.Name = Utils.ToCamelCase(estateCreateDto.Name);
                estateCreateDto.Address = Utils.ToCamelCase(estateCreateDto.Address);
                estateCreateDto.Comments = Utils.ToCamelCase(estateCreateDto.Comments);
                Estate estate = _mapper.Map<Estate>(estateCreateDto);
                estate.CityDS = city;
                estate.OwnerDS = owner;
                estate.Creation = DateTime.Now;
                estate.Update = DateTime.Now;

                await _estateRepository.Create(estate);
                _logger.LogInformation($"Se creó correctamente la propiedad {estate.Name}.");
                await _logService.LogAction("Estate", "Create", $"Id:{estate.Id}, Nombre: {estate.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<EstateDTO>(estate);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetEstateById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = estate.Id }, _response);
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


