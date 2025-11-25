using NewsShelf.UserService.Api.Contracts.Auth;

namespace NewsShelf.UserService.Api.Services;

public interface IExternalOAuthService
{
    Task<AuthResponse> SignInAsync(ExternalLoginRequest request, CancellationToken cancellationToken);
}
