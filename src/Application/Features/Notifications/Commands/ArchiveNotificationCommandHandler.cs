using MediatR;
using NotificationService.Application.Common.Abstractions.Repositories;

namespace NotificationService.Application.Features.Notifications.Commands;

public class ArchiveNotificationCommandHandler : IRequestHandler<ArchiveNotificationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ArchiveNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId);
        if (notification == null)
            return false;

        await _unitOfWork.Notifications.ArchiveAsync(request.NotificationId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
