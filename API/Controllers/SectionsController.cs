using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class SectionsController(ISectionRepository sectionRepository, IMapper mapper) : BaseApiController
{
    [HttpGet("{sectionId}")]
    public async Task<ActionResult<SectionDto>> GetSection(int sectionId)
    {
        var section = await sectionRepository.GetSectionAsync(sectionId);
        if (section == null) return BadRequest("Section does not exist");

        return Ok(mapper.Map<SectionDto>(section));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SectionDto>>> GetSections()
    {
        var sections = await sectionRepository.GetSectionsAsync();
        return Ok(sections);
    }

    [Authorize(Policy = "RequireModeratorRole")]
    [HttpPost]
    public async Task<ActionResult<SectionDto>> CreateSection(SectionCreateDto sectionCreateDto)
    {
        var section = mapper.Map<Section>(sectionCreateDto);

        sectionRepository.AddSection(section);
        if (await sectionRepository.Complete()) return Ok(mapper.Map<SectionDto>(section));
        return BadRequest("Failed to create section");
    }

    [Authorize(Policy = "RequireModeratorRole")]
    [HttpPut("{sectionId}")]
    public async Task<ActionResult<SectionDto>> EditSection(SectionCreateDto sectionCreateDto, int sectionId)
    {
        var section = await sectionRepository.GetSectionAsync(sectionId);
        if (section == null) return BadRequest("Section does not exist");

        mapper.Map(sectionCreateDto, section);

        if (await sectionRepository.Complete()) return NoContent();
        return BadRequest("Failed to edit section");
    }

    [Authorize(Policy = "RequireModeratorRole")]
    [HttpDelete("{sectionId}")]
    public async Task<ActionResult<SectionDto>> DeleteSection(int sectionId)
    {
        var section = await sectionRepository.GetSectionAsync(sectionId);
        if (section == null) return BadRequest("Section does not exist");

        sectionRepository.DeleteSection(section);

        if (await sectionRepository.Complete()) return NoContent();
        return BadRequest("Failed to delete section");
    }
}
