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

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTestCases(
        [FromForm] string userId,
        [FromForm] string description,
        List<IFormFile> yamlFiles,
        List<IFormFile>? jsonFiles = null)
    {
        if (yamlFiles == null || !yamlFiles.Any())
            return BadRequest("At least one YAML file is required");

        var results = new List<TestCaseDefinition>();

        try
        {
            for (int i = 0; i < yamlFiles.Count; i++)
            {
                var yamlFile = yamlFiles[i];
                var jsonFile = jsonFiles?.Count > i ? jsonFiles[i] : null;

                if (yamlFile.Length == 0)
                    continue;

                // Desserializa o YAML para TestCaseDefinition
                TestCaseDefinition? testCase;
                using (var reader = new StreamReader(yamlFile.OpenReadStream()))
                {
                    var yamlContent = await reader.ReadToEndAsync();
                    testCase = new YamlDotNet.Serialization.DeserializerBuilder()
                        .IgnoreUnmatchedProperties()
                        .Build()
                        .Deserialize<TestCaseDefinition>(yamlContent);
                }

                if (testCase == null)
                    continue;

                // Preenche campos adicionais se necessário
                testCase.Description ??= $"{description} [{i + 1}/{yamlFiles.Count}]";

                var result = await _testService.SaveTestCaseAsync(
                    userId,
                    testCase,
                    yamlFile.OpenReadStream(),
                    jsonFile?.OpenReadStream());

                if (result != null) results.Add(result);
            }

            return CreatedAtAction(nameof(GetTestCases), new { userId }, results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading batch test cases");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetTestCases(string userId)
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

    [HttpPost("run/{userId}")]
    public async Task<IActionResult> RunTestCases(string userId,
        [FromBody] List<string> TestCaseIds)
    {
        if (TestCaseIds == null || !TestCaseIds.Any())
            return BadRequest("Informe algum testcase.");

        try
        {
            var results = new List<OperationOutcome>();

            foreach (var caseId in TestCaseIds)
            {
                var result = await _testService.RunTestCaseAsync(userId, caseId);
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
