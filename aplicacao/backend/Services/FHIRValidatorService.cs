﻿using FHIRUT.API.Models.CLI;
using FHIRUT.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Abstractions;

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
                $"-jar {_options.ValidatorPath}",
                $"-version 4.0.1",
                $"-output {tempOutput}",
                instancePath
            };

            if (profiles?.Any() == true)
            {
                foreach (var profile in profiles)
                {
                    arguments.Add($"-profile {profile}");
                }
            }

            if (igs?.Any() == true)
            {
                foreach (var ig in igs)
                {
                    arguments.Add($"-ig {ig}");
                }
            }

            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = string.Join(" ", arguments),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogError("Validation failed for {Path}: {Error}", instancePath, error);
            }

            var result = await _fileSystem.File.ReadAllTextAsync(tempOutput);
            _fileSystem.File.Delete(tempOutput);

            return result;
        }
    }
}
