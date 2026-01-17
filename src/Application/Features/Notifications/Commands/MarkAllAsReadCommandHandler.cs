using MediatR;
using NotificationService.Application.Common.Abstractions.Repositories;

namespace NotificationService.Application.Features.Notifications.Commands;

public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllAsReadCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
