namespace FHIRUT.API.Models
{
    public class BatchRunResponse
    {
        public string? UserId { get; set; }
        public List<ValidationResultModel>? Results { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
    }
}
