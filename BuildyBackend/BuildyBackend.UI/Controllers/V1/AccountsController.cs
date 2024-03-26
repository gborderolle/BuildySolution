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
using System.Security.Cryptography;
using BuildyBackend.Core.Domain.IdentityEntities;
using BuildyBackend.Infrastructure.Services;


namespace BuildyBackend.UI.Controllers.V1
{
    [ApiController]
    [HasHeader("x-version", "1")]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountsController> _logger; // Logger para registrar eventos. 
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
            IWebHostEnvironment environment
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }


        [HttpGet("GetLogs")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("makeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("removeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("register")] //api/accounts/register
        public async Task<ActionResult<APIResponse>> Register(BuilderUserCreateDTO builderUserCreateDTO)
        {
            try
            {
                var user = new BuildyUser
                {
                    UserName = builderUserCreateDTO.Username,
                    Name = builderUserCreateDTO.Name,
                    Email = builderUserCreateDTO.Email,
                };
                var result = await _userManager.CreateAsync(user, builderUserCreateDTO.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Registración correcta.");
                    await _logService.LogAction("Account", "Create", $"Username: {builderUserCreateDTO.Username}, Rol: {builderUserCreateDTO.UserRoleId}.", builderUserCreateDTO.Username);

                    _response.StatusCode = HttpStatusCode.OK;

                    // Asignar el rol al usuario
                    if (!string.IsNullOrEmpty(builderUserCreateDTO.UserRoleId))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, builderUserCreateDTO.UserRoleName);
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
                        Password = builderUserCreateDTO.Password
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> UpdateUser(string id, [FromBody] BuilderUserUpdateDTO builderUserUpdateDTO)
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
                user.UserName = builderUserUpdateDTO.Username; // Si el email es también el nombre de usuario
                user.Email = builderUserUpdateDTO.Email;
                user.Name = builderUserUpdateDTO.Name;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages = updateResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(_response);
                }

                // Actualiza el rol si es necesario
                if (!string.IsNullOrEmpty(builderUserUpdateDTO.UserRoleId))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, builderUserUpdateDTO.UserRoleName);
                }

                _logger.LogInformation("Usuario actualizado.");
                await _logService.LogAction("Account", "Update", $"Username: {builderUserUpdateDTO.Username}, Rol: {builderUserUpdateDTO.UserRoleId}.", builderUserUpdateDTO.Username);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<UserDTO>(user); // Mapea el usuario actualizado a un DTO si es necesario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] BuilderUserLoginDTO builderUserLoginDTO)
        {
            try
            {
                // lockoutOnFailure: bloquea al usuario si tiene muchos intentos de logueo
                var result = await _signInManager.PasswordSignInAsync(builderUserLoginDTO.Username, builderUserLoginDTO.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Login correcto.");
                    var user = await _userManager.FindByNameAsync(builderUserLoginDTO.Username);
                    var roles = await _userManager.GetRolesAsync(user); // Obtener roles del usuario

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = new
                    {
                        Token = await TokenSetup(builderUserLoginDTO),
                        UserRoles = roles // Añade los roles del usuario aquí
                    };
                    await SendLoginNotification(builderUserLoginDTO);
                }
                else
                {
                    _logger.LogError($"Login incorrecto.");
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return Ok(_response);
        }


        [HttpPost("CreateUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult<APIResponse>> CreateUserRole([FromBody] BuilderRoleCreateDTO model)
        {
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExist)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "El rol ya existe." };
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
            return Ok(_response);
        }

        [HttpPut("UpdateUserRole/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
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
                _response.ErrorMessages = new List<string> { ex.ToString() };
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
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
            // string emailNotificationDestination = _configuration["NotificationEmail:To"];
            // string emailNotificationSubject = _configuration["NotificationEmail:Subject"];
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