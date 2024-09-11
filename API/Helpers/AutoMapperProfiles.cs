using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, AppUser>();
        CreateMap<AppUser, UserDto>();
        CreateMap<Section, SectionDto>();
        CreateMap<SectionCreateDto, Section>();
        CreateMap<Topic, TopicDto>();
        CreateMap<TopicCreateDto, Topic>();
        CreateMap<Comment, CommentDto>();
        CreateMap<CommentCreateDto, Comment>();
    }
}
