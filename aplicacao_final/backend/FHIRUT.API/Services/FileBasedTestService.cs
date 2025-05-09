using System.IO.Abstractions;
using FHIRUT.API.Models;
using FHIRUT.API.Services.Interfaces;

namespace FHIRUT.API.Services
{
    public class FileBasedTestService : IFileBasedTestService
    {
        private readonly IFHIRValidatorService _validatorService;
        private readonly IFileSystem _fileSystem;
        private readonly string _baseDataPath;

        public FileBasedTestService(IFileSystem fileSystem, IConfiguration config, IFHIRValidatorService validatorService)
        {
            _fileSystem = fileSystem;
            _baseDataPath = config["Data:BasePath"] ?? "fhirut-data";
            _validatorService = validatorService;
        }

        public async Task<TestCaseFileModel> SaveTestCaseAsync(string userId, TestCaseFileModel testCase, Stream yamlFile, Stream? jsonFile)
        {
            var userPath = Path.Combine(_baseDataPath, "users", userId);
            var casePath = Path.Combine(userPath, "test-cases", testCase.Id);

            _fileSystem.Directory.CreateDirectory(casePath);

            // Salvar YAML
            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            await using (var stream = _fileSystem.File.Create(yamlPath))
            {
                await yamlFile.CopyToAsync(stream);
            }
            testCase.YamlPath = yamlPath;

            // Salvar JSON (se existir)
            if (jsonFile != null)
            {
                var jsonPath = Path.Combine(casePath, "instance.json");
                await using (var stream = _fileSystem.File.Create(jsonPath))
                {
                    await jsonFile.CopyToAsync(stream);
                }
                testCase.JsonPath = jsonPath;
            }

            return testCase;
        }

        public async Task<ValidationResultModel> RunTestCaseAsync(string userId, string caseId)
        {
            var casePath = Path.Combine(_baseDataPath, "users", userId, "test-cases", caseId);
            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            var jsonPath = Path.Combine(casePath, "instance.json");

            if (!_fileSystem.File.Exists(yamlPath) || !_fileSystem.File.Exists(jsonPath))
                throw new FileNotFoundException("Test case files not found");

            // Parse YAML (simplificado)
            var yamlContent = await _fileSystem.File.ReadAllTextAsync(yamlPath);

            // Validação real
            var operationOutcome = await _validatorService.ValidateAsync(
                jsonPath,
                new List<string> { "http://perfil.fhir.br/paciente" }); // Perfis do YAML

            return new ValidationResultModel
            {
                TestCaseId = caseId,
                Status = operationOutcome.Contains("\"severity\":\"error\"") ? "error" : "success",
                RawOperationOutcome = operationOutcome
            };
        }

    }
}
