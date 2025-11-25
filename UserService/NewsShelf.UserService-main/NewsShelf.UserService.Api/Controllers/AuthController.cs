using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsShelf.UserService.Api.Contracts.Auth;
using NewsShelf.UserService.Api.Models;
using NewsShelf.UserService.Api.Services;

namespace NewsShelf.UserService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IExternalOAuthService externalOAuthService,
    IActivityService activityService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            Bio = request.Bio
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        if (request.FavoriteTopics is not null)
        {
            await activityService.SetFavoriteTopicsAsync(user.Id, request.FavoriteTopics, cancellationToken);
        }

        var response = tokenService.GenerateAccessToken(user);
        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized("Invalid credentials");
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Unauthorized("Invalid credentials");
        }

        return tokenService.GenerateAccessToken(user);
    }

    [HttpPost("external")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> External(ExternalLoginRequest request, CancellationToken cancellationToken)
    {
        var response = await externalOAuthService.SignInAsync(request, cancellationToken);
        return Ok(response);
    }
}
