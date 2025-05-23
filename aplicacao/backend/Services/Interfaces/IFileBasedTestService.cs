using FHIRUT.API.Models.Outcome;
using FHIRUT.API.Models.Tests;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IFileBasedTestService
    {
        Task<OperationOutcome?> RunTestCaseAsync(string yamlContent, List<string> jsonContents);
    }
}
