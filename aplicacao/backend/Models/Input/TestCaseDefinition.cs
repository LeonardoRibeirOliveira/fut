﻿using System.ComponentModel.DataAnnotations;

namespace FHIRUT.API.Models.Tests
{
    public class TestCaseDefinition
    {
        [Required]
        public string YamlFile { get; set; } = string.Empty;
    }
}