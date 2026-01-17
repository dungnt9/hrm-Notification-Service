namespace NotificationService.Domain.Entities;

public class NotificationLog
{
    public Guid Id { get; set; }
    public Guid? NotificationId { get; set; }
    public Guid UserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Error { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
