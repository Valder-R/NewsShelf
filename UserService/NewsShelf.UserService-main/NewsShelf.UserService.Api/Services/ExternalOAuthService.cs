using Microsoft.AspNetCore.Identity;
using NewsShelf.UserService.Api.Contracts.Auth;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Services;

public class ExternalOAuthService(
    UserManager<ApplicationUser> userManager,
    IExternalTokenProvider tokenProvider,
    ITokenService tokenService) : IExternalOAuthService
{
    public async Task<AuthResponse> SignInAsync(ExternalLoginRequest request, CancellationToken cancellationToken)
    {
        var payload = await tokenProvider.ValidateTokenAsync(request.Provider, request.ExternalToken, cancellationToken);
        
        if (payload == null)
        {
            throw new InvalidOperationException("External token validation failed");
        }
        
        var provider = request.Provider.ToLowerInvariant();
        var user = await userManager.FindByLoginAsync(provider, request.ExternalToken);
        
        if (user is null)
        {
            user = await userManager.FindByEmailAsync(payload.Email);
        }
        
        if (user is null)
        {
            var email = payload.Email;
            user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                DisplayName = payload.Name
            };
        
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(';', createResult.Errors.Select(e => e.Description)));
            }
        }
        
        var logins = await userManager.GetLoginsAsync(user);
        if (logins.All(l => l.ProviderKey != request.ExternalToken))
        {
            var addLoginResult = await userManager.AddLoginAsync(user, new UserLoginInfo(provider, request.ExternalToken, provider));
            if (!addLoginResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(';', addLoginResult.Errors.Select(e => e.Description)));
            }
        }
        
        return tokenService.GenerateAccessToken(user);
    }
}
