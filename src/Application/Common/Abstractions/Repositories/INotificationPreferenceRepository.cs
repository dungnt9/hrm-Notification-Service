using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Abstractions.Repositories;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    Task<NotificationPreference?> GetByUserAndTypeAsync(Guid userId, NotificationType type);
    Task<IEnumerable<NotificationPreference>> GetByUserIdAsync(Guid userId);
}
