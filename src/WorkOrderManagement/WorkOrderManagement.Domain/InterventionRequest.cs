namespace WorkOrderManagement.Domain;

/// <summary>
/// Aggregate Root : une demande d'intervention (déclaration de panne) rattachée
/// à un équipement via sa <see cref="AssetReference"/>.
/// </summary>
public sealed class InterventionRequest
{
    public InterventionRequestId Id { get; }
    public AssetReference Asset { get; }
    public string FailureDescription { get; }
    public InterventionPriority Priority { get; }
    public string ReportedBy { get; }
    public DateTimeOffset ReportedAt { get; }
    public InterventionRequestStatus Status { get; private set; }

    public InterventionRequest(
        InterventionRequestId id,
        AssetReference asset,
        string failureDescription,
        InterventionPriority priority,
        string reportedBy,
        DateTimeOffset reportedAt)
    {
        if (string.IsNullOrWhiteSpace(failureDescription))
            throw new ArgumentException("La description de la panne est requise.", nameof(failureDescription));
        if (string.IsNullOrWhiteSpace(reportedBy))
            throw new ArgumentException("Le déclarant est requis.", nameof(reportedBy));

        Id = id;
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
        FailureDescription = failureDescription;
        Priority = priority;
        ReportedBy = reportedBy;
        ReportedAt = reportedAt;
        Status = InterventionRequestStatus.Submitted;
    }
}

public enum InterventionRequestStatus
{
    Submitted,
    Acknowledged,
    Closed
}
