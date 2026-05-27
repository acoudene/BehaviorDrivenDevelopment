using AssetManagement.Domain;
using WorkOrderManagement.Domain;

namespace WorkOrderManagement.Tests.Acl;

/// <summary>
/// Adapter de l'Anti-Corruption Layer.
///
/// Il implémente le port <see cref="IAssetCatalog"/> attendu par
/// WorkOrderManagement en s'appuyant sur le modèle du contexte AssetManagement
/// (<see cref="IAssetRepository"/> / <see cref="Asset"/>). Sa responsabilité :
/// traduire le modèle amont en <see cref="AssetReference"/> et appliquer la
/// règle d'éligibilité (seul un équipement actif peut recevoir une demande).
///
/// C'est cet adapter qui empêche le modèle d'AssetManagement de « fuiter » dans
/// le contexte WorkOrderManagement.
/// </summary>
public sealed class AssetManagementCatalogAdapter : IAssetCatalog
{
    private readonly IAssetRepository _assetRepository;

    public AssetManagementCatalogAdapter(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public AssetReference? ResolveForIntervention(Guid assetId)
    {
        var asset = _assetRepository.FindById(new AssetId(assetId));

        // Inconnu OU non actif (déposé) => non éligible : rien ne traverse l'ACL.
        if (asset is null || !asset.IsActive)
            return null;

        return new AssetReference(asset.Id.Value, $"{asset.Tag} - {asset.Designation}");
    }
}
