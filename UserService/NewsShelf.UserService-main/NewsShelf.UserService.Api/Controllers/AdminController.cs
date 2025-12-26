using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsShelf.UserService.Api.Contracts.Admin;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Authorize(Roles = "ADMIN")]
[Route("api/admin")]
public class AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpDelete("users/{userId}")]
    public async Task<ActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        return NoContent();
    }

    [HttpPost("users/{userId}/roles")]
    public async Task<ActionResult> UpdateUserRole(string userId, UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var roleExists = await roleManager.RoleExistsAsync(request.Role);
        if (!roleExists)
        {
            return BadRequest($"Role '{request.Role}' does not exist");
        }

        IdentityResult result;
        if (request.Add)
        {
            var userHasRole = await userManager.IsInRoleAsync(user, request.Role);
            if (userHasRole)
            {
                return BadRequest($"User already has role '{request.Role}'");
            }

            result = await userManager.AddToRoleAsync(user, request.Role);
        }
        else
        {
            var userHasRole = await userManager.IsInRoleAsync(user, request.Role);
            if (!userHasRole)
            {
                return BadRequest($"User does not have role '{request.Role}'");
            }

            result = await userManager.RemoveFromRoleAsync(user, request.Role);
        }

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        var action = request.Add ? "added to" : "removed from";
        return Ok(new { message = $"Role '{request.Role}' {action} user successfully" });
    }
}

