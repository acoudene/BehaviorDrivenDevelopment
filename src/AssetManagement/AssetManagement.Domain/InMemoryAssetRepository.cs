using System.Collections.Concurrent;

namespace AssetManagement.Domain;

/// <summary>
/// Référentiel d'assets en mémoire — suffisant pour l'exécution des scénarios BDD.
/// </summary>
public sealed class InMemoryAssetRepository : IAssetRepository
{
    private readonly ConcurrentDictionary<AssetId, Asset> _assets = new();

    public void Add(Asset asset) => _assets[asset.Id] = asset;

    public Asset? FindById(AssetId id) => _assets.TryGetValue(id, out var asset) ? asset : null;
}
