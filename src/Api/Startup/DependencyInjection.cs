using Application;
using Infrastructure;

namespace Api.Startup;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();

        services.AddControllers();
        services.AddOpenApi("v1");

        return services;
    }
}
