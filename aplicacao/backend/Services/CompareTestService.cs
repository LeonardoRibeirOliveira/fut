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
                    ExpectedStatus = ConvertExpectedResultsToString(testCaseDefinition.ExpectedResults),
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

        private string ConvertExpectedResultsToString(YamlExpectedResults expectedResults)
        {
            var results = new List<string>();

            if (!string.IsNullOrWhiteSpace(expectedResults?.Status))
                results.Add($"Status: {expectedResults.Status}");

            if (expectedResults?.Errors?.Any() == true)
                results.Add($"Errors: {string.Join(", ", expectedResults.Errors)}");

            if (expectedResults?.Warnings?.Any() == true)
                results.Add($"Warnings: {string.Join(", ", expectedResults.Warnings)}");

            if (expectedResults?.Informations?.Any() == true)
                results.Add($"Informations: {string.Join(", ", expectedResults.Informations)}");

            if (expectedResults?.Invariants != null)
            {
                results.Add($"Invariant Expression: {expectedResults.Invariants.Expression}");
                results.Add($"Invariant Expected: {expectedResults.Invariants.Expected}");
            }

            return string.Join(" | ", results);
        }
    }
}
