using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var storiesService = new Mock<IStoriesService>(MockBehavior.Strict);
            storiesService
                .Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            services.AddSingleton(storiesService.Object);
        });
    }
}
