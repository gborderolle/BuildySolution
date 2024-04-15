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
    [Route("api/jobs")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class JobsController : CustomBaseController<Job>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly IMessage<Job> _message;

        public JobsController(ILogger<JobsController> logger, IMapper mapper, IJobRepository jobRepository, IPhotoRepository photoRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext, IMessage<Job> message)
        : base(mapper, logger, jobRepository)
        {
            _response = new();
            _jobRepository = jobRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetJob")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<Job>>
            {
                    new IncludePropertyConfiguration<Job>
                    {
                        IncludeExpression = b => b.Estate
                    },
                new IncludePropertyConfiguration<Job>
                    {
                        IncludeExpression = b => b.ListWorkers
                    },
                new IncludePropertyConfiguration<Job>
                    {
                        IncludeExpression = b => b.ListPhotos
                    },
                };
            return await Get<Job, JobDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var jobs = await _jobRepository.GetAll();
            _response.Result = _mapper.Map<List<JobDTO>>(jobs);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/Jobs/1
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Job>>
            {
                    new IncludePropertyConfiguration<Job>
                    {
                        IncludeExpression = b => b.ListWorkers
                    },
                new IncludePropertyConfiguration<Job>
                    {
                        IncludeExpression = b => b.ListPhotos
                    },
                };
            return await Get<Job, JobDTO>(includes: includes);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var model = await _jobRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Job>>
        {
            new IncludePropertyConfiguration<Job>
            {
                IncludeExpression = j => j.ListPhotos
            }
        });

                if (model == null)
                {
                    _logger.LogError($"Obra no encontrada ID = {id}.");
                    _response.ErrorMessages = new() { $"Obra no encontrada ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Obra no encontrada ID = {id}.");
                }
                string container = null;

                // Eliminar las fotos asociadas
                if (model.ListPhotos != null)
                {
                    foreach (var photo in model.ListPhotos)
                    {
                        container = $"uploads/jobs/estate{model.EstateId}/{photo.Creation.ToString("yyyy_MM")}/job{model.Id}";
                        await _fileStorage.DeleteFile(photo.URL, container);
                        _dbContext.Photo.Remove(photo); // Asegúrate de que el contexto de la base de datos sea correcto
                    }

                }
                if (container != null)
                {
                    // Eliminar la carpeta del contenedor una sola vez
                    await _fileStorage.DeleteFolder(container);
                }
                await _jobRepository.Remove(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Job", "Delete", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] JobCreateDTO dto)
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

                var model = await _jobRepository.Get(v => v.Id == id, includes: new List<IncludePropertyConfiguration<Job>> {
                    new IncludePropertyConfiguration<Job> { IncludeExpression = b => b.ListWorkers },
                    new IncludePropertyConfiguration<Job> { IncludeExpression = b => b.ListPhotos }
                });

                if (model == null)
                {
                    _logger.LogError(_message.NotFound(id));
                    _response.ErrorMessages = new() { _message.NotFound(id) };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                model.Name = Utils.ToCamelCase(str: dto.Name);
                model.Comments = Utils.ToCamelCase(str: dto.Comments);
                model.Month = dto.Month;
                model.LabourCost = dto.LabourCost;
                model.Comments = dto.Comments;
                model.EstateId = dto.EstateId;
                model.Estate = await _dbContext.Estate.FindAsync(dto.EstateId);
                model.Update = DateTime.Now;

                if (dto.WorkerIds != null)
                {
                    // Primero, limpia la lista actual de trabajadores
                    model.ListWorkers.Clear();

                    // Luego, agrega los nuevos trabajadores basado en los IDs
                    foreach (var workerId in dto.WorkerIds)
                    {
                        var worker = await _dbContext.Worker.FindAsync(workerId);
                        if (worker != null)
                        {
                            model.ListWorkers.Add(worker);
                        }
                        else
                        {
                            _logger.LogWarning($"Trabajador no encontrado ID = {workerId}.");
                        }
                    }
                }

                var updatedJob = await _jobRepository.Update(model);

                if (dto.ListPhotos != null && dto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/jobs/estate{estate.Id}/{DateTime.Now:yyyy_MM}/job{model.Id}";
                    foreach (var photoForm in dto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Job = model;

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
                await _logService.LogAction("Job", "Update", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<JobDTO>(updatedJob);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromForm] JsonPatchDocument<JobDTO> dto)
        {
            return await Patch<Job, JobDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] JobCreateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Ocurrió un error en el servidor.");
                    _response.ErrorMessages = new() { "Ocurrió un error en el servidor." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
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

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    dto.Name = Utils.ToCamelCase(dto.Name);
                    dto.Comments = Utils.ToCamelCase(dto.Comments);
                    Job model = _mapper.Map<Job>(dto);
                    model.Estate = estate;
                    model.Creation = DateTime.Now;
                    model.Update = DateTime.Now;

                    foreach (var workerId in dto.WorkerIds)
                    {
                        var worker = await _dbContext.Worker.FindAsync(workerId);
                        if (worker == null)
                        {
                            return NotFound($"El trabajador ID={workerId} no existe en el sistema.");
                        }
                        model.ListWorkers.Add(worker);
                    }

                    await _jobRepository.Create(model);

                    if (dto.ListPhotos != null && dto.ListPhotos.Count > 0)
                    {
                        string dynamicContainer = $"uploads/jobs/estate{estate.Id}/{DateTime.Now:yyyy_MM}/job{model.Id}";
                        foreach (var photoForm in dto.ListPhotos)
                        {
                            Photo newPhoto = new();
                            newPhoto.Job = model;

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

                    await transaction.CommitAsync();

                    _logger.LogInformation(_message.Created(model.Id, model.Name));
                    await _logService.LogAction("Job", "Create", _message.ActionLog(model.Id, model.Name), User.Identity.Name);

                    _response.Result = _mapper.Map<JobDTO>(model);
                    _response.StatusCode = HttpStatusCode.Created;

                    return CreatedAtAction(nameof(Get), new { id = model.Id }, _response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
                return _response;
            }
        }

        #endregion

        #region Private methods

        #endregion

    }
}