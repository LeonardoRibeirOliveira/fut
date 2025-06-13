using FHIRUT.API.Models.Result;
using FHIRUT.API.Models.Tests;
using Hl7.Fhir.Model;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IFileBasedTestService
    {
        Task<List<TestCaseResult>?> RunTestCaseAsync(List<TestCaseDefinition> yamlContent);
    }
}
