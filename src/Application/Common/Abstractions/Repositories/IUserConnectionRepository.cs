using NotificationService.Domain.Entities;

namespace NotificationService.Application.Common.Abstractions.Repositories;

public interface IUserConnectionRepository : IRepository<UserConnection>
{
    Task<IEnumerable<UserConnection>> GetActiveConnectionsByUserIdAsync(Guid userId);
    Task<UserConnection?> GetByConnectionIdAsync(string connectionId);
    Task DisconnectAsync(string connectionId);
    Task UpdateLastActivityAsync(string connectionId);
}
