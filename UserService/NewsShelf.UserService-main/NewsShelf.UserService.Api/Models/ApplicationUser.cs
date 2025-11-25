using Microsoft.AspNetCore.Identity;

namespace NewsShelf.UserService.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public ICollection<UserFavoriteTopic> FavoriteTopics { get; set; } = new List<UserFavoriteTopic>();
    public ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
}
