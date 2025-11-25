using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsShelf.UserService.Api.Contracts.Profile;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Services;

public class UserProfileService(UserManager<ApplicationUser> userManager, IActivityService activityService)
    : IUserProfileService
{
    public async Task<UserProfileDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.FavoriteTopics)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var topics = await activityService.GetFavoriteTopicsAsync(userId, cancellationToken);
        return Map(user, topics);
    }

    public async Task<UserProfileDto> UpdateAsync(string userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        user.DisplayName = request.DisplayName;
        user.Bio = request.Bio;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(';', result.Errors.Select(e => e.Description)));
        }

        IEnumerable<string> topics = request.FavoriteTopics is not null
            ? await activityService.SetFavoriteTopicsAsync(userId, request.FavoriteTopics, cancellationToken)
            : await activityService.GetFavoriteTopicsAsync(userId, cancellationToken);

        return Map(user, topics);
    }

    private static UserProfileDto Map(ApplicationUser user, IEnumerable<string> topics) => new()
    {
        Id = user.Id,
        Email = user.Email ?? string.Empty,
        DisplayName = user.DisplayName,
        Bio = user.Bio,
        FavoriteTopics = topics
    };
}
