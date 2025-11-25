using System.ComponentModel.DataAnnotations;

namespace NewsShelf.UserService.Api.Contracts.Profile;

public class UpdateProfileRequest
{
    [Required, MaxLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string? Bio { get; set; }

    public IEnumerable<string>? FavoriteTopics { get; set; }
}
