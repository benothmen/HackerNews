using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.Tests;

public sealed class StoriesControllerTests(
    CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetBestStories_Returns_200_And_Json_Array()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/stories/best?n=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<StoryResponse>>();
        Assert.NotNull(payload);
    }

    [Fact]
    public async Task GetBestStories_Calls_Service_With_N()
    {
        // Arrange
        var storiesServiceMock = new Mock<IStoriesService>(MockBehavior.Strict);
        storiesServiceMock
            .Setup(s => s.GetBestStoriesAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable();

        await using var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(storiesServiceMock.Object);
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/api/stories/best?n=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        storiesServiceMock.Verify();
    }

    [Fact]
    public async Task GetBestStories_Returns_Stories_In_Descending_Score_Order()
    {
        // Arrange
        var storiesServiceMock = new Mock<IStoriesService>(MockBehavior.Strict);
        storiesServiceMock
            .Setup(s => s.GetBestStoriesAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new StoryResponse
                {
                    Title = "t2",
                    Uri = "u2",
                    PostedBy = "b2",
                    Time = DateTimeOffset.Parse("2019-10-12T13:43:01+00:00"),
                    Score = 30,
                    CommentCount = 2
                },
                new StoryResponse
                {
                    Title = "t3",
                    Uri = "u3",
                    PostedBy = "b3",
                    Time = DateTimeOffset.Parse("2019-10-12T13:43:01+00:00"),
                    Score = 20,
                    CommentCount = 3
                },
                new StoryResponse
                {
                    Title = "t1",
                    Uri = "u1",
                    PostedBy = "b1",
                    Time = DateTimeOffset.Parse("2019-10-12T13:43:01+00:00"),
                    Score = 10,
                    CommentCount = 1
                }
            ]);

        await using var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(storiesServiceMock.Object);
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/api/stories/best?n=3");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<StoryResponse>>();
        Assert.NotNull(payload);
        Assert.Equal(3, payload.Count);
        Assert.True(payload[0].Score >= payload[1].Score);
        Assert.True(payload[1].Score >= payload[2].Score);
    }

    [Fact]
    public async Task GetBestStories_Returns_Expected_Response_Contract()
    {
        // Arrange
        var storiesServiceMock = new Mock<IStoriesService>(MockBehavior.Strict);
        storiesServiceMock
            .Setup(s => s.GetBestStoriesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new StoryResponse
                {
                    Title = "title",
                    Uri = "https://example.test",
                    PostedBy = "user",
                    Time = DateTimeOffset.Parse("2019-10-12T13:43:01+00:00"),
                    Score = 1,
                    CommentCount = 0
                }
            ]);

        await using var factory1 = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(storiesServiceMock.Object);
            });
        });

        var client = factory1.CreateClient();

        // Act
        var response = await client.GetAsync("/api/stories/best?n=1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<StoryResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
        var story = payload[0];
        Assert.False(string.IsNullOrWhiteSpace(story.Title));
        Assert.False(string.IsNullOrWhiteSpace(story.Uri));
        Assert.False(string.IsNullOrWhiteSpace(story.PostedBy));
        Assert.True(story.Time > DateTimeOffset.MinValue);
    }
}
