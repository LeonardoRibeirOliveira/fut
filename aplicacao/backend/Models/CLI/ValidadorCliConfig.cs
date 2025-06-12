namespace FHIRUT.API.Models.CLI;

public class ValidadorCliConfig
{
    // Inicialize as propriedades com strings vazias para resolver o aviso.
    public string BasePath { get; set; } = string.Empty;
    public string ValidatorPath { get; set; } = string.Empty;
    public string ValidatorDownloadUrl { get; set; } = string.Empty;
}