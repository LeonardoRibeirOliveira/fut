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

    [HttpPost("upload/batch")]
    public async Task<IActionResult> UploadTestCasesBatch(
        [FromForm] string userId,
        [FromForm] string description,
        List<IFormFile> yamlFiles,
        List<IFormFile>? jsonFiles = null)
    {
        if (yamlFiles == null || !yamlFiles.Any())
            return BadRequest("At least one YAML file is required");

        var results = new List<TestCaseFileModel>();

        try
        {
            for (int i = 0; i < yamlFiles.Count; i++)
            {
                var yamlFile = yamlFiles[i];
                var jsonFile = jsonFiles?.Count > i ? jsonFiles[i] : null;

                if (yamlFile.Length == 0)
                    continue;

                var testCase = new TestCaseFileModel
                {
                    Name = Path.GetFileNameWithoutExtension(yamlFile.FileName),
                    Description = $"{description} [{i + 1}/{yamlFiles.Count}]",
                    UserId = userId
                };

                var result = await _testService.SaveTestCaseAsync(
                    userId,
                    testCase,
                    yamlFile.OpenReadStream(),
                    jsonFile?.OpenReadStream());

                if (result != null) results.Add(result);
            }

            return CreatedAtAction(nameof(GetTestCasesBatch), new { userId }, results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading batch test cases");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetTestCasesBatch(string userId)
    {
        try
        {
            var testCases = await _testService.GetTestCasesAsync(userId);
            return Ok(testCases);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test cases");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}/{caseId}")]
    public async Task<IActionResult> GetTestCase(string userId, string caseId)
    {
        try
        {
            var testCase = await _testService.GetTestCaseAsync(userId, caseId);
            if (testCase == null)
                return NotFound();

            return Ok(testCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving test case");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("batch/run")]
    public async Task<IActionResult> RunTestCasesBatch(
    [FromBody] BatchRunRequest request)
    {
        if (request?.TestCaseIds == null || !request.TestCaseIds.Any())
            return BadRequest("At least one test case ID is required");

        try
        {
            var results = new List<ValidationResultModel>();

            foreach (var caseId in request.TestCaseIds)
            {
                var result = await _testService.RunTestCaseAsync(request.UserId, caseId);
                if(result != null) results.Add(result);
            }

            return Ok(new BatchRunResponse
            {
                UserId = request.UserId,
                Results = results,
                TotalCount = results.Count,
                SuccessCount = results.Count(r => r.Status == "success"),
                ErrorCount = results.Count(r => r.Status == "error")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running batch validation");
            return StatusCode(500, "Internal server error");
        }
    }
}