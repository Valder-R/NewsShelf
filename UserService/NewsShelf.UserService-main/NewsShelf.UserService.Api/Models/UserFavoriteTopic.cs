namespace NewsShelf.UserService.Api.Models;

public class UserFavoriteTopic
{
    public Guid Id { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
