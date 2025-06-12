using FHIRUT.API.Models.Result;
using FHIRUT.API.Models.Tests;
using Hl7.Fhir.Model;

namespace FHIRUT.API.Services.Interfaces
{
    public interface ICompareTestService
    {
        List<TestCaseResult> GenerateComparedResults(OperationOutcome outcome, TimeSpan executionTime);
    }

}
