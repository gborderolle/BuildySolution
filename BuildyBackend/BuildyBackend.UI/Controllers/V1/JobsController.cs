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
    [Route("api/jobs")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class JobsController : CustomBaseController<Job> // Notice <Job> here
    {
        private readonly IJobRepository _jobRepository; // Servicio que contiene la lógica principal de negocio para Jobs.
        private readonly IPhotoRepository _photoRepository; // Servicio que contiene la lógica principal de negocio para Reports.
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;

        public JobsController(ILogger<JobsController> logger, IMapper mapper, IJobRepository jobRepository, IPhotoRepository photoRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext)
        : base(mapper, logger, jobRepository)
        {
            _response = new();
            _jobRepository = jobRepository;
            _photoRepository = photoRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetJob")]
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

        [HttpDelete("{id:int}", Name = "DeleteJob")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var job = await _jobRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Job>>
        {
            new IncludePropertyConfiguration<Job>
            {
                IncludeExpression = j => j.ListPhotos
            }
        });

                if (job == null)
                {
                    _logger.LogError($"Obra no encontrada ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Obra no encontrada ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Obra no encontrada ID = {id}.");
                }
                string container = null;

                // Eliminar las fotos asociadas
                if (job.ListPhotos != null)
                {
                    foreach (var photo in job.ListPhotos)
                    {
                        container = $"uploads/jobs/estate{job.EstateId}/{photo.Creation.ToString("yyyy_MM")}/job{job.Id}";
                        await _fileStorage.DeleteFile(photo.URL, container);
                        _dbContext.Photo.Remove(photo); // Asegúrate de que el contexto de la base de datos sea correcto
                    }

                }
                if (container != null)
                {
                    // Eliminar la carpeta del contenedor una sola vez
                    await _fileStorage.DeleteFolder(container);
                }
                await _jobRepository.Remove(job);

                _logger.LogInformation($"Se eliminó correctamente la obra Id:{id}.");
                await _logService.LogAction("Job", "Delete", $"Id:{job.Id}, Nombre: {job.Name}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] JobCreateDTO jobCreateDto)
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

                var estate = await _dbContext.Estate.FindAsync(jobCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                var job = await _jobRepository.Get(v => v.Id == id, includes: new List<IncludePropertyConfiguration<Job>> {
                    new IncludePropertyConfiguration<Job> { IncludeExpression = b => b.ListWorkers },
                    new IncludePropertyConfiguration<Job> { IncludeExpression = b => b.ListPhotos }
                });

                if (job == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                job.Name = Utils.ToCamelCase(str: jobCreateDto.Name);
                job.Comments = Utils.ToCamelCase(str: jobCreateDto.Comments);
                job.Month = jobCreateDto.Month;
                job.LabourCost = jobCreateDto.LabourCost;
                job.Comments = jobCreateDto.Comments;
                job.EstateId = jobCreateDto.EstateId;
                job.Estate = await _dbContext.Estate.FindAsync(jobCreateDto.EstateId);
                job.Update = DateTime.Now;

                if (jobCreateDto.WorkerIds != null)
                {
                    // Primero, limpia la lista actual de trabajadores
                    job.ListWorkers.Clear();

                    // Luego, agrega los nuevos trabajadores basado en los IDs
                    foreach (var workerId in jobCreateDto.WorkerIds)
                    {
                        var worker = await _dbContext.Worker.FindAsync(workerId);
                        if (worker != null)
                        {
                            job.ListWorkers.Add(worker);
                        }
                        else
                        {
                            _logger.LogWarning($"Trabajador no encontrado ID = {workerId}.");
                        }
                    }
                }

                var updatedJob = await _jobRepository.Update(job);

                if (jobCreateDto.ListPhotos != null && jobCreateDto.ListPhotos.Count > 0)
                {
                    string dynamicContainer = $"uploads/jobs/estate{estate.Id}/{DateTime.Now:yyyy_MM}/job{job.Id}";
                    foreach (var photoForm in jobCreateDto.ListPhotos)
                    {
                        Photo newPhoto = new();
                        newPhoto.Job = job;

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

                _logger.LogInformation($"Se actualizó correctamente la obra Id:{id}.");
                await _logService.LogAction("Job", "Update", $"Id:{job.Id}, Nombre: {job.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<JobDTO>(updatedJob);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<JobDTO> patchDto)
        {
            return await Patch<Job, JobDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateJob")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] JobCreateDTO jobCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Ocurrió un error en el servidor.");
                    _response.ErrorMessages = new List<string> { "Ocurrió un error en el servidor." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ModelState);
                }
                var estate = await _dbContext.Estate.FindAsync(jobCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"La propiedad ID={jobCreateDto.EstateId} no existe en el sistema.");
                    return BadRequest(ModelState);
                }

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    jobCreateDto.Name = Utils.ToCamelCase(jobCreateDto.Name);
                    jobCreateDto.Comments = Utils.ToCamelCase(jobCreateDto.Comments);
                    Job job = _mapper.Map<Job>(jobCreateDto);
                    job.Estate = estate;
                    job.Creation = DateTime.Now;
                    job.Update = DateTime.Now;

                    foreach (var workerId in jobCreateDto.WorkerIds)
                    {
                        var worker = await _dbContext.Worker.FindAsync(workerId);
                        if (worker == null)
                        {
                            return NotFound($"El trabajador ID={workerId} no existe en el sistema.");
                        }
                        job.ListWorkers.Add(worker);
                    }

                    await _jobRepository.Create(job);

                    if (jobCreateDto.ListPhotos != null && jobCreateDto.ListPhotos.Count > 0)
                    {
                        string dynamicContainer = $"uploads/jobs/estate{estate.Id}/{DateTime.Now:yyyy_MM}/job{job.Id}";
                        foreach (var photoForm in jobCreateDto.ListPhotos)
                        {
                            Photo newPhoto = new();
                            newPhoto.Job = job;

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

                    _logger.LogInformation($"Se creó correctamente la obra Id:{job.Id}.");
                    await _logService.LogAction("Job", "Create", $"Id:{job.Id}, Nombre: {job.Name}.", User.Identity.Name);

                    _response.Result = _mapper.Map<JobDTO>(job);
                    _response.StatusCode = HttpStatusCode.Created;

                    return CreatedAtAction(nameof(Get), new { id = job.Id }, _response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return _response;
            }
        }

        #endregion

        #region Private methods

        #endregion

    }
}