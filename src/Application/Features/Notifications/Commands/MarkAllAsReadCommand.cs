using MediatR;

namespace NotificationService.Application.Features.Notifications.Commands;

public record MarkAllAsReadCommand(Guid UserId) : IRequest<bool>;
