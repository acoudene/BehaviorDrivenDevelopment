namespace AssetManagement.Domain;

/// <summary>
/// Aggregate Root représentant un équipement du patrimoine immobilier
/// (CTA, ascenseur, GTB, porte automatique, etc.) identifié par un QR Code.
/// </summary>
public sealed class Asset
{
    public AssetId Id { get; }

    /// <summary>Code patrimonial lisible apposé sur l'équipement (ex: "ASC-B2-007").</summary>
    public string Tag { get; }

    public string Designation { get; }

    /// <summary>Localisation fonctionnelle (ex: "Site Lyon - Bât. B - Niveau 2").</summary>
    public string Location { get; }

    public AssetStatus Status { get; private set; }

    public Asset(AssetId id, string tag, string designation, string location, AssetStatus status = AssetStatus.Active)
    {
        if (string.IsNullOrWhiteSpace(tag)) throw new ArgumentException("Le tag patrimonial est requis.", nameof(tag));
        if (string.IsNullOrWhiteSpace(designation)) throw new ArgumentException("La désignation est requise.", nameof(designation));

        Id = id;
        Tag = tag;
        Designation = designation;
        Location = location ?? string.Empty;
        Status = status;
    }

    public bool IsActive => Status == AssetStatus.Active;

    public void Decommission() => Status = AssetStatus.Decommissioned;
}
