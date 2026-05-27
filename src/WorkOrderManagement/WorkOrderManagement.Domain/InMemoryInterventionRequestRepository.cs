using System.Collections.Concurrent;

namespace WorkOrderManagement.Domain;

public sealed class InMemoryInterventionRequestRepository : IInterventionRequestRepository
{
    private readonly ConcurrentDictionary<InterventionRequestId, InterventionRequest> _store = new();

    public void Save(InterventionRequest request) => _store[request.Id] = request;

    public InterventionRequest? FindById(InterventionRequestId id) =>
        _store.TryGetValue(id, out var request) ? request : null;

    public IReadOnlyCollection<InterventionRequest> All() => _store.Values.ToArray();
}
