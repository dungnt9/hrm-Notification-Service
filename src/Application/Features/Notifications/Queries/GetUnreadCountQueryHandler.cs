using MediatR;
using NotificationService.Application.Common.Abstractions.Repositories;

namespace NotificationService.Application.Features.Notifications.Queries;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUnreadCountQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Notifications.GetUnreadCountAsync(request.UserId);
    }
}
