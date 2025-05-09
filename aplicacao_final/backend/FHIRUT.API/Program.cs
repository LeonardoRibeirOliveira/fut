using System.IO.Abstractions;
using FHIRUT.API.Models;
using Microsoft.AspNetCore.Http.Features;
using FHIRUT.API.Services;
using FHIRUT.API.Services.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do File Provider
builder.Services.AddSingleton<IFileSystem>(new FileSystem());

// Configurar caminho base
builder.Services.Configure<DataOptions>(builder.Configuration.GetSection("Data"));

builder.Services.AddSingleton<IFHIRValidatorService, FHIRValidatorService>();
builder.Services.AddSingleton<IFileBasedTestService, FileBasedTestService>();

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
}

app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthorization();
app.MapControllers();

// Obter configurações
var dataOptions = app.Services.GetRequiredService<IOptions<DataOptions>>().Value;

// Criar estrutura de pastas inicial
var fileSystem = app.Services.GetRequiredService<IFileSystem>();
fileSystem.Directory.CreateDirectory(Path.Combine(dataOptions.BasePath, "system"));
fileSystem.Directory.CreateDirectory(Path.Combine(dataOptions.BasePath, "users"));

// Verificar e baixar o validador se necessário (Nova seção)
var validatorService = app.Services.GetRequiredService<IFHIRValidatorService>();
try
{
    await ((FHIRValidatorService)validatorService).EnsureValidatorExists();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao verificar validador: {ex.Message}");
}

app.Run();