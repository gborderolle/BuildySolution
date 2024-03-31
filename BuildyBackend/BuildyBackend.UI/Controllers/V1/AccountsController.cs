using AutoMapper;
using BuildyBackend.Core.DTO;
using BuildyBackend.EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Wangkanai.Detection.Services;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Helpers;
using BuildyBackend.Core.Domain.IdentityEntities;
using BuildyBackend.Infrastructure.Services;
using BuildyBackend.Core.Enums;
using BuildyBackend.Infrastructure.MessagesService;

namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/accounts")]
    // [Authorize(Roles = nameof(UserTypeOptions.Admin))]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IDetectionService _detectionService;
        private readonly UserManager<BuildyUser> _userManager;
        private readonly SignInManager<BuildyUser> _signInManager;
        private readonly RoleManager<BuildyRole> _roleManager;
        private readonly ILogService _logService;
        private readonly ContextDB _contextDB;
        private APIResponse _response;
        private readonly IWebHostEnvironment _environment;
        private readonly IMessage<BuildyUser> _messageUser;
        private readonly IMessage<BuildyRole> _messageRole;

        public AccountsController
        (
            ILogger<AccountsController> logger,
            IMapper mapper,
            IConfiguration configuration,
            IEmailSender emailSender,
            EmailConfiguration emailConfiguration,
            IDetectionService detectionService,
            UserManager<BuildyUser> userManager,
            SignInManager<BuildyUser> signInManager,
            RoleManager<BuildyRole> roleManager,
            ILogService logService,
            ContextDB dbContext,
            IWebHostEnvironment environment,
            IMessage<BuildyUser> messageUser,
            IMessage<BuildyRole> messageRole
        )
        {
            _response = new();
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _emailSender = emailSender;
            _emailConfiguration = emailConfiguration;
            _detectionService = detectionService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logService = logService;
            _contextDB = dbContext;
            _environment = environment;
            _messageUser = messageUser;
            _messageRole = messageRole;
        }

        #region Endpoints genéricos

        [HttpGet("GetUsers")]
        public async Task<ActionResult<APIResponse>> GetUsers([FromQuery] PaginationDTO paginationDTO)
        {
            try
            {
                var queryable = _contextDB.BuildyUser;
                await HttpContext.InsertParamPaginationHeader(queryable);
                var users = await queryable.OrderBy(x => x.UserName).DoPagination(paginationDTO).ToListAsync();
                _response.Result = users;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<APIResponse>> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                _response.Result = roles;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }


        [HttpGet("GetLogs")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> GetLogs([FromQuery] PaginationDTO paginationDTO)
        {
            try
            {
                var queryable = _contextDB.Log;
                await HttpContext.InsertParamPaginationHeader(queryable);
                var logs = await queryable.OrderByDescending(x => x.Creation).DoPagination(paginationDTO).ToListAsync();
                _response.Result = logs;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpGet("GetUserRole/{id}")]
        public async Task<ActionResult<APIResponse>> GetUserRole(string id)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);
                _response.Result = roles;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("makeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> MakeAdmin([FromBody] string usuarioId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(usuarioId);
                await _userManager.AddClaimAsync(user, new Claim("role", "admin"));
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("removeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> RemoveAdmin([FromBody] string usuarioId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(usuarioId);
                await _userManager.RemoveClaimAsync(user, new Claim("role", "admin"));
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("CreateUser")] //api/accounts/CreateUser
        public async Task<ActionResult<APIResponse>> CreateUser(BuilderUserCreateDTO dto)
        {
            try
            {
                var user = new BuildyUser
                {
                    UserName = dto.Username,
                    Name = dto.Name,
                    Email = dto.Email,
                };
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation(_messageUser.Created(0, user.UserName));
                    await _logService.LogAction("Account", "Create", $"Username: {dto.Username}, Rol: {dto.UserRoleId}.", dto.Username);

                    _response.StatusCode = HttpStatusCode.OK;

                    // Asignar el rol al usuario
                    if (!string.IsNullOrEmpty(dto.UserRoleId))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, dto.UserRoleName);
                        if (!roleResult.Succeeded)
                        {
                            _response.IsSuccess = false;
                            _response.StatusCode = HttpStatusCode.InternalServerError;
                            _logger.LogError($"Error al asignar rol al usuario.");
                        }
                    }

                    var userCredential = new BuilderUserLoginDTO
                    {
                        Username = user.UserName,
                        Password = dto.Password
                    };
                    _response.Result = await TokenSetup(userCredential);
                }
                else
                {
                    _logger.LogError($"Registración incorrecta.");
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> UpdateUser(string id, [FromBody] BuilderUserUpdateDTO dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Actualiza los campos del usuario
                user.UserName = dto.Username; // Si el email es también el nombre de usuario
                user.Email = dto.Email;
                user.Name = dto.Name;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages = updateResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(_response);
                }

                // Actualiza el rol si es necesario
                if (!string.IsNullOrEmpty(dto.UserRoleId))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, dto.UserRoleName);
                }

                _logger.LogInformation(_messageUser.Updated(0, dto.Username));
                await _logService.LogAction("Account", "Update", $"Username: {dto.Username}, Rol: {dto.UserRoleId}.", dto.Username);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<UserDTO>(user); // Mapea el usuario actualizado a un DTO si es necesario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> Login([FromBody] BuilderUserLoginDTO dto)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(dto.Username);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        await _logService.LogAction(((BuildyUserMessage)_messageUser).ActionLog(0, user.UserName), "Login", "Inicio de sesión.", user.UserName);
                        _logger.LogInformation(((BuildyUserMessage)_messageUser).LoginSuccess(user.Id, user.UserName));

                        _response.StatusCode = HttpStatusCode.OK;
                        _response.Result = new
                        {
                            Token = await TokenSetup(dto),
                            UserRoles = roles
                        };
                        await SendLoginNotification(dto);
                    }
                    else
                    {
                        _logger.LogError($"User not found.");
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest("User not found");
                    }
                }
                else
                {
                    await _logService.LogAction(((BuildyUserMessage)_messageUser).ActionLog(0, dto.Username), "Login", "Inicio de sesión fallido.", dto.Username);
                    _logger.LogError(((BuildyUserMessage)_messageUser).LoginFailed(dto.Username));
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Login incorrecto");  // respuesta genérica para no revelar información
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("CreateUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> CreateUserRole([FromBody] BuilderRoleCreateDTO model)
        {
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExist)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new() { "El rol ya existe." };
                    return BadRequest(_response);
                }

                var newRole = new BuildyRole
                {
                    Name = model.Name
                };

                var result = await _roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = newRole;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
                return StatusCode(500, _response);
            }
            return Ok(_response);
        }

        [HttpPut("UpdateUserRole/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserTypeOptions.Admin))]
        public async Task<ActionResult<APIResponse>> UpdateUserRole(string id, [FromBody] BuilderRoleUpdateDTO model)
        {
            try
            {
                var userRole = await _roleManager.FindByIdAsync(id);
                if (userRole == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                userRole.Name = model.Name;
                var updateResult = await _roleManager.UpdateAsync(userRole);
                if (!updateResult.Succeeded)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages = updateResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<UserDTO>(userRole); // Mapea el usuario actualizado a un DTO si es necesario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new() { ex.ToString() };
            }
            return Ok(_response);
        }

        #endregion

        #region Private methods

        private async Task<AuthenticationResponse> TokenSetup(BuilderUserLoginDTO userCredential)
        {
            var user = await _userManager.FindByNameAsync(userCredential.Username);
            if (user == null)
            {
                return null;
            }
            var claims = new List<Claim>()
            {
                new Claim("email", userCredential.Username),
                new Claim("username", userCredential.Username),
                new Claim(ClaimTypes.Name, userCredential.Username)
                //new Claim("username", userCredential.Username)
            };

            var claimsDB = await _userManager.GetClaimsAsync(user);
            if (claimsDB != null)
            {
                claims.AddRange(collection: claimsDB);
            }

            // Intentar obtener la clave JWT desde una variable de entorno
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

            // Si la variable de entorno no está establecida, intenta obtenerla desde appsettings.json
            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = _configuration["JWT:key"];
            }

            // Asegúrate de que la clave no sea nula o vacía
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("No se encontró la clave JWT. Asegúrese de configurar la variable de entorno 'JWT_KEY' o definirla en appsettings.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: credentials);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        #region Email

        private async Task SendLoginNotification(BuilderUserLoginDTO userCredential)
        {
            // Comprueba si el entorno es de producción
            if (!_environment.IsProduction())
            {
                return;
            }

            var user = await _userManager.FindByNameAsync(userCredential.Username);
            if (user == null)
            {
                _logger.LogError("Usuario no encontrado para la notificación de inicio de sesión.");
                return;
            }

            // Comprueba si el usuario tiene el rol de administrador
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Admin"))
            {
                return;
            }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            string? clientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
            string? clientIPCity = await GetIpInfo(clientIP);
            bool isMobile = _detectionService.Device.Type == Wangkanai.Detection.Models.Device.Mobile;
            await SendAsyncEmail(userCredential, clientIP, clientIPCity, isMobile);
        }

        private static async Task<string?> GetIpInfo(string? Ip_Api_Url)
        {
            string? returnString = string.Empty;
            if (!string.IsNullOrWhiteSpace(Ip_Api_Url) && Ip_Api_Url != "::1")
            {
                using (HttpClient httpClient = new())
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage httpResponse = await httpClient.GetAsync("http://ip-api.com/json/" + Ip_Api_Url);
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            var geolocationInfo = await httpResponse.Content.ReadFromJsonAsync<LocationDetails_IpApi>();
                            returnString = geolocationInfo?.city;
                        }
                    }
                    catch (Exception)
                    {
                        //ServiceLog.AddException("Excepcion. Obteniendo info de IP al login. ERROR: " + ex.Message, MethodBase.GetCurrentMethod()?.DeclaringType?.Name, MethodBase.GetCurrentMethod()?.Name, ex.Message);
                    }
                }
            }
            return returnString;
        }

        private async Task SendAsyncEmail(BuilderUserLoginDTO userCredential, string? clientIP, string? clientIPCity, bool isMobile)
        {
            string emailNotificationDestination = _emailConfiguration.To;
            string emailNotificationSubject = _emailConfiguration.Subject;

            string emailNotificationBody = GlobalServices.GetEmailNotificationBody(userCredential, clientIP, clientIPCity, isMobile);
            var message = new Message(new string[] { emailNotificationDestination }, emailNotificationSubject, emailNotificationBody);
            await _emailSender.SendEmailAsync(message);
        }

        private class LocationDetails_IpApi
        {
            public string? query { get; set; }
            public string? city { get; set; }
            public string? country { get; set; }
            public string? countryCode { get; set; }
            public string? isp { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public string? org { get; set; }
            public string? region { get; set; }
            public string? regionName { get; set; }
            public string? status { get; set; }
            public string? timezone { get; set; }
            public string? zip { get; set; }
        }

        #endregion Email

        #endregion

    }
}