namespace NotificationService.Application.Common.Abstractions.Repositories;

public interface IUnitOfWork : IDisposable
{
    INotificationRepository Notifications { get; }
    IUserConnectionRepository UserConnections { get; }
    INotificationTemplateRepository NotificationTemplates { get; }
    INotificationPreferenceRepository NotificationPreferences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
