using FHIRUT.API.Models.Input;
using FHIRUT.API.Models.Result;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model;

namespace FHIRUT.API.Services
{
    public class CompareTestService : ICompareTestService
    {
        public List<TestCaseResult> GenerateComparedResults(
            YamlTestCaseDefinition testCaseDefinition,
            OperationOutcome outcome,
            TimeSpan executionTime,
            string jsonPath)
        {
            var status = outcome.Issue.Any(i => i.Severity == OperationOutcome.IssueSeverity.Error)
                ? "error"
                : outcome.Issue.Any(i => i.Severity == OperationOutcome.IssueSeverity.Warning)
                    ? "warning"
                    : "success";

            return new List<TestCaseResult>
            {
                new TestCaseResult
                {
                    TestId = testCaseDefinition.TestId,
                    JsonId = jsonPath,
                    ExpectedStatus = testCaseDefinition.ExpectedResults.ToString() ?? "",
                    ActualStatus = status,
                    ExecutionTime = executionTime,
                    Issues = outcome.Issue.Select(io => new IssueSummary
                    {
                        Severity = io.Severity.ToString().ToLower(),
                        Location = io.Expression?.FirstOrDefault() ?? "",
                        Code = io.Code?.ToString() ?? "",
                        Details = io.Details?.Text ?? "",
                        Source = io.Extension?
                            .FirstOrDefault(e => e.Url.Contains("issue-source"))?
                            .Value?.ToString() ?? "validator"
                    }).ToList(),
                    OperationOutcome = outcome,
                }
            };
        }

    }

}
