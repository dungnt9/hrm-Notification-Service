using MediatR;
using NotificationService.Application.Common.Abstractions.Repositories;
using NotificationService.Application.Features.Notifications.Dtos;

namespace NotificationService.Application.Features.Notifications.Queries;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = request.UnreadOnly == true
            ? await _unitOfWork.Notifications.GetUnreadByUserIdAsync(request.UserId)
            : await _unitOfWork.Notifications.GetByUserIdAsync(request.UserId);

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            Priority = n.Priority.ToString(),
            Data = n.Data,
            ActionUrl = n.ActionUrl,
            IsRead = n.IsRead,
            IsArchived = n.IsArchived,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt
        }).OrderByDescending(n => n.CreatedAt);
    }
}
