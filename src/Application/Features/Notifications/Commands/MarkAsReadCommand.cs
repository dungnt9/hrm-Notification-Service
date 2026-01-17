using MediatR;

namespace NotificationService.Application.Features.Notifications.Commands;

public record MarkAsReadCommand(Guid NotificationId) : IRequest<bool>;
