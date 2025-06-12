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
                var igs = ExtractIgsFromYaml(yamlContent);
                var outcomes = new List<OperationOutcome>();

                // CORREÇÃO FINAL: Usa o diretório de trabalho atual (a raiz do projeto "backend")
                // para construir o caminho absoluto.
                var absolutePathIgs = igs.Select(ig => 
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), _baseDataPath, ig))
                ).ToList();

                foreach (var jsonContent in jsonContents)
                {
                    var tempJsonPath = Path.GetTempFileName();
                    await _fileSystem.File.WriteAllTextAsync(tempJsonPath, jsonContent);

                    var xmlOutcome = await _validatorService.ValidateAsync(
                        tempJsonPath,
                        profiles,
                        absolutePathIgs);

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

        private List<string> ExtractIgsFromYaml(string yamlContent)
        {
            var igs = new List<string>();
            var lines = yamlContent.Split('\n');
            bool inIgsSection = false;
            int baseIndentation = -1;

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd();
                if (line.TrimStart().StartsWith("igs:"))
                {
                    inIgsSection = true;
                    baseIndentation = rawLine.IndexOf("igs:");
                    continue;
                }

                if (inIgsSection)
                {
                    int currentIndentation = rawLine.TakeWhile(char.IsWhiteSpace).Count();
                    if (currentIndentation <= baseIndentation || string.IsNullOrWhiteSpace(line))
                    {
                        inIgsSection = false;
                        continue;
                    }

                    if (line.TrimStart().StartsWith("-"))
                    {
                        var ig = line.TrimStart().Substring(1).Trim().Trim('"').Trim('\'');
                        if (!string.IsNullOrEmpty(ig))
                            igs.Add(ig);
                    }
                }
            }
            return igs;
        }

        private List<string> ExtractProfilesFromYaml(string yamlContent)
        {
            var profiles = new List<string>();
            var lines = yamlContent.Split('\n');
            bool inProfilesSection = false;
            int baseIndentation = -1;

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd();
                if (line.TrimStart().StartsWith("profiles:"))
                {
                    inProfilesSection = true;
                    baseIndentation = rawLine.IndexOf("profiles:");
                    continue;
                }

                if (inProfilesSection)
                {
                    int currentIndentation = rawLine.TakeWhile(char.IsWhiteSpace).Count();
                    if (currentIndentation <= baseIndentation || string.IsNullOrWhiteSpace(line))
                    {
                        inProfilesSection = false;
                        continue;
                    }

                    if (line.TrimStart().StartsWith("-"))
                    {
                        var profile = line.TrimStart().Substring(1).Trim().Trim('"').Trim('\'');
                        if (!string.IsNullOrEmpty(profile))
                            profiles.Add(profile);
                    }
                }
            }
            return profiles;
        }
        
        private OperationOutcome? ParseOperationOutcome(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                Console.WriteLine("Erro: O resultado da validação está vazio. O validador pode ter falhado.");
                return new OperationOutcome
                {
                    Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Severity = OperationOutcome.IssueSeverity.Error,
                            Details = new CodeableConcept { Text = "O validador FHIR não retornou um resultado. Verifique os logs do backend." }
                        }
                    }
                };
            }

            var parser = new FhirXmlParser();
            try
            {
                var parsed = parser.Parse<OperationOutcome>(xml);
                return parsed;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Erro ao fazer parse do OperationOutcome: {ex.Message}");
                return new OperationOutcome
                {
                     Issue = new List<OperationOutcome.IssueComponent>
                    {
                        new OperationOutcome.IssueComponent
                        {
                            Severity = OperationOutcome.IssueSeverity.Fatal,
                            Details = new CodeableConcept { Text = $"Falha ao fazer parse do XML do OperationOutcome: {ex.Message}" }
                        }
                    }
                };
            }
        }
    }
}