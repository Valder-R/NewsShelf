using Microsoft.EntityFrameworkCore;
using NewsShelf.UserService.Api.Contracts.Activity;
using NewsShelf.UserService.Api.Data;
using NewsShelf.UserService.Api.Models;

namespace NewsShelf.UserService.Api.Services;

public class ActivityService(ApplicationDbContext dbContext) : IActivityService
{
    public async Task<ActivityResponse> RecordReadAsync(string userId, RecordReadRequest request, CancellationToken cancellationToken)
    {
        var entity = new UserActivity
        {
            UserId = userId,
            NewsId = request.NewsId,
            NewsTitle = request.NewsTitle,
            Topic = request.Topic,
            ActivityType = ActivityType.Read,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await dbContext.UserActivities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ActivityResponse
        {
            Id = entity.Id,
            NewsId = entity.NewsId,
            NewsTitle = entity.NewsTitle,
            Topic = entity.Topic,
            ActivityType = entity.ActivityType,
            OccurredAt = entity.OccurredAt
        };
    }

    public async Task<IEnumerable<ActivityResponse>> GetHistoryAsync(string userId, int take, CancellationToken cancellationToken)
    {
        return await dbContext.UserActivities
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.OccurredAt)
            .Take(take)
            .Select(a => new ActivityResponse
            {
                Id = a.Id,
                NewsId = a.NewsId,
                NewsTitle = a.NewsTitle,
                Topic = a.Topic,
                ActivityType = a.ActivityType,
                OccurredAt = a.OccurredAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> SetFavoriteTopicsAsync(string userId, IEnumerable<string> topics, CancellationToken cancellationToken)
    {
        var distinctTopics = topics
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(20)
            .ToList();

        var existing = await dbContext.UserFavoriteTopics
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.UserFavoriteTopics.RemoveRange(existing.Where(t => distinctTopics.All(dt => !dt.Equals(t.Topic, StringComparison.OrdinalIgnoreCase))));

        foreach (var topic in distinctTopics)
        {
            if (existing.All(t => !t.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase)))
            {
                dbContext.UserFavoriteTopics.Add(new UserFavoriteTopic
                {
                    UserId = userId,
                    Topic = topic
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetFavoriteTopicsAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetFavoriteTopicsAsync(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserFavoriteTopics
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => t.Topic)
            .OrderBy(t => t)
            .ToListAsync(cancellationToken);
    }
}
