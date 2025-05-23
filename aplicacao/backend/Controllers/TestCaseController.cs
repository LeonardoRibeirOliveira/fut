using FHIRUT.API.Models.Outcome;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var results = new List<OperationOutcome>();

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
}
