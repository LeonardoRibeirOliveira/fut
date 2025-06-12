using System.IO.Abstractions;
using FHIRUT.API.Models.Result;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FHIRUT.API.Services
{
    public class FileBasedTestService : IFileBasedTestService
    {
        private readonly IFHIRValidatorService _validatorService;
        private readonly IFileSystem _fileSystem;
        private readonly ICompareTestService _compareTestService;
        private readonly IYamlCaseMapperService _yamlCaseMapperService;
        private readonly string _baseDataPath;

        public FileBasedTestService(IFileSystem fileSystem, IConfiguration config, IFHIRValidatorService validatorService, ICompareTestService compareTestService, IYamlCaseMapperService yamlCaseMapperService)
        {
            _fileSystem = fileSystem;
            _baseDataPath = config["Data:BasePath"] ?? "fhirut-data";
            _validatorService = validatorService;
            _compareTestService = compareTestService;
            _yamlCaseMapperService = yamlCaseMapperService;
        }

        public async Task<List<TestCaseResult>?> RunTestCaseAsync(List<TestCaseDefinition> testCaseRequests)
        {
            try
            {
                var testTasks = testCaseRequests.Select(async request =>
                {
                    var testCaseDefinition = await _yamlCaseMapperService.LoadTestCaseAsync(request.YamlFilePath);

                    var resultTasks = testCaseDefinition.InstancePath.Select(async jsonPath =>
                    {
                        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                        var xmlOutcome = await _validatorService.ValidateAsync(
                            jsonPath,
                            testCaseDefinition.Context.Profiles,
                            null);

                        var outcome = ParseOperationOutcome(xmlOutcome);
                        stopwatch.Stop();

                        return _compareTestService.GenerateComparedResults(testCaseDefinition, outcome, stopwatch.Elapsed, request.YamlFilePath);
                    });

                    var results = await System.Threading.Tasks.Task.WhenAll(resultTasks);
                    return results.SelectMany(r => r).ToList();
                });

                var allResultGroups = await System.Threading.Tasks.Task.WhenAll(testTasks);
                return allResultGroups.SelectMany(r => r).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error running test case", ex);
            }
        }




        private OperationOutcome ParseOperationOutcome(string xml)
        {
            var parser = new FhirXmlParser();

            try
            {
                var parsed = parser.Parse<OperationOutcome>(xml);
                return parsed;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Erro ao fazer parse do OperationOutcome: {ex.Message}");
                return new OperationOutcome();
            }
        }
    }
}
