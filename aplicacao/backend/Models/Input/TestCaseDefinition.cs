using System.ComponentModel.DataAnnotations;

namespace FHIRUT.API.Models.Tests
{
    public class TestCaseDefinition
    {
        [Required]
        public string YamlFilePath { get; set; } = string.Empty;
    }
}