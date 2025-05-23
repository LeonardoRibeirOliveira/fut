using System.IO.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using FHIRUT.API.Services;
using FHIRUT.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using FHIRUT.API.Models.CLI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do File Provider
builder.Services.AddSingleton<IFileSystem>(new FileSystem());

// Configurar caminho base
builder.Services.Configure<ValidadorCliConfig>(builder.Configuration.GetSection("Data"));

builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddSingleton<IFHIRValidatorService, FHIRValidatorService>();
builder.Services.AddSingleton<IFileBasedTestService, FileBasedTestService>();
builder.Services.AddSingleton<FHIRConfigValidatorService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<FHIRConfigValidatorService>>();
    var fileSystem = sp.GetRequiredService<IFileSystem>();
    var options = sp.GetRequiredService<IOptions<ValidadorCliConfig>>().Value;

    return new FHIRConfigValidatorService(logger, fileSystem, options);
});

// Configuração para uploads grandes
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50MB
});

// CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthorization();
app.MapControllers();

// Obter configurações
var dataOptions = app.Services.GetRequiredService<IOptions<ValidadorCliConfig>>().Value;

// Criar estrutura de pastas inicial
var fileSystem = app.Services.GetRequiredService<IFileSystem>();
fileSystem.Directory.CreateDirectory(Path.Combine(dataOptions.BasePath, "system"));

// Verificar e baixar o validador se necessário (Nova seção)
var validatorService = app.Services.GetRequiredService<FHIRConfigValidatorService>();
await validatorService.EnsureValidatorExists();

app.Run();