using MediatR;
using NotificationService.Application.Common.Abstractions.Repositories;

namespace NotificationService.Application.Features.Notifications.Commands;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId);
        if (notification == null)
            return false;

        await _unitOfWork.Notifications.MarkAsReadAsync(request.NotificationId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
