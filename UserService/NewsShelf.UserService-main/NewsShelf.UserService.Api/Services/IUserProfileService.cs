using NewsShelf.UserService.Api.Contracts.Profile;

namespace NewsShelf.UserService.Api.Services;

public interface IUserProfileService
{
    Task<UserProfileDto> GetAsync(string userId, CancellationToken cancellationToken);
    Task<UserProfileDto> UpdateAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken);
}
