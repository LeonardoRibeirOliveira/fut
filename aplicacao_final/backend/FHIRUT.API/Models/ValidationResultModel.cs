namespace FHIRUT.API.Models
{
    public class ValidationResultModel
    {
        public string TestCaseId { get; set; }
        public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } // "success", "error", "warning"
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public string RawOperationOutcome { get; set; }
    }
}
