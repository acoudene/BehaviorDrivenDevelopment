namespace WorkOrderManagement.Domain;

public interface IInterventionRequestRepository
{
    void Save(InterventionRequest request);
    InterventionRequest? FindById(InterventionRequestId id);
    IReadOnlyCollection<InterventionRequest> All();
}
