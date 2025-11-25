namespace NewsShelf.UserService.Api.Contracts.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string? RefreshToken { get; set; }
}
