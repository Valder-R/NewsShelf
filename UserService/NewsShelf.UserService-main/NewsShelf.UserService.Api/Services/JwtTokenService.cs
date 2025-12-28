using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NewsShelf.UserService.Api.Contracts.Auth;
using NewsShelf.UserService.Api.Models;
using NewsShelf.UserService.Api.Options;

namespace NewsShelf.UserService.Api.Services;

public class JwtTokenService(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<AuthResponse> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.DisplayName ?? user.Email ?? user.Id)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: creds);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);

        return new AuthResponse
        {
            AccessToken = jwt,
            ExpiresAt = expires
        };
    }
}
