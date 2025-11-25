using System.ComponentModel.DataAnnotations;

namespace NewsShelf.UserService.Api.Contracts.Activity;

public class RecordReadRequest
{
    [Required]
    public string NewsId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? NewsTitle { get; set; }

    [MaxLength(64)]
    public string? Topic { get; set; }
}
