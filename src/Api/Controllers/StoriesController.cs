using Application.Interfaces;
using Application.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/stories")]
public class StoriesController(IStoriesService storiesService) : ControllerBase
{
    [HttpGet("best")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<StoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [EndpointDescription("Returns an array of the best n stories from the Hacker News API, ordered by score descending.")]
    public async Task<IActionResult> GetBestStories([FromQuery, Required, Range(1, 500)] int? n, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || n is null)
            return ValidationProblem(ModelState);

        return Ok(await storiesService.GetBestStoriesAsync(n.Value, cancellationToken));
    }
}