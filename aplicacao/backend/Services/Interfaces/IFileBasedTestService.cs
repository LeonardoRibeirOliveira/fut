using Hl7.Fhir.Model;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IFileBasedTestService
    {
        Task<List<OperationOutcome>?> RunTestCaseAsync(string yamlContent);
    }
}
