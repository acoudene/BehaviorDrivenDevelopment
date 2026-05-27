namespace WorkOrderManagement.Domain;

/// <summary>
/// Value Object local au Bounded Context WorkOrderManagement.
/// C'est la représentation *traduite* d'un équipement issu du contexte
/// AssetManagement : WorkOrderManagement ne connaît jamais l'aggregate Asset,
/// uniquement cette référence minimale fournie par l'Anti-Corruption Layer.
/// </summary>
public sealed record AssetReference
{
    /// <summary>Identifiant de l'asset (clé étrangère vers AssetManagement).</summary>
    public Guid AssetId { get; }

    /// <summary>Libellé dénormalisé pour affichage sur la demande d'intervention.</summary>
    public string Label { get; }

    public AssetReference(Guid assetId, string label)
    {
        if (assetId == Guid.Empty) throw new ArgumentException("AssetId requis.", nameof(assetId));
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Label requis.", nameof(label));

        AssetId = assetId;
        Label = label;
    }
}
