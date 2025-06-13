using System.Text;
using FHIRUT.API.Models.Result;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Hl7.Fhir.Model;
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
    public async Task<IActionResult> RunTestCases([FromBody] List<TestCaseDefinition> testCaseRequests)
    {
        if (testCaseRequests == null || !testCaseRequests.Any())
            return BadRequest("Informe algum test case.");

        try
        {
            var result = await _testService.RunTestCaseAsync(testCaseRequests);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running batch validation");
            return StatusCode(500, "Internal server error");
        }
    }

}
