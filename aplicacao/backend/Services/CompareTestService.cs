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
            var expected = testCaseDefinition.ExpectedResults;
            var validationMessages = new List<string>();

            var actualStatus = outcome.Issue.Any(i => i.Severity == OperationOutcome.IssueSeverity.Error)
                ? "error"
                : outcome.Issue.Any(i => i.Severity == OperationOutcome.IssueSeverity.Warning)
                    ? "warning"
                    : outcome.Issue.Any(i => i.Severity == OperationOutcome.IssueSeverity.Information)
                        ? "information"
                        : "success";

            bool passed = CompareExpectedAndActualResults(outcome, expected, validationMessages, actualStatus);

            return new List<TestCaseResult>
            {
                new TestCaseResult
                {
                    TestId = testCaseDefinition.TestId,
                    JsonId = jsonPath,
                    ExpectedStatus = ConvertExpectedResultsToString(expected),
                    ActualStatus = actualStatus,
                    ExecutionTime = executionTime,
                    Passed = passed,
                    ValidationMessages = validationMessages,
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
                    OperationOutcome = outcome
                }
            };
        }

        private bool CompareExpectedAndActualResults(OperationOutcome outcome, YamlExpectedResults expected, List<string> validationMessages, string actualStatus)
        {
            var actualErrors = outcome.Issue
                            .Where(i => i.Severity == OperationOutcome.IssueSeverity.Error)
                            .Select(i => i.Details?.Text?.Trim() ?? "")
                            .ToList();

            var actualWarnings = outcome.Issue
                .Where(i => i.Severity == OperationOutcome.IssueSeverity.Warning)
                .Select(i => i.Details?.Text?.Trim() ?? "")
                .ToList();

            var actualInfos = outcome.Issue
                .Where(i => i.Severity == OperationOutcome.IssueSeverity.Information)
                .Select(i => i.Details?.Text?.Trim() ?? "")
                .ToList();

            bool statusMatches = expected.Status.Equals(actualStatus, StringComparison.OrdinalIgnoreCase);
            if (!statusMatches)
                validationMessages.Add($"Status esperado {expected.Status.ToUpper()} difere do retornado pelo fhir {actualStatus.ToUpper()}.");

            bool errorsMatch = ListsMatch(expected.Errors, actualErrors);
            if (!errorsMatch)
                validationMessages.Add("Os ERRORS retornados pelo fhir não correspondem exatamente aos ERRORS esperados.");

            bool warningsMatch = ListsMatch(expected.Warnings, actualWarnings);
            if (!warningsMatch)
                validationMessages.Add("Os WARNINGS retornados pelo fhir não correspondem exatamente aos WARNINGS esperados.");

            bool infosMatch = ListsMatch(expected.Informations, actualInfos);
            if (!infosMatch)
                validationMessages.Add("As INFORMATIONS retornados pelo fhir não correspondem exatamente aos INFORMATIONS esperadas.");

            bool passed = statusMatches && errorsMatch && warningsMatch && infosMatch;
            return passed;
        }

        private bool ListsMatch(List<string> expected, List<string> actual)
        {
            var expectedSet = new HashSet<string>(expected.Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
            var actualSet = new HashSet<string>(actual.Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
            return expectedSet.SetEquals(actualSet);
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

            return string.Join(" | ", results);
        }
    }
}
