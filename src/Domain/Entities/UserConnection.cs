namespace NotificationService.Domain.Entities;

public class UserConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public string? DeviceType { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? DisconnectedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
