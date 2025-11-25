using System.ComponentModel.DataAnnotations;

namespace NewsShelf.UserService.Api.Contracts.Auth;

public class ExternalLoginRequest
{
    [Required]
    public string Provider { get; set; } = string.Empty;

    [Required]
    public string ExternalToken { get; set; } = string.Empty;
}
