using FHIRUT.API.Models;
using FHIRUT.API.Services;
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

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTestCase(
        [FromForm] string userId,
        [FromForm] string description,
        IFormFile yamlFile,
        IFormFile? jsonFile = null)
    {
        if (yamlFile == null || yamlFile.Length == 0)
            return BadRequest("YAML file is required");

        var testCase = new TestCaseFileModel
        {
            Name = Path.GetFileNameWithoutExtension(yamlFile.FileName),
            Description = description,
            UserId = userId
        };

        try
        {
            var result = await _testService.SaveTestCaseAsync(
                userId,
                testCase,
                yamlFile.OpenReadStream(),
                jsonFile?.OpenReadStream());

            return CreatedAtAction(nameof(GetTestCase), new { userId, caseId = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading test case");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}/{caseId}")]
    public async Task<IActionResult> GetTestCase(string userId, string caseId)
    {
        // Implementar lógica de busca
        return Ok();
    }

    [HttpPost("{userId}/{caseId}/run")]
    public async Task<IActionResult> RunTestCase(string userId, string caseId)
    {
        try
        {
            var result = await _testService.RunTestCaseAsync(userId, caseId);
            return Ok(result);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}