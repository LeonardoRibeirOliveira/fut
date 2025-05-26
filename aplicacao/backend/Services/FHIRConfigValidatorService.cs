using System.IO.Abstractions;
using FHIRUT.API.Models.CLI;

namespace FHIRUT.API.Services
{
    public class FHIRConfigValidatorService
    {
        private readonly ILogger<FHIRConfigValidatorService> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly ValidadorCliConfig _options;

        public FHIRConfigValidatorService(ILogger<FHIRConfigValidatorService> logger, IFileSystem fileSystem, ValidadorCliConfig options)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _options = options;
        }

        public async Task EnsureValidatorExists()
        {
            if (!_fileSystem.File.Exists(_options.ValidatorPath))
            {
                try
                {
                    _logger.LogInformation("Downloading FHIR validator...");
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(_options.ValidatorPath)!);

                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(_options.ValidatorDownloadUrl);
                    response.EnsureSuccessStatusCode();

                    await using var stream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = _fileSystem.File.Create(_options.ValidatorPath);
                    await stream.CopyToAsync(fileStream);

                    _logger.LogInformation("Validator downloaded to {Path}", _options.ValidatorPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to download FHIR validator.");
                    throw;
                }
            }
        }
    }
}
