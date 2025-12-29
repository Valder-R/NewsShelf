namespace NewsShelf.UserService.Api.Contracts.Admin;

public sealed class AdminUserItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = "READER";
    public string Status { get; set; } = "ACTIVE";
}
