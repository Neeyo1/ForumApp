using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class SectionRepository(DataContext context, IMapper mapper) : ISectionRepository
{
    public void AddSection(Section section)
    {
        context.Sections.Add(section);
    }

    public void DeleteSection(Section section)
    {
        context.Sections.Remove(section);
    }

    public async Task<Section?> GetSectionAsync(int sectionId)
    {
        return await context.Sections
            .FirstOrDefaultAsync(x => x.Id == sectionId);
    }

    public async Task<IEnumerable<SectionDto>> GetSectionsAsync()
    {
        return await context.Sections
            .ProjectTo<SectionDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
