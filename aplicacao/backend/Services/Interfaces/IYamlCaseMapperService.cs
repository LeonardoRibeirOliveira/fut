using FHIRUT.API.Models.Input;
using FHIRUT.API.Models.Tests;

namespace FHIRUT.API.Services.Interfaces
{
    public interface IYamlCaseMapperService
    {
        Task<YamlTestCaseDefinition> LoadTestCaseAsync(string yamlFilePath);
    }
}
