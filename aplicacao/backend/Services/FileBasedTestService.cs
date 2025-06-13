using System.IO.Abstractions;
using FHIRUT.API.Controllers;
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
        private readonly ILogger<TestCaseController> _logger;

        public FileBasedTestService(IFileSystem fileSystem, IConfiguration config, IFHIRValidatorService validatorService, ICompareTestService compareTestService, IYamlCaseMapperService yamlCaseMapperService, ILogger<TestCaseController> logger)
        {
            _fileSystem = fileSystem;
            _baseDataPath = config["Data:BasePath"] ?? "fhirut-data";
            _validatorService = validatorService;
            _compareTestService = compareTestService;
            _yamlCaseMapperService = yamlCaseMapperService;
            _logger = logger;
        }

        public async Task<List<TestCaseResult>?> RunTestCaseAsync(List<TestCaseDefinition> testCaseRequests)
        {
            var allResults = new List<TestCaseResult>();

            foreach (var request in testCaseRequests)
            {
                try
                {
                    var testCaseDefinition = _yamlCaseMapperService.LoadTestCase(request.YamlFile);

                    var resultTasks = testCaseDefinition.InstancePath.Select(async jsonPath =>
                    {
                        try
                        {
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                            var xmlOutcome = await _validatorService.ValidateAsync(
                                jsonPath,
                                testCaseDefinition.Context.Profiles,
                                null);

                            var outcome = ParseOperationOutcome(xmlOutcome);
                            stopwatch.Stop();

                            return _compareTestService.GenerateComparedResults(
                                testCaseDefinition,
                                outcome,
                                stopwatch.Elapsed,
                                jsonPath);
                        }
                        catch (Exception innerEx)
                        {
                            _logger.LogError(innerEx, $"Erro ao validar o caminho {jsonPath}");
                            return new List<TestCaseResult>(); // ou retorne um erro tratado
                        }
                    });

                    var caseResults = await System.Threading.Tasks.Task.WhenAll(resultTasks);
                    allResults.AddRange(caseResults.SelectMany(r => r));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao processar o teste do arquivo {request.YamlFile}");
                }
            }

            return allResults;
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
