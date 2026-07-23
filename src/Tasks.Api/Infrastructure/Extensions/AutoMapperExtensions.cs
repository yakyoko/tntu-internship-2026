namespace Tasks.Api.Infrastructure.Extensions;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddTasksAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
