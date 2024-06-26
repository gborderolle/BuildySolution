﻿using BuildyBackend.Core.ApiBehavior;
using BuildyBackend.Core.Domain.Entities;
using BuildyBackend.Core.Domain.IdentityEntities;
using BuildyBackend.Core.Domain.RepositoryContracts;
using BuildyBackend.Core.Enums;
using BuildyBackend.Core.Filters;
using BuildyBackend.Core.Helpers;
using BuildyBackend.EmailService;
using BuildyBackend.Infrastructure.DbContext;
using BuildyBackend.Infrastructure.MessagesService;
using BuildyBackend.Infrastructure.Repositories;
using BuildyBackend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace BuildyBackend.UI.StartupExtensions;

/// <summary>
/// Extension method to configure services
/// s: https://www.udemy.com/course/asp-net-core-true-ultimate-guide-real-project/learn/lecture/34524780#overview
/// </summary>
public static class ConfigureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Swagger

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Buildy 3.0",
                Version = "v1",
                Description = "Con Buildy, los usuarios pueden realizar diversas tareas relacionadas con la administración de propiedades de manera centralizada.",
                TermsOfService = new Uri("https://buildy.uy/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Soporte",
                    Email = "hola@buildy.uy",
                    Url = new Uri("https://buildy.uy/support"),
                },
                License = new OpenApiLicense
                {
                    Name = "Uso bajo licencia 2024.",
                    Url = new Uri("https://buildy.uy"),
                }
            });
            // Include XML comments
            var xmlFile = "api.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        #endregion

        // Configuración de servicios
        services.AddControllers(options =>
        {
            // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816116#notes
            options.Filters.Add(typeof(ExceptionFilter));
            options.Filters.Add(typeof(BadRequestParse));
            options.Conventions.Add(new SwaggerGroupByVersion());
        })
        .ConfigureApiBehaviorOptions(BehaviorBadRequests.Parse)
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // para arreglar errores de loop de relaciones 1..n y viceversa
        });

        services.AddAutoMapper(typeof(AutoMapperProfiles));

        #region Registro de servicios

        // --------------

        // AddTransient: cambia dentro del contexto
        // AddScoped: se mantiene dentro del contexto (mejor para los servicios)
        // AddSingleton: no cambia nunca

        // Repositorios
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IEstateRepository, EstateRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IProvinceRepository, ProvinceDSRepository>();
        services.AddScoped<IRentRepository, RentRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();

        // Mensajes
        services.AddScoped<IMessage<City>, CityMessage>();
        services.AddScoped<IMessage<Country>, CountryMessage>();
        services.AddScoped<IMessage<Estate>, EstateMessage>();
        services.AddScoped<IMessage<Job>, JobMessage>();
        services.AddScoped<IMessage<Owner>, OwnerMessage>();
        services.AddScoped<IMessage<Province>, ProvinceMessage>();
        services.AddScoped<IMessage<Rent>, RentMessage>();
        services.AddScoped<IMessage<Report>, ReportMessage>();
        services.AddScoped<IMessage<Tenant>, TenantMessage>();
        services.AddScoped<IMessage<Worker>, WorkerMessage>();
        services.AddScoped<IMessage<BuildyUser>, BuildyUserMessage>();
        services.AddScoped<IMessage<BuildyRole>, BuildyRoleMessage>();

        // Otros servicios
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<ICountryResolver, CountryResolver>();
        services.AddScoped<CountryResolverService>();

        // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/27148834#notes
        services.AddTransient<GenerateLinks>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // Filtros
        //Ejemplo: services.AddScoped<MovieExistsAttribute>();

        // Servicios extra
        services.AddSignalR();

        // Manejo de archivos en el servidor 
        services.AddSingleton<IFileStorage, FileStorageLocal>();
        services.AddHttpContextAccessor();

        #endregion

        #region Email Configuration

        var emailConfig = configuration.GetSection("NotificationEmail").Get<EmailConfiguration>();
        services.AddSingleton(emailConfig);
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddDetection();

        #endregion

        #region AddDbContext and AddIdentity

        services.AddDbContext<ContextDB>(options =>
        {
            // Obtener el ConnectionString desde una variable de entorno
            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING_BUILDY");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Si no se encuentra el ConnectionString en las variables de entorno, intenta obtenerlo de appsettings
                connectionString = configuration.GetConnectionString("ConnectionString_Buildy"); // Localhost
                // Si aún es nulo o vacío, lanza una excepción
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("No se encontró el ConnectionString. Asegúrese de configurar la variable de entorno 'ConnectionString_Buildy' o definirla en appsettings.");
                }
            }
            options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging();
        });

        services.AddIdentity<BuildyUser, BuildyRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 6;
        })
            .AddEntityFrameworkStores<ContextDB>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<BuildyUser, BuildyRole, ContextDB, string>>()
            .AddRoleStore<RoleStore<BuildyRole, ContextDB, string>>();

        #endregion

        // --------------

        #region AddAuthentication

        // Intentar obtener la clave JWT desde una variable de entorno
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

        // Si la variable de entorno no está establecida, intenta obtenerla desde appsettings.json
        if (string.IsNullOrEmpty(jwtKey))
        {
            jwtKey = configuration["JWT:key"];
        }

        // Asegúrate de que la clave no sea nula o vacía
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("No se encontró la clave JWT. Asegúrese de configurar la variable de entorno 'JWT_KEY' o definirla en appsettings.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,   // false para desarrollo y pruebas
            ValidateAudience = false, // false para desarrollo y pruebas
                                      //ValidateIssuer = true,
                                      //ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        });
        #endregion

        #region AddAuthorization

        // Autorización basada en Claims
        // Agregar los roles del sistema
        // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/27047710#notes
        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdmin", policy => policy.RequireRole(UserTypeOptions.Admin.ToString()));
        });

        #endregion

        #region AddCors

        // Configuración CORS: para permitir recibir peticiones http desde un origen específico
        // CORS Sólo sirve para aplicaciones web (Angular, React, etc)
        // Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/27047732#notes
        // apirequest.io
        services.AddCors(options =>
        {
            var frontendURL = configuration.GetValue<string>("Frontend_URL");
            options.AddDefaultPolicy(builder =>
            {
                //builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader();
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                builder.WithExposedHeaders(new string[] { "totalSizeRecords" }); // Permite agregar headers customizados. Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/27148924#notes
            });
        });

        #endregion

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

        return services;
    }
}
