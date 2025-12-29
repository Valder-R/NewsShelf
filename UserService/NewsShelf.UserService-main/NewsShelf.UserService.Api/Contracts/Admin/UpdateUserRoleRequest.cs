namespace NewsShelf.UserService.Api.Contracts.Admin;

public class UpdateUserRoleRequest
{
    public string Role { get; set; } = string.Empty;
    public bool Add { get; set; } = true; 
}

