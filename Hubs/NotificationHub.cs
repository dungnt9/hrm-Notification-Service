using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Domain.Entities;

namespace NotificationService.Hubs;

public class NotificationHub : Hub
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(NotificationDbContext context, ILogger<NotificationHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            var connection = new UserConnection
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                ConnectionId = Context.ConnectionId,
                ConnectedAt = DateTime.UtcNow
            };
            _context.UserConnections.Add(connection);
            await _context.SaveChangesAsync();

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connection = await _context.UserConnections
            .FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

        if (connection != null)
        {
            connection.DisconnectedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Connection {ConnectionId} disconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotification(string userId, string title, string message, string type, string? data)
    {
        await Clients.Group(userId).SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = type,
            Data = data,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task MarkAsRead(string notificationId)
    {
        if (Guid.TryParse(notificationId, out var id))
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<List<object>> GetUnreadNotifications()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            return new List<object>();
        }

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userGuid && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                Type = n.Type.ToString(),
                n.Data,
                n.CreatedAt
            })
            .ToListAsync();

        return notifications.Cast<object>().ToList();
    }
}
