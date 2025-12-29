using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsShelf.UserService.Api.Contracts.Profile;
using NewsShelf.UserService.Api.Services;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Authorize]
[Route("profiles")]
public class ProfileController(IUserProfileService profileService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> Me(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var profile = await profileService.GetAsync(userId, cancellationToken);

        if (profile is null) return NotFound();
        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> Update([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.FavoriteTopics == null || !request.FavoriteTopics.Any())
        {
            return BadRequest(new { message = "FavoriteTopics is required" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var profile = await profileService.UpdateAsync(userId, request, cancellationToken);
        return Ok(profile);
    }
}
