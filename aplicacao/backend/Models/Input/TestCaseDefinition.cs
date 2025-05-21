namespace FHIRUT.API.Models.Tests
{
    public class TestCaseDefinition
    {
        public string TestId { get; set; } = Guid.NewGuid().ToString();
        public string? Description { get; set; }
        public TestContext? Context { get; set; }
        public string? InstancePath { get; set; }
        public ExpectedResults? ExpectedResults { get; set; }
    }

    public class TestContext
    {
        public List<string>? Igs { get; set; }
        public List<string>? Profiles { get; set; }
        public List<string>? Resources { get; set; }
    }

    public class ExpectedResults
    {
        public string? Status { get; set; }
        public List<string>? Errors { get; set; }
        public List<string>? Warnings { get; set; }
        public List<string>? Informations { get; set; }
        public List<InvariantResult>? Invariants { get; set; }
    }

    public class InvariantResult
    {
        public string Expression { get; set; } = null!;
        public bool? Expected { get; set; }
    }
}