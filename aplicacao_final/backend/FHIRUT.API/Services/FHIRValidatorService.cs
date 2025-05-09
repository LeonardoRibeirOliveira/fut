using FHIRUT.API.Models;
using FHIRUT.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Abstractions;

namespace FHIRUT.API.Services
{
    public class FHIRValidatorService : IFHIRValidatorService
    {
        private readonly IFileSystem _fileSystem;
        private readonly DataOptions _options;
        private readonly ILogger<FHIRValidatorService> _logger;

        public FHIRValidatorService(IFileSystem fileSystem, IOptions<DataOptions> options, ILogger<FHIRValidatorService> logger)
        {
            _fileSystem = fileSystem;
            _options = options.Value;
            _logger = logger;

            EnsureValidatorExists().Wait();
        }

        public async Task EnsureValidatorExists()
        {
            if (!_fileSystem.File.Exists(_options.ValidatorPath))
            {
                _logger.LogInformation("Downloading FHIR validator...");
                _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(_options.ValidatorPath)!);

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(_options.ValidatorDownloadUrl);
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = _fileSystem.File.Create(_options.ValidatorPath);
                await stream.CopyToAsync(fileStream);

                _logger.LogInformation("Validator downloaded to {Path}", _options.ValidatorPath);
            }
        }

        public async Task<string> ValidateAsync(string instancePath, List<string>? profiles = null, List<string>? igs = null)
        {
            if (!_fileSystem.File.Exists(instancePath))
                throw new FileNotFoundException("Instance file not found", instancePath);

            var tempOutput = Path.GetTempFileName();
            var arguments = new List<string>
            {
                $"-jar {_options.ValidatorPath}",
                $"-version 4.0.1",
                $"-output {tempOutput}",
                instancePath
            };

            if (profiles?.Any() == true)
                arguments.Add($"-profile {string.Join(",", profiles)}");

            if (igs?.Any() == true)
                arguments.Add($"-ig {string.Join(",", igs)}");

            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = string.Join(" ", arguments),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Validation failed: {error}");
            }

            var result = await _fileSystem.File.ReadAllTextAsync(tempOutput);
            _fileSystem.File.Delete(tempOutput);

            return result;
        }
    }
}
