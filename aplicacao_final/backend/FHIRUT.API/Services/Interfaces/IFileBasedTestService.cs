using FHIRUT.API.Models;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IFileBasedTestService
    {
        Task<TestCaseFileModel> SaveTestCaseAsync(string userId, TestCaseFileModel testCase, Stream yamlFile, Stream? jsonFile);
        Task<ValidationResultModel> RunTestCaseAsync(string userId, string caseId);
    }
}
