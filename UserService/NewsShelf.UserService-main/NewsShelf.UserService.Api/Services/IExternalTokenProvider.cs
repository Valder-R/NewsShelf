using Google.Apis.Auth;

namespace NewsShelf.UserService.Api.Services;

public interface IExternalTokenProvider
{
    Task<GoogleJsonWebSignature.Payload?> ValidateTokenAsync(string provider, string token, CancellationToken cancellationToken);
}
