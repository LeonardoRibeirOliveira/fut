namespace FHIRUT.API.Models.Input
{
    public class YamlContext
    {
        public List<string> Igs { get; set; } = new List<string>();
        public List<string> Profiles { get; set; } = new List<string>();
        public List<string> Resources { get; set; } = new List<string>();
    }
}
