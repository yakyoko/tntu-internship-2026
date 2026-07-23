namespace Projects.Api.Infrastructure.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddProjectsAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
