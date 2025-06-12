using Hl7.Fhir.Model;

namespace FHIRUT.API.Models.Result
{
    public class TestCaseResult
    {
        public string TestId { get; set; } = "";
        public string YamlId { get; set; } = "";
        public string ExpectedStatus { get; set; } = "";
        public string ActualStatus { get; set; } = "";
        public List<IssueSummary> Issues { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }

        public OperationOutcome? OperationOutcome { get; set; }
    }

    public class IssueSummary
    {
        public string Severity { get; set; } = "";
        public string Location { get; set; } = "";
        public string Code { get; set; } = "";
        public string Details { get; set; } = "";
        public string Source { get; set; } = "";
    }
}
