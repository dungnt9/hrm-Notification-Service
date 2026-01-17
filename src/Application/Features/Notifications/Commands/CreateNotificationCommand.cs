using MediatR;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Notifications.Commands;

public class CreateNotificationCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string? Data { get; set; }
    public string? ActionUrl { get; set; }
}
