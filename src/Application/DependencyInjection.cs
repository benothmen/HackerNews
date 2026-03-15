using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;
using Application.Services;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IStoriesService, StoriesService>();

        return services;
    }
}