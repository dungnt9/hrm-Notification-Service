using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Domain.Entities;
using NotificationService.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NotificationService.BackgroundServices;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private IModel? _channel;

    public RabbitMqConsumer(
        IServiceProvider serviceProvider,
        IConnection connection,
        IHubContext<NotificationHub> hubContext,
        ILogger<RabbitMqConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("notification.queue", durable: true, exclusive: false, autoDelete: false);
        
        _channel.QueueBind("notification.queue", "hrm.events", "leave_request_created");
        _channel.QueueBind("notification.queue", "hrm.events", "leave_request_approved");
        _channel.QueueBind("notification.queue", "hrm.events", "leave_request_rejected");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            try
            {
                await ProcessMessage(ea.RoutingKey, message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {RoutingKey}", ea.RoutingKey);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume("notification.queue", false, consumer);

        _logger.LogInformation("RabbitMQ consumer started");

        return Task.CompletedTask;
    }

    private async Task ProcessMessage(string eventType, string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

        var eventData = JsonSerializer.Deserialize<JsonElement>(message);
        var payload = eventData.GetProperty("Payload");

        switch (eventType)
        {
            case "leave_request_created":
                await HandleLeaveRequestCreated(context, payload);
                break;
            case "leave_request_approved":
                await HandleLeaveRequestApproved(context, payload);
                break;
            case "leave_request_rejected":
                await HandleLeaveRequestRejected(context, payload);
                break;
        }
    }

    private async Task HandleLeaveRequestCreated(NotificationDbContext context, JsonElement payload)
    {
        var approverId = payload.GetProperty("ApproverId").GetString();
        var employeeId = payload.GetProperty("EmployeeId").GetString();
        var leaveType = payload.GetProperty("LeaveType").GetString();
        var startDate = payload.GetProperty("StartDate").GetDateTime();
        var endDate = payload.GetProperty("EndDate").GetDateTime();
        var totalDays = payload.GetProperty("TotalDays").GetInt32();

        if (!Guid.TryParse(approverId, out var approverGuid))
        {
            return;
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = approverGuid,
            Title = "New Leave Request",
            Message = $"A new {leaveType} leave request for {totalDays} days ({startDate:MMM dd} - {endDate:MMM dd}) needs your approval.",
            Type = NotificationType.LeaveRequestCreated,
            Data = payload.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        await _hubContext.Clients.Group(approverId).SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Title,
            notification.Message,
            Type = notification.Type.ToString(),
            notification.Data,
            notification.CreatedAt
        });

        _logger.LogInformation("Sent leave request notification to approver {ApproverId}", approverId);
    }

    private async Task HandleLeaveRequestApproved(NotificationDbContext context, JsonElement payload)
    {
        var employeeId = payload.GetProperty("EmployeeId").GetString();

        if (!Guid.TryParse(employeeId, out var employeeGuid))
        {
            return;
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = employeeGuid,
            Title = "Leave Request Approved",
            Message = "Your leave request has been approved.",
            Type = NotificationType.LeaveRequestApproved,
            Data = payload.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        await _hubContext.Clients.Group(employeeId).SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Title,
            notification.Message,
            Type = notification.Type.ToString(),
            notification.Data,
            notification.CreatedAt
        });

        _logger.LogInformation("Sent leave approved notification to employee {EmployeeId}", employeeId);
    }

    private async Task HandleLeaveRequestRejected(NotificationDbContext context, JsonElement payload)
    {
        var employeeId = payload.GetProperty("EmployeeId").GetString();
        var reason = payload.TryGetProperty("Reason", out var reasonProp) ? reasonProp.GetString() : "";

        if (!Guid.TryParse(employeeId, out var employeeGuid))
        {
            return;
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = employeeGuid,
            Title = "Leave Request Rejected",
            Message = $"Your leave request has been rejected. Reason: {reason}",
            Type = NotificationType.LeaveRequestRejected,
            Data = payload.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        await _hubContext.Clients.Group(employeeId).SendAsync("ReceiveNotification", new
        {
            notification.Id,
            notification.Title,
            notification.Message,
            Type = notification.Type.ToString(),
            notification.Data,
            notification.CreatedAt
        });

        _logger.LogInformation("Sent leave rejected notification to employee {EmployeeId}", employeeId);
    }

    public override void Dispose()
    {
        _channel?.Close();
        base.Dispose();
    }
}
