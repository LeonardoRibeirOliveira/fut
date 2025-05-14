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

        public async Task<TestCaseFileModel?> SaveTestCaseAsync(string userId, TestCaseFileModel testCase, Stream yamlFile, Stream? jsonFile)
        {
            var userPath = Path.Combine(_baseDataPath, "users", userId);
            var casePath = Path.Combine(userPath, "test-cases", testCase.Id);

            _fileSystem.Directory.CreateDirectory(casePath);

            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            await using (var stream = _fileSystem.File.Create(yamlPath))
            {
                await yamlFile.CopyToAsync(stream);
            }
            testCase.YamlPath = yamlPath;

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

        public async Task<ValidationResultModel?> RunTestCaseAsync(string userId, string caseId)
        {
            var casePath = Path.Combine(_baseDataPath, "users", userId, "test-cases", caseId);
            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            var jsonPath = Path.Combine(casePath, "instance.json");

            if (!_fileSystem.File.Exists(yamlPath) || !_fileSystem.File.Exists(jsonPath))
                throw new FileNotFoundException("Test case files not found");

            var yamlContent = await _fileSystem.File.ReadAllTextAsync(yamlPath);
            var profiles = ExtractProfilesFromYaml(yamlContent);

            var operationOutcome = await _validatorService.ValidateAsync(
                jsonPath,
                profiles,
                null);

            return ParseOperationOutcome(caseId, operationOutcome);
        }

        public async Task<IEnumerable<TestCaseFileModel>?> GetTestCasesAsync(string userId)
        {
            var userPath = Path.Combine(_baseDataPath, "users", userId, "test-cases");

            if (!_fileSystem.Directory.Exists(userPath))
                return Enumerable.Empty<TestCaseFileModel>();

            var testCases = new List<TestCaseFileModel>();
            var caseDirectories = _fileSystem.Directory.GetDirectories(userPath);

            foreach (var caseDir in caseDirectories)
            {
                var caseId = Path.GetFileName(caseDir);
                var yamlPath = Path.Combine(caseDir, "test-case.yaml");
                var jsonPath = Path.Combine(caseDir, "instance.json");

                if (_fileSystem.File.Exists(yamlPath))
                {
                    testCases.Add(new TestCaseFileModel
                    {
                        Id = caseId,
                        Name = Path.GetFileNameWithoutExtension(yamlPath),
                        YamlPath = yamlPath,
                        JsonPath = _fileSystem.File.Exists(jsonPath) ? jsonPath : null,
                        UserId = userId,
                        CreatedAt = _fileSystem.File.GetLastWriteTime(yamlPath)
                    });
                }
            }

            return testCases.OrderByDescending(tc => tc.CreatedAt);
        }

        public async Task<TestCaseFileModel?> GetTestCaseAsync(string userId, string caseId)
        {
            var casePath = Path.Combine(_baseDataPath, "users", userId, "test-cases", caseId);
            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            var jsonPath = Path.Combine(casePath, "instance.json");

            if (!_fileSystem.File.Exists(yamlPath))
                return null;

            return new TestCaseFileModel
            {
                Id = caseId,
                Name = Path.GetFileNameWithoutExtension(yamlPath),
                YamlPath = yamlPath,
                JsonPath = _fileSystem.File.Exists(jsonPath) ? jsonPath : null,
                UserId = userId,
                CreatedAt = _fileSystem.File.GetLastWriteTime(yamlPath)
            };
        }

        private List<string> ExtractProfilesFromYaml(string yamlContent)
        {
            var profiles = new List<string>();

            if (yamlContent.Contains("profile:"))
            {
                var lines = yamlContent.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Trim().StartsWith("profile:"))
                    {
                        var profile = line.Split(':')[1].Trim().Trim('"').Trim('\'');
                        if (!string.IsNullOrEmpty(profile))
                            profiles.Add(profile);
                    }
                }
            }

            return profiles.Any() ? profiles : new List<string> { "http://perfil.fhir.br/paciente" };
        }

        private ValidationResultModel ParseOperationOutcome(string caseId, string operationOutcome)
        {
            var hasErrors = operationOutcome.Contains("\"severity\":\"error\"");
            var hasWarnings = operationOutcome.Contains("\"severity\":\"warning\"");

            var result = new ValidationResultModel
            {
                TestCaseId = caseId,
                Status = hasErrors ? "error" : (hasWarnings ? "warning" : "success"),
                RawOperationOutcome = operationOutcome
            };

            return result;
        }

    }
}
