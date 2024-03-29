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
    [Route("api/rents")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RentsController : CustomBaseController<Rent> // Notice <Rent> here
    {
        private readonly IRentRepository _rentRepository;
        private readonly IEstateRepository _estateRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ILogService _logService;
        private readonly ContextDB _dbContext;
        private readonly IFileStorage _fileStorage;

        public RentsController(ILogger<RentsController> logger, IMapper mapper, IRentRepository rentRepository, IEstateRepository estateRepository, IFileStorage fileStorage, ILogService logService, ContextDB dbContext, IFileRepository fileRepository
        )
        : base(mapper, logger, rentRepository)
        {
            _response = new();
            _rentRepository = rentRepository;
            _estateRepository = estateRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _logService = logService;
            _dbContext = dbContext;
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
                var rent = await _rentRepository.Get(x => x.Id == id, tracked: true, includes: new List<IncludePropertyConfiguration<Rent>>
        {
            new IncludePropertyConfiguration<Rent>
            {
                IncludeExpression = j => j.ListFiles
            }
        });

                if (rent == null)
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
                var estateId = rent.EstateId;
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
                if (rent.ListFiles != null)
                {
                    foreach (var file in rent.ListFiles)
                    {
                        container = $"uploads/rents/estate{rent.EstateId}/{file.Creation.ToString("yyyy_MM")}/rent{rent.Id}";
                        await _fileStorage.DeleteFile(file.URL, container);
                        _dbContext.File.Remove(file);
                    }
                }

                if (container != null)
                {
                    await _fileStorage.DeleteFolder(container);
                }
                await _rentRepository.Remove(rent);
                _logger.LogInformation($"Se eliminó correctamente la renta Id:{id}.");
                await _logService.LogAction("Rent", "Delete", $"Id:{rent.Id}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Put(int id, [FromForm] RentCreateDTO rentCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(Messages.Generic.NotValid);
                    _response.ErrorMessages = new() { Messages.Generic.NotValid };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (id <= 0)
                {
                    _logger.LogError($"Datos de entrada inválidos.");
                    _response.ErrorMessages = new() { $"Datos de entrada inválidos." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var rent = await _rentRepository.Get(v => v.Id == id);
                if (rent == null)
                {
                    _logger.LogError($"Renta no encontrada ID = {id}.");
                    _response.ErrorMessages = new() { $"Renta no encontrada ID = {id}" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                var estate = await _dbContext.Estate.FindAsync(rentCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema.");
                    _response.ErrorMessages = new() { $"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema.");
                }

                rent.Warrant = Utils.ToCamelCase(str: rentCreateDto.Warrant);
                rent.Comments = Utils.ToCamelCase(str: rentCreateDto.Comments);
                rent.MonthlyValue = rentCreateDto.MonthlyValue;
                rent.Datetime_monthInit = rentCreateDto.Datetime_monthInit;
                rent.Duration = rentCreateDto.Duration;
                rent.RentIsEnded = rentCreateDto.RentIsEnded;
                rent.Update = DateTime.Now;

                // Procesamiento de inquilinos
                foreach (int tenantId in rentCreateDto.TenantIds)
                {
                    var tenant = await _dbContext.Tenant.FindAsync(tenantId);
                    if (tenant == null)
                    {
                        return NotFound($"El inquilino ID={tenantId} no existe en el sistema.");
                    }
                    rent.ListTenants.Add(tenant);
                }

                var updatedRent = await _rentRepository.Update(rent);

                // Procesamiento de fotos
                if (rentCreateDto.ListFiles != null && rentCreateDto.ListFiles.Count > 0)
                {
                    await ProcessRentFiles(updatedRent, rentCreateDto.ListFiles);
                }

                _logger.LogInformation($"Se actualizó correctamente el contrato Id:{rent.Id}.");
                await _logService.LogAction("Rent", "Update", $"Id:{rent.Id}, Nombre (propiedad): {estate.Name}.", User.Identity.Name);

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
        public async Task<ActionResult<APIResponse>> Patch(int id, [FromBody] JsonPatchDocument<RentPatchDTO> patchDto)
        {
            return await Patch<Rent, RentPatchDTO>(id, patchDto);
        }

        #endregion

        #region Endpoints específicos

        [HttpPost(Name = "CreateRent")]
        public async Task<ActionResult<APIResponse>> Post([FromForm] RentCreateDTO rentCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError(Messages.Generic.NotValid);
                    _response.ErrorMessages = new() { Messages.Generic.NotValid };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var estate = await _dbContext.Estate.FindAsync(rentCreateDto.EstateId);
                if (estate == null)
                {
                    _logger.LogError($"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema.");
                    _response.ErrorMessages = new() { $"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema." };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound($"La propiedad ID={rentCreateDto.EstateId} no existe en el sistema.");
                }

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {

                    rentCreateDto.Warrant = Utils.ToCamelCase(rentCreateDto.Warrant);
                    rentCreateDto.Comments = Utils.ToCamelCase(str: rentCreateDto.Comments);
                    Rent rent = _mapper.Map<Rent>(rentCreateDto);
                    rent.Estate = estate;
                    rent.Creation = DateTime.Now;
                    rent.Update = DateTime.Now;

                    // Procesamiento de inquilinos
                    foreach (int tenantId in rentCreateDto.TenantIds)
                    {
                        var tenant = await _dbContext.Tenant.FindAsync(tenantId);
                        if (tenant == null)
                        {
                            return NotFound($"El inquilino ID={tenantId} no existe en el sistema.");
                        }
                        rent.ListTenants.Add(tenant);
                    }

                    await _rentRepository.Create(rent);

                    // Actualizar la propiedad
                    estate.PresentRentId = rent.Id;
                    estate.EstateIsRented = true;
                    await _estateRepository.Update(estate);

                    // Procesamiento de fotos
                    if (rentCreateDto.ListFiles != null && rentCreateDto.ListFiles.Count > 0)
                    {
                        await ProcessRentFiles(rent, rentCreateDto.ListFiles);
                    }

                    await transaction.CommitAsync();

                    _logger.LogInformation($"Se creó correctamente el contrato Id:{rent.Id}.");
                    await _logService.LogAction("Rent", "Create", $"Id:{rent.Id}, Nombre (propiedad): {estate.Name}.", User.Identity.Name);

                    _response.Result = _mapper.Map<RentDTO>(rent);
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtAction(nameof(Get), new { id = rent.Id }, _response);
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