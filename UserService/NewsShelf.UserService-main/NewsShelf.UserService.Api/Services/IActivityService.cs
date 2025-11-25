using NewsShelf.UserService.Api.Contracts.Activity;

namespace NewsShelf.UserService.Api.Services;

public interface IActivityService
{
    Task<ActivityResponse> RecordReadAsync(string userId, RecordReadRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<ActivityResponse>> GetHistoryAsync(string userId, int take, CancellationToken cancellationToken);
    Task<IEnumerable<string>> SetFavoriteTopicsAsync(string userId, IEnumerable<string> topics, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetFavoriteTopicsAsync(string userId, CancellationToken cancellationToken);
}
