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
    [Route("api/rents")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentsController : CustomBaseController<Rent>
    {
        private readonly IRentRepository _rentRepository;
        private readonly IEstateRepository _estateRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly IMessage<Rent> _message;

        public RentsController(ILogger<RentsController> logger, IMapper mapper, IRentRepository rentRepository, IEstateRepository estateRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext, IFileRepository fileRepository, IMessage<Rent> message)

        : base(mapper, logger, rentRepository)
        {
            _response = new();
            _rentRepository = rentRepository;
            _estateRepository = estateRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
            _message = message;
        }

        #region Endpoints genéricos

        [HttpGet("GetRent")]
        public async Task<ActionResult<APIResponse>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var includes = new List<IncludePropertyConfiguration<Rent>>
            {
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.Estate
                    },
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.ListFiles
                    },
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.ListTenants
                    },
            };
            return await Get<Rent, RentDTO>(paginationDTO: paginationDTO, includes: includes);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> All()
        {
            var rents = await _rentRepository.GetAll();
            _response.Result = _mapper.Map<List<RentDTO>>(rents);
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpGet("{id:int}")] // url completa: https://localhost:7003/api/Rents/1
        public async Task<ActionResult<APIResponse>> Get([FromRoute] int id)
        {
            // 1..n
            var includes = new List<IncludePropertyConfiguration<Rent>>
            {
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.Estate
                    },
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.ListFiles
                    },
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = b => b.ListTenants
                    },
            };
            return await Get<Rent, RentDTO>(includes: includes);
        }

        [HttpDelete("{id:int}", Name = "DeleteRent")]
        public async Task<ActionResult<APIResponse>> Delete([FromRoute] int id)
        {
            try
            {
                var model = await _rentRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Rent>>
                {
                    new IncludePropertyConfiguration<Rent>
                    {
                        IncludeExpression = j => j.ListFiles
                    }
                });

                if (model == null)
                {
                    _logger.LogError($"Renta no encontrada ID = {id}.");
                    _response.ErrorMessages = new() { $"Renta no encontrada ID = {id}." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Eliminar o actualizar los registros de Tenant relacionados
                var tenants = _dbContext.Tenant.Where(t => t.RentId == id).ToList();
                foreach (var tenant in tenants)
                {
                    tenant.RentId = null; // Para actualizar
                }
                _dbContext.SaveChanges();

                // Eliminar todas las rentas asociadas al Estate de la renta que se está eliminando
                var estateId = model.EstateId;
                var estate = await _dbContext.Estate.FindAsync(estateId);
                if (estate != null)
                {
                    estate.PresentRentId = null;
                    estate.EstateIsRented = false;
                    _dbContext.Estate.Update(estate);
                    _dbContext.SaveChanges();
                }

                // Continuar con la eliminación de Rent
                string container = null;
                if (model.ListFiles != null)
                {
                    foreach (var file in model.ListFiles)
                    {
                        container = $"uploads/rents/estate{model.EstateId}/{file.Creation.ToString("yyyy_MM")}/rent{model.Id}";
                        await _fileStorage.DeleteFile(file.URL, container);
                        _dbContext.File.Remove(file);
                    }
                }

                if (container != null)
                {
                    await _fileStorage.DeleteFolder(container);
                }
                await _rentRepository.Remove(model);
                _logger.LogInformation(_message.Deleted(model.Id, model.Estate?.Name));
                await _logService.LogAction("Rent", "Delete", $"Id:{model.Id}.", User.Identity.Name);

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
                return BadRequest(_response);
            }
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")] // Asegura que el método acepte multipart/form-data
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] RentCreateDTO dto)
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
                var model = await _rentRepository.Get(v => v.Id == id);
                if (model == null)
                {
                    _logger.LogError($"Renta no encontrada ID = {id}.");
                    _response.ErrorMessages = new() { $"Renta no encontrada ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                var estate = await _dbContext.Estate.FindAsync(dto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={dto.EstateId} no existe en el sistema.");
                    _response.ErrorMessages = new() { $"La propiedad ID={dto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"La propiedad ID={dto.EstateId} no existe en el sistema.");
                }

                model.Warrant = Utils.ToCamelCase(str: dto.Warrant);
                model.Comments = Utils.ToCamelCase(str: dto.Comments);
                model.MonthlyValue = dto.MonthlyValue;
                model.Datetime_monthInit = dto.Datetime_monthInit;
                model.Duration = dto.Duration;
                model.RentIsEnded = dto.RentIsEnded;
                model.Update = DateTime.Now;

                // Procesamiento de inquilinos
                foreach (int tenantId in dto.TenantIds)
                {
                    var tenant = await _dbContext.Tenant.FindAsync(tenantId);
                    if (tenant == null)
                    {
                        return NotFound($"El inquilino ID={tenantId} no existe en el sistema.");
                    }
                    model.ListTenants.Add(tenant);
                }

                var updatedRent = await _rentRepository.Update(model);

                // Procesamiento de fotos
                if (dto.ListFiles != null && dto.ListFiles.Count > 0)
                {
                    await ProcessRentFiles(updatedRent, dto.ListFiles);
                }

                _logger.LogInformation(_message.Updated(model.Id, estate.Name));
                await _logService.LogAction("Rent", "Update", $"Id:{model.Id}, Nombre (propiedad): {estate.Name}.", User.Identity.Name);

                _response.Result = _mapper.Map<RentDTO>(updatedRent);
                _response.StatusCode = HttpStatusCode.Created;

                return Ok(_response);
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

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<RentPatchDTO> dto)
        {
            return await Patch<Rent, RentPatchDTO>(id, dto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateRent")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] RentCreateDTO dto)
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
                    _logger.LogError($"La propiedad ID={dto.EstateId} no existe en el sistema.");
                    _response.ErrorMessages = new() { $"La propiedad ID={dto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"La propiedad ID={dto.EstateId} no existe en el sistema.");
                }

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {

                    dto.Warrant = Utils.ToCamelCase(dto.Warrant);
                    dto.Comments = Utils.ToCamelCase(str: dto.Comments);
                    Rent model = _mapper.Map<Rent>(dto);
                    model.Estate = estate;
                    model.Creation = DateTime.Now;
                    model.Update = DateTime.Now;

                    // Procesamiento de inquilinos
                    foreach (int tenantId in dto.TenantIds)
                    {
                        var tenant = await _dbContext.Tenant.FindAsync(tenantId);
                        if (tenant == null)
                        {
                            return NotFound($"El inquilino ID={tenantId} no existe en el sistema.");
                        }
                        model.ListTenants.Add(tenant);
                    }

                    await _rentRepository.Create(model);

                    // Actualizar la propiedad
                    estate.PresentRentId = model.Id;
                    estate.EstateIsRented = true;
                    await _estateRepository.Update(estate);

                    // Procesamiento de fotos
                    if (dto.ListFiles != null && dto.ListFiles.Count > 0)
                    {
                        await ProcessRentFiles(model, dto.ListFiles);
                    }

                    await transaction.CommitAsync();

                    _logger.LogInformation(_message.Created(model.Id, model.Estate?.Name));
                    await _logService.LogAction("Rent", "Create", $"Id:{model.Id}, Nombre (propiedad): {estate.Name}.", User.Identity.Name);

                    _response.Result = _mapper.Map<RentDTO>(model);
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

        private async Task ProcessRentFiles(Rent rent, IEnumerable<IFormFile> files)
        {
            string dynamicContainer = $"uploads/rents/estate{rent.EstateId}/{DateTime.Now:yyyy_MM}/rent{rent.Id}";
            foreach (var fileForm in files)
            {
                var extension = Path.GetExtension(fileForm.FileName).ToLower();
                using (var stream = new MemoryStream())
                {
                    await fileForm.CopyToAsync(stream);
                    var content = stream.ToArray();
                    string url = await _fileStorage.SaveFile(content, extension, dynamicContainer, fileForm.ContentType);

                    var file = new File1
                    {
                        URL = url,
                        Rent = rent,
                        Creation = DateTime.Now,
                        Update = DateTime.Now
                    };
                    await _fileRepository.Create(file);

                    rent.ListFiles.Add(file);
                    await _rentRepository.Update(rent);
                }
            }
            await _dbContext.SaveChangesAsync(); // Guarda los cambios en la base de datos al finalizar el procesamiento
        }

        #endregion

    }
}