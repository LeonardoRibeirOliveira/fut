using System.IO.Abstractions;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

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

        public async Task<List<OperationOutcome>?> RunTestCaseAsync(string yamlContent, List<string> jsonContents)
        {
            try
            {
                var profiles = ExtractProfilesFromYaml(yamlContent);
                var outcomes = new List<OperationOutcome>();

                foreach (var jsonContent in jsonContents)
                {
                    var tempJsonPath = Path.GetTempFileName();
                    await _fileSystem.File.WriteAllTextAsync(tempJsonPath, jsonContent);

                    var xmlOutcome = await _validatorService.ValidateAsync(
                        tempJsonPath,
                        profiles,
                        null);

                    var parsedOutcome = ParseOperationOutcome(xmlOutcome);

                    if (parsedOutcome != null)
                    {
                        outcomes.Add(parsedOutcome);
                    }

                    _fileSystem.File.Delete(tempJsonPath);
                }

                return outcomes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error running test case", ex);
            }
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

        public OperationOutcome? ParseOperationOutcome(string xml)
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
                return null;
            }
        }
    }
}
