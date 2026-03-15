using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Application.Tests;

public sealed class StoriesServiceTests
{
    [Fact]
    public async Task GetBestStoriesAsync_Returns_TopN_By_Score()
    {
        // Arrange
        var ids = new List<long> { 1, 2, 3 };
        var stories = new Dictionary<long, HackerNewsStory?>
        {
            [1] = new() { Title = "t1", Url = "u1", By = "b1", Time = 1, Score = 10, Descendants = 1 },
            [2] = new() { Title = "t2", Url = "u2", By = "b2", Time = 1, Score = 30, Descendants = 2 },
            [3] = new() { Title = "t3", Url = "u3", By = "b3", Time = 1, Score = 20, Descendants = 3 },
        };

        var client = new Mock<IHackerNewsClient>(MockBehavior.Strict);
        client.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ids);
        client.Setup(c => c.GetStoryByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(stories[1]);
        client.Setup(c => c.GetStoryByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(stories[2]);
        client.Setup(c => c.GetStoryByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(stories[3]);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new StoriesService(client.Object, cache);

        // Act
        var result = (await sut.GetBestStoriesAsync(2, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("t2", result[0].Title);
        Assert.Equal("t3", result[1].Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetBestStoriesAsync_WhenNIsInvalid_Returns_Empty(int n)
    {
        // Arrange
        var client = new Mock<IHackerNewsClient>(MockBehavior.Strict);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sut = new StoriesService(client.Object, cache);

        // Act
        var result = await sut.GetBestStoriesAsync(n, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
