using Hl7.Fhir.Model;

namespace FHIRUT.API.Models.Result
{
    public class TestCaseResult
    {
        public string TestId { get; set; } = "";
        public string JsonId { get; set; } = "";
        public string ExpectedStatus { get; set; } = "";
        public string ActualStatus { get; set; } = "";
        public List<IssueSummary> Issues { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public bool Passed { get; set; }
        public List<string> ValidationMessages { get; set; } = new();

        public OperationOutcome? OperationOutcome { get; set; }
    }
}
