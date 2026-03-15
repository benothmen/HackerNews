using Application.Interfaces;
using Infrastructure.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        });

        return services;
    }
}