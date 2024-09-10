using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SectionCreateDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Description { get; set; }
}
