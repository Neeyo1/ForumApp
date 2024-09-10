using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ISectionRepository
{
    void AddSection(Section section);
    void DeleteSection(Section section);
    Task<Section?> GetSectionAsync(int sectionId);
    Task<IEnumerable<SectionDto>> GetSectionsAsync();
    Task<bool> Complete();
}
