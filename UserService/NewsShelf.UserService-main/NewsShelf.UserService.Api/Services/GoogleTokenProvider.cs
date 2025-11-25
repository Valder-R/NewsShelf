using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;

namespace NewsShelf.UserService.Api.Services;

public class GoogleTokenProvider : IExternalTokenProvider
{
    private readonly GoogleOptions _googleOptions;
    
    public GoogleTokenProvider(IOptionsMonitor<GoogleOptions> googleOptionsMonitor)
    {
        _googleOptions = googleOptionsMonitor.Get(GoogleDefaults.AuthenticationScheme);
    }
    
    public Task<GoogleJsonWebSignature.Payload?> ValidateTokenAsync(string provider, string token, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_googleOptions.ClientId]
            };
            return GoogleJsonWebSignature.ValidateAsync(token, settings);
        }
        catch (Exception)
        {
            return Task.FromResult<GoogleJsonWebSignature.Payload?>(null);
        }
    }
}
