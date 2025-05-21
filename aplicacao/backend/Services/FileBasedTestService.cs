using System.IO.Abstractions;
using System.Text.Json;
using FHIRUT.API.Models.Outcome;
using FHIRUT.API.Models.Tests;
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

        public async Task<TestCaseDefinition?> SaveTestCaseAsync(string userId, TestCaseDefinition testCase, Stream yamlFile, Stream? jsonFile)
        {
            var userPath = Path.Combine(_baseDataPath, "users", userId);
            var casePath = Path.Combine(userPath, "test-cases", testCase.TestId);

            _fileSystem.Directory.CreateDirectory(casePath);

            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            await using (var stream = _fileSystem.File.Create(yamlPath))
            {
                await yamlFile.CopyToAsync(stream);
            }

            if (testCase.GetType().GetProperty("YamlPath") != null)
                testCase.GetType().GetProperty("YamlPath")?.SetValue(testCase, yamlPath);

            if (jsonFile != null)
            {
                var jsonPath = Path.Combine(casePath, "instance.json");
                await using (var stream = _fileSystem.File.Create(jsonPath))
                {
                    await jsonFile.CopyToAsync(stream);
                }

                if (testCase.GetType().GetProperty("JsonPath") != null)
                    testCase.GetType().GetProperty("JsonPath")?.SetValue(testCase, jsonPath);
            }

            return testCase;
        }


        public async Task<OperationOutcome?> RunTestCaseAsync(string userId, string caseId)
        {
            var casePath = Path.Combine(_baseDataPath, "users", userId, "test-cases", caseId);
            var yamlPath = Path.Combine(casePath, "test-case.yaml");
            var jsonPath = Path.Combine(casePath, "instance.json");

            if (!_fileSystem.File.Exists(yamlPath) || !_fileSystem.File.Exists(jsonPath))
                throw new FileNotFoundException("Test case files not found");

            var yamlContent = await _fileSystem.File.ReadAllTextAsync(yamlPath);
            var profiles = ExtractProfilesFromYaml(yamlContent);

            var operationOutcomeJson = await _validatorService.ValidateAsync(
                jsonPath,
                profiles,
                null);

            return ParseOperationOutcome(operationOutcomeJson);
        }

        public async Task<IEnumerable<TestCaseDefinition>?> GetTestCasesAsync(string userId)
        {
            var userPath = Path.Combine(_baseDataPath, "users", userId, "test-cases");

            if (!_fileSystem.Directory.Exists(userPath))
                return Enumerable.Empty<TestCaseDefinition>();

            var testCases = new List<TestCaseDefinition>();
            var caseDirectories = _fileSystem.Directory.GetDirectories(userPath);

            foreach (var caseDir in caseDirectories)
            {
                var yamlPath = Path.Combine(caseDir, "test-case.yaml");

                if (_fileSystem.File.Exists(yamlPath))
                {
                    using var reader = _fileSystem.File.OpenText(yamlPath);
                    var yamlContent = await reader.ReadToEndAsync();

                    var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                        .IgnoreUnmatchedProperties()
                        .Build();

                    var testCase = deserializer.Deserialize<TestCaseDefinition>(yamlContent);

                    // Opcional: preencher InstancePath se não estiver no YAML
                    if (string.IsNullOrEmpty(testCase.InstancePath))
                    {
                        var jsonPath = Path.Combine(caseDir, "instance.json");
                        if (_fileSystem.File.Exists(jsonPath))
                            testCase.InstancePath = jsonPath;
                    }

                    testCases.Add(testCase);
                }
            }

            return testCases;
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

            return profiles.Any() ? profiles : new List<string> { "http://hl7.org/fhir/StructureDefinition/Patient" };
        }

        private OperationOutcome? ParseOperationOutcome(string operationOutcomeJson)
        {
            if (string.IsNullOrWhiteSpace(operationOutcomeJson))
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var outcome = JsonSerializer.Deserialize<OperationOutcome>(operationOutcomeJson, options);
                return outcome;
            }
            catch
            {
                // Se não conseguir desserializar, retorna nulo
                return null;
            }
        }
    }
}
