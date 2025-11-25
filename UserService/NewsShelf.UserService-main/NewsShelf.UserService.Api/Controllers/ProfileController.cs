using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsShelf.UserService.Api.Contracts.Profile;
using NewsShelf.UserService.Api.Services;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/profiles")]
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
        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> Update(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var profile = await profileService.UpdateAsync(userId, request, cancellationToken);
        return Ok(profile);
    }
}
