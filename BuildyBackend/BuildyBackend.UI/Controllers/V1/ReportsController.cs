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
using BuildyBackend.Infrastructure.Services;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/reports")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportsController : CustomBaseController<Report> // Notice <Report> here
    {
        private readonly IReportRepository _reportRepository; // Servicio que contiene la lógica principal de negocio para Reports.
        private readonly IPhotoRepository _photoRepository; // Servicio que contiene la lógica principal de negocio para Reports.
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;

        public ReportsController(ILogger<ReportsController> logger, IMapper mapper, IReportRepository reportRepository, IPhotoRepository photoRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext)
        : base(mapper, logger, reportRepository)
        {
            _response = new();
            _reportRepository = reportRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetReport")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<Report>>
            {
                    new IncludePropertyConfiguration<Report>
                    {
                        IncludeExpression = b => b.Estate
                    },
                    new IncludePropertyConfiguration<Report>
                    {
                        IncludeExpression = b => b.ListPhotos
                    },
            };
            return await Get<Report, ReportDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var reports = await _reportRepository.GetAll();
            _response.Result = _mapper.Map<List<ReportDTO>>(reports);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/Reports/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Report>>
            {
                    new IncludePropertyConfiguration<Report>
                    {
                        IncludeExpression = b => b.Estate
                    },
                    new IncludePropertyConfiguration<Report>
                    {
                        IncludeExpression = b => b.ListPhotos
                    },
            };
            return await Get<Report, ReportDTO>(includes: includes);
        }

        [HttpDelete("{id:int}", Name = "DeleteReport")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var report = await _reportRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Report>>
        {
            new IncludePropertyConfiguration<Report>
            {
                IncludeExpression = j => j.ListPhotos
            }
        });

                if (report == null)
                {
                    _logger.LogError($"Reporte no encontrada ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Reporte no encontrada ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Reporte no encontrada ID = {id}.");
                }

                // Eliminar las fotos asociadas
                if (report.ListPhotos != null)
                {
                    var container = $"uploads/reports/estate{report.EstateId}/{report.Creation.ToString("yyyy_MM")}/report{report.Id}";
                    foreach (var photo in report.ListPhotos)
                    {
                        await _fileStorage.DeleteFile(photo.URL, container);
                        _dbContext.Photo.Remove(photo);
                    }

                    // Eliminar la carpeta del contenedor
                    await _fileStorage.DeleteFolder(container);
                }
                await _reportRepository.Remove(report);

                _logger.LogInformation($"Se eliminó correctamente el reporte Id:{id}.");
                await _logService.LogAction("Report", "Delete", $"Id:{report.Id}, Nombre: {report.Name}.", User.Identity.Name);

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
        [Consumes("multipart/form-data")] // Asegura que el método acepte multipart/form-data
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] ReportCreateDTO reportCreateDto)
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
                var includes = new List<IncludePropertyConfiguration<Report>>
                {
                        new IncludePropertyConfiguration<Report>
                        {
                            IncludeExpression = b => b.Estate
                        },
                        new IncludePropertyConfiguration<Report>
                        {
                            IncludeExpression = b => b.ListPhotos
                        },
                };
                var report = await _reportRepository.Get(v => v.Id == id, includes: includes);
                if (report == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                var estate = await _dbContext.Estate.FindAsync(reportCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                report.Name = Utils.ToCamelCase(reportCreateDto.Name);
                report.Comments = Utils.ToCamelCase(reportCreateDto.Comments);
                report.Month = reportCreateDto.Month;
                report.Comments = reportCreateDto.Comments;
                report.EstateId = reportCreateDto.EstateId;
                report.Estate = await _dbContext.Estate.FindAsync(reportCreateDto.EstateId);
                report.Update = DateTime.Now;

                var updatedReporter = await _reportRepository.Update(report);

                if (reportCreateDto.ListPhotos != null && reportCreateDto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/reports/estate{estate.Id}/{DateTime.Now:yyyy_MM}/report{report.Id}";
                    foreach (var photoForm in reportCreateDto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Report = report;

                        using (var stream = new MemoryStream())
                        {
                            await photoForm.CopyToAsync(stream);
                            var content = stream.ToArray();
                            var extension = Path.GetExtension(photoForm.FileName);
                            newPhoto.URL = await _fileStorage.SaveFile(content, extension, dynamicContainer, photoForm.ContentType);
                        }
                        await _photoRepository.Create(newPhoto);
                    }
                }

                _logger.LogInformation($"Se actualizó correctamente el reporte Id:{id}.");
                await _logService.LogAction("Report", "Update", $"Id:{report.Id}, Nombre: {report.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<ReportDTO>(updatedReporter);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<ReportPatchDTO> patchDto)
        {
            return await Patch<Report, ReportPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateReport")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] ReportCreateDTO reportCreateDto)
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
                var estate = await _dbContext.Estate.FindAsync(reportCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={reportCreateDto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                reportCreateDto.Name = Utils.ToCamelCase(reportCreateDto.Name);
                reportCreateDto.Comments = Utils.ToCamelCase(reportCreateDto.Comments);
                Report report = _mapper.Map<Report>(reportCreateDto);
                report.Estate = estate;
                report.Creation = DateTime.Now;
                report.Update = DateTime.Now;

                await _reportRepository.Create(report);
                _logger.LogInformation($"Se creó correctamente el reporte Id:{report.Id}.");
                await _logService.LogAction("Report", "Create", $"Id:{report.Id}, Nombre: {report.Name}.", User.Identity.Name);

                if (reportCreateDto.ListPhotos != null && reportCreateDto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/reports/estate{estate.Id}/{DateTime.Now:yyyy_MM}/report{report.Id}";
                    foreach (var photoForm in reportCreateDto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Report = report;

                        using (var stream = new MemoryStream())
                        {
                            await photoForm.CopyToAsync(stream);
                            var content = stream.ToArray();
                            var extension = Path.GetExtension(photoForm.FileName);
                            newPhoto.URL = await _fileStorage.SaveFile(content, extension, dynamicContainer, photoForm.ContentType);
                        }
                        await _photoRepository.Create(newPhoto);
                    }
                }

                _response.Result = _mapper.Map<ReportDTO>(report);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetEstateById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = report.Id }, _response);
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