using MediatR;
using NotificationService.Application.Features.Notifications.Dtos;

namespace NotificationService.Application.Features.Notifications.Queries;

public record GetNotificationsQuery(Guid UserId, bool? UnreadOnly = null) : IRequest<IEnumerable<NotificationDto>>;
