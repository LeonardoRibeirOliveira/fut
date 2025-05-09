namespace FHIRUT.API.Models
{
    public class TestCaseFileModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? YamlPath { get; set; }
        public string? JsonPath { get; set; }
        public string? UserId { get; set; }
    }
}
