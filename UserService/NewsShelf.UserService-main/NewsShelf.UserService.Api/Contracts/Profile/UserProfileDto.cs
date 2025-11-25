namespace NewsShelf.UserService.Api.Contracts.Profile;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public IEnumerable<string> FavoriteTopics { get; set; } = Enumerable.Empty<string>();
}
