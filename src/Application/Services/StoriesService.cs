using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class StoriesService(
    IHackerNewsClient client,
    IMemoryCache cache) : IStoriesService
{
    private const string BestStoryIdsCacheKey = "HackerNews:BestStoryIds";
    private static readonly TimeSpan BestStoryIdsCacheDuration = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan StoryByIdCacheDuration = TimeSpan.FromMinutes(2);
    private const int MaxConcurrency = 100;

    public async Task<IEnumerable<StoryResponse>> GetBestStoriesAsync(int n, CancellationToken cancellationToken = default)
    {
        if(n <= 0)
            return [];

        var ids = await GetBestStoryIdsAsync(cancellationToken);
        if (ids.Count == 0)
            return [];

        var candidates = await FetchStoryCandidatesAsync(ids, n, cancellationToken);

        var topStories = candidates
            .OrderByDescending(s => s.Score)
            .Take(n)
            .Select(s => new StoryResponse
            {
                Title = s.Title,
                Uri = s.Url,
                PostedBy = s.By,
                Time = DateTimeOffset.FromUnixTimeSeconds(s.Time),
                Score = s.Score,
                CommentCount = s.Descendants
            })
            .ToList();

        return topStories;
    }

    private async Task<List<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        return await cache.GetOrCreateAsync(BestStoryIdsCacheKey,
                   async entry =>
                   {
                       entry.AbsoluteExpirationRelativeToNow = BestStoryIdsCacheDuration;
                       return await client.GetBestStoryIdsAsync(cancellationToken);
                   }) ?? [];
    }

    private async Task<List<HackerNewsStory>> FetchStoryCandidatesAsync(IReadOnlyList<long> ids, int needed, CancellationToken cancellationToken)
    {
        var result = new List<HackerNewsStory>(capacity: Math.Min(ids.Count, needed));

        using var concurrency = new SemaphoreSlim(MaxConcurrency);
        var tasks = new List<Task<HackerNewsStory?>>(capacity: ids.Count);

        tasks.AddRange(ids.Select(id => GetStoryByIdCachedAsync(id, concurrency, cancellationToken)));

        var stories = await Task.WhenAll(tasks);
        result.AddRange(stories.Where(s => s != null).Select(s => s!));

        return result;
    }

    private async Task<HackerNewsStory?> GetStoryByIdCachedAsync(long id, SemaphoreSlim concurrency, CancellationToken cancellationToken)
    {
        await concurrency.WaitAsync(cancellationToken);
        try
        {
            var cacheKey = $"HackerNews:Story:{id}";
            return await cache.GetOrCreateAsync(cacheKey,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = StoryByIdCacheDuration;
                    return await client.GetStoryByIdAsync(id, cancellationToken);
                });
        }
        finally
        {
            concurrency.Release();
        }
    }
}