using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
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

        public async Task<OperationOutcome?> RunTestCaseAsync(string yamlContent, List<string> jsonContents)
        {
            try
            {
                var profiles = ExtractProfilesFromYaml(yamlContent);
                var outcomes = new List<OperationOutcome>();

                foreach (var jsonContent in jsonContents)
                {
                    var tempJsonPath = Path.GetTempFileName();
                    await _fileSystem.File.WriteAllTextAsync(tempJsonPath, jsonContent);

                    var operationOutcome = await _validatorService.ValidateAsync(
                        tempJsonPath,
                        profiles,
                        null);

                    var parsedOutcome = ParseOperationOutcome(operationOutcome);

                    if (parsedOutcome != null)
                    {
                        outcomes.Add(parsedOutcome);
                    }

                    _fileSystem.File.Delete(tempJsonPath);
                }

                return CombineOperationOutcomes(outcomes);
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

        private OperationOutcome? ParseOperationOutcome(string operationOutcomeJson)
        {
            if (string.IsNullOrWhiteSpace(operationOutcomeJson))
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() } // Para lidar com enums se houver
            };

            try
            {
                    var xmlDoc = XDocument.Parse(operationOutcomeJson);
                    operationOutcomeJson = JsonSerializer.Serialize(xmlDoc);
                    var outcome = JsonSerializer.Deserialize<OperationOutcome>(operationOutcomeJson, options);
                

                return outcome;
            }
            catch (Exception ex)
            {
                return new OperationOutcome
                {
                    Issue = new List<OperationOutcomeIssue>
                    {
                        new OperationOutcomeIssue
                        {
                            Severity = "error",
                            Code = "invalid-content",
                            Diagnostics = $"Failed to parse OperationOutcome: {ex.Message}",
                            Details = new CodeableConcept
                            {
                                Text = "Parsing error"
                            }
                        }
                    }
                };
            }
        }

        private OperationOutcome CombineOperationOutcomes(List<OperationOutcome> outcomes)
        {
            if (outcomes == null || outcomes.Count == 0)
            {
                return new OperationOutcome();
            }

            // Se houver apenas um outcome, retorna ele mesmo
            if (outcomes.Count == 1)
            {
                return outcomes[0];
            }

            var combinedOutcome = new OperationOutcome
            {
                ResourceType = "OperationOutcome",
                // Mantém o ID do primeiro outcome ou gera um novo se necessário
                Id = outcomes.FirstOrDefault(o => !string.IsNullOrEmpty(o.Id))?.Id,
                // Combina os textos (se houver)
                Text = CombineNarratives(outcomes.Select(o => o.Text).ToList()),
                // Combina todas as issues
                Issue = outcomes.SelectMany(o => o.Issue).ToList(),
                // Combina as extensões únicas
                Extension = CombineExtensions(outcomes.Select(o => o.Extension).ToList())
            };

            return combinedOutcome;
        }

        private Narrative? CombineNarratives(List<Narrative?> narratives)
        {
            // Filtra narratives não nulas
            var validNarratives = narratives.Where(n => n != null).ToList();

            if (validNarratives.Count == 0)
            {
                return null;
            }

            // Se houver apenas uma narrative, retorna ela
            if (validNarratives.Count == 1)
            {
                return validNarratives[0];
            }

            // Combina múltiplas narratives em uma
            return new Narrative
            {
                Status = "generated",
                Div = string.Join("<hr/>", validNarratives.Select(n => n?.Div))
            };
        }

        private List<Extension>? CombineExtensions(List<List<Extension>?> extensionsLists)
        {
            // Filtra listas não nulas
            var validExtensions = extensionsLists.Where(e => e != null).ToList();

            if (validExtensions.Count == 0)
            {
                return null;
            }

            // Combina todas as extensões em uma única lista
            var combined = validExtensions.SelectMany(e => e!).ToList();

            // Remove duplicatas baseadas na URL
            return combined
                .GroupBy(e => e.Url)
                .Select(g => g.First())
                .ToList();
        }
    }
}
