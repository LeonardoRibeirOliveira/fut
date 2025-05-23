namespace FHIRUT.API.Models.Outcome
{
    public class OperationOutcome
    {
        public string ResourceType { get; set; } = "OperationOutcome";
        public string? Id { get; set; }
        public Narrative? Text { get; set; }
        public List<Extension>? Extension { get; set; }
        public List<OperationOutcomeIssue> Issue { get; set; } = new();
    }

    public class Narrative
    {
        public string? Status { get; set; }
        public string? Div { get; set; }
    }

    public class OperationOutcomeIssue
    {
        public string Severity { get; set; } = null!;
        public string Code { get; set; } = null!;
        public CodeableConcept? Details { get; set; }
        public string? Diagnostics { get; set; }
        public List<string>? Location { get; set; }
        public List<string>? Expression { get; set; }
        public List<Extension>? Extension { get; set; }
        public CodeableConcept? Source { get; set; }
    }

    public class CodeableConcept
    {
        public List<Coding>? Coding { get; set; }
        public string? Text { get; set; }
    }

    public class Coding
    {
        public string? System { get; set; }
        public string? Code { get; set; }
        public string? Display { get; set; }
    }

    public class Extension
    {
        public string Url { get; set; } = null!;
        public string? ValueString { get; set; }
        public int? ValueInteger { get; set; }
        public string? ValueCode { get; set; }
        public bool? ValueBoolean { get; set; }
        public decimal? ValueDecimal { get; set; }
        public DateTime? ValueDateTime { get; set; }
    }
}