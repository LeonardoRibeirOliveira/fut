using YamlDotNet.Serialization;

namespace FHIRUT.API.Models.Input
{
    public class YamlTestCaseDefinition
    {
        [YamlMember(Alias = "test_id")]
        public string TestId { get; set; }

        public string Description { get; set; }

        public YamlContext Context { get; set; }

        [YamlMember(Alias = "instance_path")]
        public object InstancePath { get; set; } // Pode ser string ou lista

        [YamlMember(Alias = "expected_results")]
        public YamlExpectedResults ExpectedResults { get; set; }
    }

    public class YamlTestCaseDefinitionList
    {
        [YamlMember(Alias = "test_cases")]
        public List<YamlTestCaseDefinition> TestCases { get; set; }
    }

    public class YamlContext
    {
        public List<string> Igs { get; set; }
        public List<string> Profiles { get; set; }
        public List<string> Resources { get; set; }
    }

    public class YamlExpectedResults
    {
        public string Status { get; set; }
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Informations { get; set; }
        public List<YamlInvariant> Invariants { get; set; }
    }

    public class YamlInvariant
    {
        public string Expression { get; set; }
        public bool Expected { get; set; } = true;
    }
}
