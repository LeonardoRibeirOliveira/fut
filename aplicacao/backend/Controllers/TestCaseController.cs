using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.AssemblyIdentityComparer;

namespace FHIRUT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestCaseController : ControllerBase
{
    private readonly IFileBasedTestService _testService;
    private readonly ILogger<TestCaseController> _logger;

    public TestCaseController(IFileBasedTestService testService, ILogger<TestCaseController> logger)
    {
        _testService = testService;
        _logger = logger;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunTestCases(
    [FromBody] List<TestCaseDefinition> testCaseRequests)
    {
        if (testCaseRequests == null || !testCaseRequests.Any())
            return BadRequest("Informe algum test case.");

        try
        {
            var results = new List<List<OperationOutcome>?>();

            foreach (var request in testCaseRequests)
            {
                var result = await _testService.RunTestCaseAsync(request.YamlFile, request.JsonFiles);
                if (result != null) results.Add(result);
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running batch validation");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost("run-and-compare")]
    public async Task<IActionResult> RunAndCompareTestCases(
    [FromBody] RunAndCompareTestCasesRequest request)  // ✅ Apenas um FromBody
    {
        if (request.TestCaseRequests == null || !request.TestCaseRequests.Any())
            return BadRequest("Informe algum test case.");

        if (request.ExpectedOutcomes == null || !request.ExpectedOutcomes.Any())
            return BadRequest("Informe os resultados esperados.");

        try
        {
            var actualResults = new List<List<OperationOutcome>?>();
            foreach (var testCase in request.TestCaseRequests)
            {
                var result = await _testService.RunTestCaseAsync(testCase.YamlFile, testCase.JsonFiles);
                if (result != null) actualResults.Add(result);
            }

            var comparisonResult = CompareResultsWithDetails(actualResults, request.ExpectedOutcomes);
            return Ok(comparisonResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar e comparar testes");
            return StatusCode(500, "Internal server error");
        }
    }
    private ComparisonResult CompareResultsWithDetails(
    List<List<OperationOutcome>?> actualResults,
    List<List<OperationOutcome>> expectedOutcomes)
    {
        var comparisonResult = new ComparisonResult
        {
            AreEqual = true,
            Differences = new List<DifferenceDetail>()
        };

        // Verifica cada lista de resultados (por teste)
        for (int i = 0; i < actualResults.Count; i++)
        {
            var actualList = actualResults[i];
            var expectedList = expectedOutcomes[i];

            // Se um dos resultados for nulo ou tiver tamanhos diferentes, há diferença
            if (actualList == null || actualList.Count != expectedList.Count)
            {
                comparisonResult.AreEqual = false;
                comparisonResult.Differences.Add(new DifferenceDetail
                {
                    TestCaseIndex = i,
                    Message = $"O teste {i} teve {actualList?.Count ?? 0} resultados, mas esperava-se {expectedList.Count}."
                });
                continue;
            }

            // Compara cada OperationOutcome dentro da lista
            for (int j = 0; j < actualList.Count; j++)
            {
                var actualOutcome = actualList[j];
                var expectedOutcome = expectedList[j];

                // Compara as propriedades relevantes (ex: severidade, código do erro)
                if (actualOutcome.Issue.Count != expectedOutcome.Issue.Count ||
                    actualOutcome.Issue[0].Severity != expectedOutcome.Issue[0].Severity ||
                    actualOutcome.Issue[0].Code != expectedOutcome.Issue[0].Code)
                {
                    comparisonResult.AreEqual = false;
                    comparisonResult.Differences.Add(new DifferenceDetail
                    {
                        TestCaseIndex = i,
                        OutcomeIndex = j,
                        Expected = new SimplifiedOutcome
                        {
                            Severity = expectedOutcome.Issue[0].Severity?.ToString(),
                            Code = expectedOutcome.Issue[0].Code?.ToString()
                        },
                        Actual = new SimplifiedOutcome
                        {
                            Severity = actualOutcome.Issue[0].Severity?.ToString(),
                            Code = actualOutcome.Issue[0].Code?.ToString()
                        }
                    });
                }
            }
        }

        return comparisonResult;
    }
    public class ComparisonResult
    {
        public bool AreEqual { get; set; }
        public List<DifferenceDetail> Differences { get; set; }
    }

    public class DifferenceDetail
    {
        public int TestCaseIndex { get; set; } // Índice do teste que falhou
        public int? OutcomeIndex { get; set; } // Índice do OperationOutcome específico (se aplicável)
        public string? Message { get; set; }    // Mensagem genérica (ex: "Tamanho diferente")
        public SimplifiedOutcome? Expected { get; set; }
        public SimplifiedOutcome? Actual { get; set; }
    }

    public class SimplifiedOutcome
    {
        public string? Severity { get; set; } // Ex: "error", "warning"
        public string? Code { get; set; }     // Ex: "invalid", "not-found"
    }
}
