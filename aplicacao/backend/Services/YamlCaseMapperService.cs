using System.IO.Abstractions;
using FHIRUT.API.Models.Input;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model.CdsHooks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FHIRUT.API.Services
{
    public class YamlCaseMapperService : IYamlCaseMapperService
    {
        private readonly IFileSystem _fileSystem;

        public YamlCaseMapperService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public YamlTestCaseDefinition LoadTestCase(string yamlFile)
        {

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();


            var testCase = deserializer.Deserialize<YamlTestCaseDefinition>(yamlFile);

            return new YamlTestCaseDefinition
            {
                TestId = testCase.TestId,
                Description = testCase.Description,
                Context = new YamlContext
                {
                    Igs = testCase.Context?.Igs ?? new List<string>(),
                    Profiles = testCase.Context?.Profiles ?? new List<string>(),
                    Resources = testCase.Context?.Resources ?? new List<string>()
                },
                InstancePath = testCase.InstancePath,
                ExpectedResults = new YamlExpectedResults
                {
                    Status = testCase.ExpectedResults?.Status ?? "success",
                    Errors = testCase.ExpectedResults?.Errors ?? new List<string>(),
                    Warnings = testCase.ExpectedResults?.Warnings ?? new List<string>(),
                    Informations = testCase.ExpectedResults?.Informations ?? new List<string>(),
                    Invariants = new YamlInvariant
                    {
                        Expression = testCase.ExpectedResults?.Invariants.Expression,
                        Expected = testCase.ExpectedResults?.Invariants.Expected ?? true
                    }
                }
            };
        }
    }
}
