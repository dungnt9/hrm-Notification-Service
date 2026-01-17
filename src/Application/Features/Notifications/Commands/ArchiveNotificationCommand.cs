using MediatR;

namespace NotificationService.Application.Features.Notifications.Commands;

public record ArchiveNotificationCommand(Guid NotificationId) : IRequest<bool>;
