using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Abstractions.Repositories;

public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByCodeAsync(string code);
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync();
}
