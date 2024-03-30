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
using BuildyBackend.Infrastructure.MessagesService;

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
        private readonly IMessage<Worker> _message;


        public WorkersController(ILogger<WorkersController> logger, IMapper mapper, IWorkerRepository workerRepository, ILogService logService, ContextDB dbContext, IMessage<Worker> message)
        : base(mapper, logger, workerRepository)
        {
            _response = new();
            _workerRepository = workerRepository;
            _logService = logService;
            _dbContext = dbContext;
            _message = message;
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
                var model = await _workerRepository.Get(x => x.Id == id, tracked: true);
                if (model == null)
                {
                    _logger.LogError($"Trabajador no encontrado ID = {id}.");
                    _response.ErrorMessages = new() { $"Trabajador no encontrado ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"Trabajador no encontrado ID = {id}.");
                }
                await _workerRepository.Remove(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Worker", "Delete", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromBody] WorkerCreateDTO dto)
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
                var includes = new List<IncludePropertyConfiguration<Worker>>
                {
                 new IncludePropertyConfiguration<Worker>
                    {
                        IncludeExpression = b => b.Job
                    },
                };

                var model = await _workerRepository.Get(v => v.Id == id, includes: includes);
                if (model == null)
                {
                    _logger.LogError($"Trabajo no encontrado ID = {id}.");
                    _response.ErrorMessages = new() { $"Trabajo no encontrado ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                model.Name = Utils.ToCamelCase(dto.Name);
                model.Comments = Utils.ToCamelCase(dto.Comments);
                model.Phone = dto.Phone;
                model.Email = dto.Email;
                model.IdentityDocument = dto.IdentityDocument;
                model.Comments = dto.Comments;
                model.JobId = dto.JobId;
                model.Job = await _dbContext.Job.FindAsync(dto.JobId);
                model.Update = DateTime.Now;

                var updatedWorker = await _workerRepository.Update(model);

                _logger.LogInformation(_message.Updated(model.Id, model.Name));
                await _logService.LogAction("Worker", "Update", $"Id:{model.Id}, Nombre: {model.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<WorkerDTO>(updatedWorker);
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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<WorkerPatchDTO> dto)
        {
            return await Patch<Worker, WorkerPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateWorker")]
        public async Task<ActionResult<APIResponse>> Post([FromBody] WorkerCreateDTO dto)
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
                if (await _workerRepository.Get(v => v.Name.ToLower() == dto.Name.ToLower()) != null)
                {
                    _logger.LogError($"El nombre {dto.Name} ya existe en el sistema");
                    _response.ErrorMessages = new() { $"El nombre {dto.Name} ya existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    ModelState.AddModelError("NameAlreadyExists", _message.NameAlreadyExists(dto.Name));
                    return BadRequest(ModelState);
                }

                dto.Name = Utils.ToCamelCase(dto.Name);
                dto.Comments = Utils.ToCamelCase(dto.Comments);
                Worker model = _mapper.Map<Worker>(dto);
                model.Creation = DateTime.Now;
                model.Update = DateTime.Now;

                await _workerRepository.Create(model);

                _logger.LogInformation(_message.Created(model.Id, model.Name));
                await _logService.LogAction("Worker", "Create", _message.ActionLog(model.Id, model.Name), User.Identity.Name);

                _response.Result = _mapper.Map<WorkerDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                // CreatedAtRoute -> Nombre de la ruta (del método): GetCountryById
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

