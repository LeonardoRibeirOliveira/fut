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

        public async Task<List<OperationOutcome>?> RunTestCaseAsync(string yamlFilePath)
        {
            try
            {
                if (!_fileSystem.File.Exists(yamlFilePath))
                    throw new FileNotFoundException("YAML file not found", yamlFilePath);

                var yamlContent = await _fileSystem.File.ReadAllTextAsync(yamlFilePath);


                var profiles = ExtractProfilesFromYaml(yamlContent);
                var jsonPaths = ExtractInstancePathsFromYaml(yamlContent);

                var validationTasks = jsonPaths.Select(async path =>
                {
                    var xmlOutcome = await _validatorService.ValidateAsync(path, profiles, null);
                    return ParseOperationOutcome(xmlOutcome);
                });

                var outcomes = await System.Threading.Tasks.Task.WhenAll(validationTasks);
                return outcomes.Where(o => o != null).ToList()!;
            }
            catch (Exception ex)
            {
                throw new Exception("Error running test case", ex);
            }
        }


        private List<string> ExtractInstancePathsFromYaml(string yamlContent)
        {
            var instancePaths = new List<string>();
            var lines = yamlContent.Split('\n');

            bool inInstanceSection = false;
            int baseIndentation = -1;

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd();

                if (line.TrimStart().StartsWith("instance_path:"))
                {
                    inInstanceSection = true;
                    baseIndentation = rawLine.IndexOf("instance_path:");
                    var index = line.IndexOf('[');
                    if (index >= 0)
                    {
                        var inlineValues = line.Substring(index)
                            .Trim('[', ']')
                            .Split(',')
                            .Select(x => x.Trim().Trim('"', '\''))
                            .Where(x => !string.IsNullOrEmpty(x));

                        instancePaths.AddRange(inlineValues);
                        inInstanceSection = false;
                    }

                    continue;
                }

                if (inInstanceSection)
                {
                    int currentIndentation = rawLine.TakeWhile(Char.IsWhiteSpace).Count();

                    if (currentIndentation <= baseIndentation || string.IsNullOrWhiteSpace(line))
                    {
                        inInstanceSection = false;
                        continue;
                    }

                    if (line.TrimStart().StartsWith("-"))
                    {
                        var path = line.TrimStart().Substring(1).Trim().Trim('"').Trim('\'');
                        if (!string.IsNullOrEmpty(path))
                            instancePaths.Add(path);
                    }
                }
            }

            return instancePaths;
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
                    int currentIndentation = rawLine.TakeWhile(Char.IsWhiteSpace).Count();

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
