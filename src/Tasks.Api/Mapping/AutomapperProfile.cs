using AutoMapper;

namespace Tasks.Api.Mapping;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Models.TaskItem, Models.TaskItemDto>().ReverseMap();
    }
}
