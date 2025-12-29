using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsShelf.UserService.Api.Contracts.Admin;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Route("admin")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    private static readonly string[] AllowedRoles = ["ADMIN", "PUBLISHER", "READER"];
    private static readonly string[] AllowedStatuses = ["ACTIVE", "BLOCKED"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly string _mainAdminEmail;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;

        _mainAdminEmail = configuration["Admin:Email"]
            ?? throw new InvalidOperationException("Admin:Email is not configured");
    }


    private bool IsMainAdmin(ApplicationUser user)
        => user.Email != null &&
           user.Email.Equals(_mainAdminEmail, StringComparison.OrdinalIgnoreCase);

    private IActionResult ForbidMainAdminAction()
        => BadRequest(new { message = "Main admin cannot be modified" });



    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? role = null,
        [FromQuery] string? status = null
    )
    {
        List<ApplicationUser> users;

        if (!string.IsNullOrWhiteSpace(role))
            users = (await _userManager
                .GetUsersInRoleAsync(role.Trim().ToUpperInvariant()))
                .ToList();
        else
            users = _userManager.Users.ToList();

        var result = new List<AdminUserItemResponse>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var primaryRole = roles.FirstOrDefault() ?? "READER";

            result.Add(new AdminUserItemResponse
            {
                Id = u.Id,
                Email = u.Email,
                Role = primaryRole,
                Status = "ACTIVE"
            });
        }

        return Ok(new { users = result, total = result.Count });
    }


    [HttpPut("users/{userId}/role")]
    [HttpPatch("users/{userId}/role")]
    public async Task<IActionResult> SetRole(
        string userId,
        [FromBody] SetUserRoleRequest request
    )
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(new { message = "User not found" });

        if (IsMainAdmin(user))
            return ForbidMainAdminAction();

        var newRole = (request?.Role ?? "READER")
            .Trim()
            .ToUpperInvariant();

        if (!AllowedRoles.Contains(newRole))
            return BadRequest(new { message = "Invalid role" });

        if ((newRole == "READER" || newRole == "PUBLISHER") &&
            (user.FavoriteTopics == null || !user.FavoriteTopics.Any()))
        {
            return BadRequest(new
            {
                message = "FavoriteTopics must be set for READER / PUBLISHER"
            });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRes.Succeeded)
                return BadRequest(removeRes.Errors);
        }

        var addRes = await _userManager.AddToRoleAsync(user, newRole);
        if (!addRes.Succeeded)
            return BadRequest(addRes.Errors);

        return Ok(new { ok = true, role = newRole });
    }


    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound(new { message = "User not found" });

        if (IsMainAdmin(user))
            return ForbidMainAdminAction();

        var res = await _userManager.DeleteAsync(user);
        if (!res.Succeeded)
            return BadRequest(res.Errors);

        return Ok(new { ok = true });
    }
}
