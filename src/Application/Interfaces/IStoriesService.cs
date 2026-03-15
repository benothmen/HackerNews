using Application.DTOs;

namespace Application.Interfaces;

public interface IStoriesService
{
    Task<IEnumerable<StoryResponse>> GetBestStoriesAsync(int n, CancellationToken cancellationToken = default);
}