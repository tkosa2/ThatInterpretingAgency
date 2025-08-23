using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class Notification : Entity
{
    public Guid AgencyId { get; private set; }
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    private Notification() { }

    public static Notification Create(
        Guid agencyId, 
        Guid userId, 
        NotificationType type, 
        string message,
        string? externalReference = null,
        Dictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        return new Notification
        {
            AgencyId = agencyId,
            UserId = userId,
            Type = type,
            Message = message.Trim(),
            Status = NotificationStatus.Pending,
            ExternalReference = externalReference?.Trim(),
            Metadata = metadata ?? new Dictionary<string, string>()
        };
    }

    public void MarkAsSent(string? externalReference = null)
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(externalReference))
        {
            ExternalReference = externalReference.Trim();
        }
        
        UpdateTimestamp();
    }

    public void MarkAsDelivered()
    {
        if (Status != NotificationStatus.Sent)
            throw new InvalidOperationException("Only sent notifications can be marked as delivered");

        Status = NotificationStatus.Delivered;
        UpdateTimestamp();
    }

    public void MarkAsRead()
    {
        if (Status != NotificationStatus.Delivered)
            throw new InvalidOperationException("Only delivered notifications can be marked as read");

        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarkAsFailed(string? failureReason = null)
    {
        Status = NotificationStatus.Failed;
        if (!string.IsNullOrWhiteSpace(failureReason))
        {
            Metadata["FailureReason"] = failureReason.Trim();
        }
        UpdateTimestamp();
    }

    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));

        Metadata[key.Trim()] = value?.Trim() ?? string.Empty;
        UpdateTimestamp();
    }

    public void Retry()
    {
        if (Status != NotificationStatus.Failed)
            throw new InvalidOperationException("Only failed notifications can be retried");

        Status = NotificationStatus.Pending;
        Metadata.Remove("FailureReason");
        UpdateTimestamp();
    }

    public bool IsReadable => Status == NotificationStatus.Delivered || Status == NotificationStatus.Read;
    public bool CanRetry => Status == NotificationStatus.Failed;
}

public enum NotificationType
{
    Email,
    SMS,
    Push,
    InApp
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed
}
