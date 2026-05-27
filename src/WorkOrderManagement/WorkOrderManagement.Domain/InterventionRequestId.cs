namespace WorkOrderManagement.Domain;

public readonly record struct InterventionRequestId(Guid Value)
{
    public static InterventionRequestId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
