using FHIRUT.API.Models.Outcome;
using FHIRUT.API.Models.Tests;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IFileBasedTestService
    {
        Task<TestCaseDefinition?> SaveTestCaseAsync(string userId, TestCaseDefinition testCase, Stream yamlFile, Stream? jsonFile);
        Task<OperationOutcome?> RunTestCaseAsync(string userId, string caseId);
        Task<IEnumerable<TestCaseDefinition>?> GetTestCasesAsync(string userId);
    }
}
