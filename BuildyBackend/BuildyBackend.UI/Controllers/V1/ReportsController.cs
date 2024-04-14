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
using BuildyBackend.Infrastructure.MessagesService;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/reports")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportsController : CustomBaseController<Report>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly IMessage<Report> _message;

        public ReportsController(ILogger<ReportsController> logger, IMapper mapper, IReportRepository reportRepository, IPhotoRepository photoRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext, IMessage<Report> message)
        : base(mapper, logger, reportRepository)
        {
            _response = new();
            _reportRepository = reportRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
            _message = message;
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var model = await _reportRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Report>>
        {
            new IncludePropertyConfiguration<Report>
            {
                IncludeExpression = j => j.ListPhotos
            }
        });

                if (model == null)
                {
                    _logger.LogError($"Reporte no encontrada ID = {id}.");
                    _response.ErrorMessages = new() { $"Reporte no encontrada ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Reporte no encontrada ID = {id}.");
                }

                // Eliminar las fotos asociadas
                if (model.ListPhotos != null)
                {
                    var container = $"uploads/reports/estate{model.EstateId}/{model.Creation.ToString("yyyy_MM")}/report{model.Id}";
                    foreach (var photo in model.ListPhotos)
                    {
                        await _fileStorage.DeleteFile(photo.URL, container);
                        _dbContext.Photo.Remove(photo);
                    }

                    // Eliminar la carpeta del contenedor
                    await _fileStorage.DeleteFolder(container);
                }
                await _reportRepository.Remove(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Report", "Delete", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

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
        [Consumes("multipart/form-data")] // Asegura que el método acepte multipart/form-data
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] ReportCreateDTO dto)
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
                var model = await _reportRepository.Get(v => v.Id == id, includes: includes);
                if (model == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new() { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                var estate = await _dbContext.Estate.FindAsync(dto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={dto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new() { $"La propiedad ID={dto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={dto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                model.Name = Utils.ToCamelCase(dto.Name);
                model.Comments = Utils.ToCamelCase(dto.Comments);
                model.Month = dto.Month;
                model.Comments = dto.Comments;
                model.EstateId = dto.EstateId;
                model.Estate = await _dbContext.Estate.FindAsync(dto.EstateId);
                model.Update = DateTime.Now;

                var updatedReporter = await _reportRepository.Update(model);

                if (dto.ListPhotos != null && dto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/reports/estate{estate.Id}/{DateTime.Now:yyyy_MM}/report{model.Id}";
                    foreach (var photoForm in dto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Report = model;

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

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Report", "Update", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<ReportDTO>(updatedReporter);
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
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromForm] ReportCreateDTO dto)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Endpoints específicos

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] ReportCreateDTO dto)
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
                var estate = await _dbContext.Estate.FindAsync(dto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={dto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new() { $"La propiedad ID={dto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={dto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                dto.Comments = Utils.ToCamelCase(dto.Comments);
                Report model = _mapper.Map<Report>(dto);
                model.Estate = estate;
                model.Creation = DateTime.Now;
                model.Update = DateTime.Now;

                await _reportRepository.Create(model);
                _logger.LogInformation(_message.Created(model.Id, model.Name));
                await _logService.LogAction("Report", "Create", _message.ActionLog(model.Id, model.Name), User.Identity.Name);

                if (dto.ListPhotos != null && dto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/reports/estate{estate.Id}/{DateTime.Now:yyyy_MM}/report{model.Id}";
                    foreach (var photoForm in dto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Report = model;

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

                _response.Result = _mapper.Map<ReportDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetEstateById
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