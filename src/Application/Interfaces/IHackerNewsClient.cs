using Application.DTOs;

namespace Application.Interfaces;

public interface IHackerNewsClient
{
    Task<List<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);
    Task<HackerNewsStory?> GetStoryByIdAsync(long id, CancellationToken cancellationToken = default);
}