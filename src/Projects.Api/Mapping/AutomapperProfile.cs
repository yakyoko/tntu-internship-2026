using AutoMapper;

namespace Projects.Api.Mapping;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<Models.Project, Models.ProjectDto>().ReverseMap();
    }
}
