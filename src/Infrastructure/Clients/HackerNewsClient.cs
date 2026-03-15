using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients;

public class HackerNewsClient(HttpClient httpClient, ILogger<HackerNewsClient> logger) : IHackerNewsClient
{
    private const string BestStoriesPath = "beststories.json";
    private const string ItemByIdPathFormat = "item/{0}.json";

    public async Task<List<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<List<long>>(BestStoriesPath, cancellationToken) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch best story IDs from '{Path}'.", BestStoriesPath);
            throw;
        }
    }

    public async Task<HackerNewsStory?> GetStoryByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var path = string.Format(ItemByIdPathFormat, id);
        HackerNewsItem? item;
        try
        {
            item = await httpClient.GetFromJsonAsync<HackerNewsItem>(path, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch story {Id} from '{Path}'.", id, path);
            throw;
        }

        if (item != null)
            return new HackerNewsStory
            {
                Title = item.Title,
                Url = item.Url,
                By = item.By,
                Time = item.Time,
                Score = item.Score,
                Descendants = item.Descendants
            };
        logger.LogWarning("Story {Id} returned null from Hacker News API.", id);
        return null;

    }

}