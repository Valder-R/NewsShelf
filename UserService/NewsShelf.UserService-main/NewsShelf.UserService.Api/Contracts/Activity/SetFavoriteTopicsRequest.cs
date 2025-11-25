using System.ComponentModel.DataAnnotations;

namespace NewsShelf.UserService.Api.Contracts.Activity;

public class SetFavoriteTopicsRequest
{
    [Required]
    public IEnumerable<string> Topics { get; set; } = Enumerable.Empty<string>();
}
