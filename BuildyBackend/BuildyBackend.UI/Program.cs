using BuildyBackend.Core.Helpers;
using BuildyBackend.Infrastructure.Services;
using BuildyBackend.UI.Middlewares;
using BuildyBackend.UI.StartupExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging - Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) // Leer la configuración
    .ReadFrom.Services(services); // Hacer disponibles los servicios de la app
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

var webHostEnvironment = app.Services.GetService<IWebHostEnvironment>();
var wwwrootPath = webHostEnvironment?.WebRootPath ?? "";

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Buildy 3.0");
});

app.UseHsts();
app.UseHttpsRedirection(); //n1
app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseCors(); //n2
app.UseRouting(); //n3
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();