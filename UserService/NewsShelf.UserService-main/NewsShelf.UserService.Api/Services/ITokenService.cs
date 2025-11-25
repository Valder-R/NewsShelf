using NewsShelf.UserService.Api.Contracts.Auth;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Services;

public interface ITokenService
{
    AuthResponse GenerateAccessToken(ApplicationUser user);
}
