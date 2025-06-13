namespace FHIRUT.API.Models.Input
{
    public class YamlExpectedResults
    {
        public string Status { get; set; } = "fatal"; // Valor padrão
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Informations { get; set; } = new List<string>();
        public YamlInvariant Invariants { get; set; } = new YamlInvariant();
    }
}
