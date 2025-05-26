using Hl7.Fhir.Model;
using FHIRUT.API.Models.Tests;
using FHIRUT.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.AssemblyIdentityComparer;
using System.ComponentModel.DataAnnotations;
namespace FHIRUT.API.Models.Tests
{
    public class RunAndCompareTestCasesRequest
    {
        [Required]
        public List<TestCaseDefinition> TestCaseRequests { get; set; } = new List<TestCaseDefinition>();

        [Required]
        public List<List<OperationOutcome>> ExpectedOutcomes { get; set; } = new List<List<OperationOutcome>>();
    }
}