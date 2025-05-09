namespace FHIRUT.API.Models;

public class DataOptions
{
    public string BasePath { get; set; } = "fhirut-data";
    public string ValidatorPath { get; set; } = "fhirut-data/system/validator_cli.jar";
    public string ValidatorDownloadUrl { get; set; } = "https://github.com/hapifhir/org.hl7.fhir.core/releases/latest/download/validator_cli.jar";
}