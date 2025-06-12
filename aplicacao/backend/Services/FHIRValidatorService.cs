using FHIRUT.API.Models.CLI;
using FHIRUT.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;

namespace FHIRUT.API.Services
{
    public class FHIRValidatorService : IFHIRValidatorService
    {
        private readonly IFileSystem _fileSystem;
        private readonly ValidadorCliConfig _options;
        private readonly ILogger<FHIRValidatorService> _logger;

        public FHIRValidatorService(IFileSystem fileSystem, IOptions<ValidadorCliConfig> options, ILogger<FHIRValidatorService> logger)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> ValidateAsync(string instancePath, List<string>? profiles = null, List<string>? igs = null)
        {
            if (!_fileSystem.File.Exists(instancePath))
                throw new FileNotFoundException("Instance file not found", instancePath);

            var tempOutput = Path.GetTempFileName();
            var arguments = new List<string>
            {
                // NOVO: Adiciona um limite de memória maior para o Java, o que pode ajudar na performance.
                "-Xmx2g", 
                $"-jar \"{_options.ValidatorPath}\"",
                $"-version 4.0.1",
                $"-output \"{tempOutput}\"",
                $"\"{instancePath}\""
            };

            if (profiles?.Any() == true)
            {
                foreach (var profile in profiles)
                {
                    arguments.Add($"-profile \"{profile}\"");
                }
            }

            if (igs?.Any() == true)
            {
                foreach (var ig in igs)
                {
                    arguments.Add($"-ig \"{ig}\"");
                }
            }

            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = string.Join(" ", arguments),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8, // Garante a codificação correta
                StandardErrorEncoding = Encoding.UTF8
            };

            _logger.LogInformation("Executing validator with arguments: {Arguments}", psi.Arguments);

            using var process = Process.Start(psi);
            if (process == null)
            {
                _logger.LogError("Não foi possível iniciar o processo do validador.");
                throw new InvalidOperationException("Não foi possível iniciar o processo do validador.");
            }

            // CORREÇÃO: Lê a saída padrão e a saída de erro de forma assíncrona
            // para evitar deadlocks de buffer.
            var stdOutReader = process.StandardOutput.ReadToEndAsync();
            var stdErrReader = process.StandardError.ReadToEndAsync();

            // Espera o processo terminar E a leitura das saídas ser concluída.
            await process.WaitForExitAsync();
            await Task.WhenAll(stdOutReader, stdErrReader);

            var stdOut = await stdOutReader;
            var stdErr = await stdErrReader;

            if (!string.IsNullOrWhiteSpace(stdOut))
            {
                _logger.LogInformation("Validator Standard Output:\n{Output}", stdOut);
            }

            if (process.ExitCode != 0)
            {
                _logger.LogError("Validation failed for {Path}. Standard Error:\n{Error}", instancePath, stdErr);
            }

            var result = await _fileSystem.File.ReadAllTextAsync(tempOutput);
            _fileSystem.File.Delete(tempOutput);

            return result;
        }
    }
}