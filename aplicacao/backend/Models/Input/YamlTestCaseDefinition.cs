using YamlDotNet.Serialization;

namespace FHIRUT.API.Models.Input
{
    public class YamlTestCaseDefinition
    {
        [YamlMember(Alias = "test_id")]
        public string TestId { get; set; }

        public string Description { get; set; }

        public YamlContext Context { get; set; } = new YamlContext();

        [YamlMember(Alias = "instance_path")]
        public List<string> InstancePath { get; set; } = new List<string>();

        [YamlMember(Alias = "expected_results")]
        public YamlExpectedResults ExpectedResults { get; set; } = new YamlExpectedResults();
    }

    public class YamlContext
    {
        public List<string> Igs { get; set; } = new List<string>();
        public List<string> Profiles { get; set; } = new List<string>();
        public List<string> Resources { get; set; } = new List<string>();
    }

    public class YamlInvariant
    {
        public string Expression { get; set; }
        public bool Expected { get; set; } = true;
    }
}