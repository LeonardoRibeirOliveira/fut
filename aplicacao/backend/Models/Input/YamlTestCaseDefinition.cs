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
}