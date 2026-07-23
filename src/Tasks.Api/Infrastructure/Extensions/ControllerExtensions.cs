namespace Tasks.Api.Infrastructure.Extensions;

using System.Text.Json.Serialization;

public static class ControllerExtensions
{
    public static IMvcBuilder AddTasksControllers(this IServiceCollection services)
    {
        return services
            .AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}
