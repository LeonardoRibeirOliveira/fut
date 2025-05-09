namespace FHIRUT.API.Services.Interfaces
{
    public interface IFHIRValidatorService
    {
        Task<string> ValidateAsync(string instancePath, List<string>? profiles = null, List<string>? igs = null);
    }
}
