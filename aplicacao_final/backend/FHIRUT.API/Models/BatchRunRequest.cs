namespace FHIRUT.API.Models
{
    public class BatchRunRequest
    {
        public string UserId { get; set; }
        public List<string> TestCaseIds { get; set; }
    }
}
