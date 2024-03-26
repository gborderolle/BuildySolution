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
using BuildyBackend.Infrastructure.Services;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/workers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkersController : CustomBaseController<Worker>
    {
        private readonly IWorkerRepository _workerRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;

        public WorkersController(ILogger<WorkersController> logger, IMapper mapper, IWorkerRepository workerRepository, ILogService logService, ContextDB dbContext)
        : base(mapper, logger, workerRepository)
        {
            _response = new();
            _workerRepository = workerRepository;
            _logService = logService;
            _dbContext = dbContext;
        }

        #region Endpoints genéricos

        [HttpGet("GetWorker")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Worker>>
            {
                 new IncludePropertyConfiguration<Worker>
                    {
                        IncludeExpression = b => b.Job
                    },
                };
            return await Get<Worker, WorkerDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var workers = await _workerRepository.GetAll();
            _response.Result = _mapper.Map<List<WorkerDTO>>(workers);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id}", Name = "GetWorkerById")] // url completa: https://localhost:7003/api/Estates/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Worker>>
            {
                 new IncludePropertyConfiguration<Worker>
                    {
                        IncludeExpression = b => b.Job
                    },
                };
            return await Get<Worker, WorkerDTO>(includes: includes);
        }

        [HttpDelete("{id:int}", Name = "DeleteWorker")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var worker = await _workerRepository.Get(x => x.Id == id, tracked: true);
                if (worker == null)
                {
                    _logger.LogError($"Trabajador no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Trabajador no encontrado ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Trabajador no encontrado ID = {id}.");
                }
                await _workerRepository.Remove(worker);

                _logger.LogInformation($"Se eliminó correctamente el trabajador Id:{id}.");
                await _logService.LogAction("Worker", "Delete", $"Id:{worker.Id}, Nombre: {worker.Name}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] WorkerCreateDTO workerCreateDto)
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
                var includes = new List<IncludePropertyConfiguration<Worker>>
                {
                 new IncludePropertyConfiguration<Worker>
                    {
                        IncludeExpression = b => b.Job
                    },
                };

                var worker = await _workerRepository.Get(v => v.Id == id, includes: includes);
                if (worker == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new List<string> { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                worker.Name = Utils.ToCamelCase(workerCreateDto.Name);
                worker.Comments = Utils.ToCamelCase(workerCreateDto.Comments);
                worker.Phone = workerCreateDto.Phone;
                worker.Email = workerCreateDto.Email;
                worker.IdentityDocument = workerCreateDto.IdentityDocument;
                worker.Comments = workerCreateDto.Comments;
                worker.JobId = workerCreateDto.JobId;
                worker.Job = await _dbContext.Job.FindAsync(workerCreateDto.JobId);
                worker.Update = DateTime.Now;

                var updatedWorker = await _workerRepository.Update(worker);

                _logger.LogInformation($"Se actualizó correctamente el trabajador Id:{id}.");
                await _logService.LogAction("Worker", "Update", $"Id:{worker.Id}, Nombre: {worker.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<WorkerDTO>(updatedWorker);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<WorkerPatchDTO> patchDto)
        {
            return await Patch<Worker, WorkerPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateWorker")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] WorkerCreateDTO workerCreateDto)
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
                if (await _workerRepository.Get(v => v.Name.ToLower() == workerCreateDto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {workerCreateDto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new List<string> { $"El nombre {workerCreateDto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", $"El nombre {workerCreateDto.Name} ya existe en el sistema.");
                    return BadRequest(ModelState);
                }

                workerCreateDto.Name = Utils.ToCamelCase(workerCreateDto.Name);
                workerCreateDto.Comments = Utils.ToCamelCase(workerCreateDto.Comments);
                Worker worker = _mapper.Map<Worker>(workerCreateDto);
                worker.Creation = DateTime.Now;
                worker.Update = DateTime.Now;

                await _workerRepository.Create(worker);

                _logger.LogInformation($"Se creó correctamente el trabajador Id:{worker.Id}.");
                await _logService.LogAction("Worker", "Create", $"Id:{worker.Id}, Nombre: {worker.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<WorkerDTO>(worker);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCountryDSById
                // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816172#notes
                return CreatedAtAction(nameof(Get), new { id = worker.Id }, _response);
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

