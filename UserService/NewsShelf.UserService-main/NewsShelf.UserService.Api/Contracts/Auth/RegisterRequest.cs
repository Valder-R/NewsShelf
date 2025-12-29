using System.ComponentModel.DataAnnotations;

namespace NewsShelf.UserService.Api.Contracts.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? AccountType { get; set; }

    [Required, MaxLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public IEnumerable<string>? FavoriteTopics { get; set; }
}
