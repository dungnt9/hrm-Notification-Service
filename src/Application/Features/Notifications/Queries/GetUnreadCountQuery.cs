using MediatR;

namespace NotificationService.Application.Features.Notifications.Queries;

public record GetUnreadCountQuery(Guid UserId) : IRequest<int>;
