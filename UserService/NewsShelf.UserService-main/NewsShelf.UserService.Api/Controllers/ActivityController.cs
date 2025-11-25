using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsShelf.UserService.Api.Contracts.Activity;
using NewsShelf.UserService.Api.Services;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/activities")]
public class ActivityController(IActivityService activityService) : ControllerBase
{
    [HttpPost("read")]
    public async Task<ActionResult<ActivityResponse>> RecordRead(RecordReadRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var activity = await activityService.RecordReadAsync(userId, request, cancellationToken);
        return Ok(activity);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<ActivityResponse>>> History([FromQuery] int take = 50, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        take = Math.Clamp(take, 1, 200);
        var items = await activityService.GetHistoryAsync(userId, take, cancellationToken);
        return Ok(items);
    }

    [HttpPost("favorite-topics")]
    public async Task<ActionResult<IEnumerable<string>>> SetFavoriteTopics(SetFavoriteTopicsRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var topics = await activityService.SetFavoriteTopicsAsync(userId, request.Topics, cancellationToken);
        return Ok(topics);
    }

    [HttpGet("favorite-topics")]
    public async Task<ActionResult<IEnumerable<string>>> GetFavoriteTopics(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var topics = await activityService.GetFavoriteTopicsAsync(userId, cancellationToken);
        return Ok(topics);
    }
}
