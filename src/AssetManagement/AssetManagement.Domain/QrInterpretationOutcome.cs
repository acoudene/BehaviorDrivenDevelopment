namespace AssetManagement.Domain;

/// <summary>
/// Issue de l'interprétation d'un QR Code scanné sur le terrain.
/// </summary>
public enum QrInterpretationOutcome
{
    /// <summary>Le QR encode une URL valide pointant vers un asset actif et résolu.</summary>
    Resolved,

    /// <summary>Le contenu scanné n'est pas une URL de QR Code GMAO exploitable.</summary>
    MalformedQrCode,

    /// <summary>L'URL est valide mais ne correspond à aucun asset connu du référentiel.</summary>
    UnknownAsset,

    /// <summary>L'asset existe mais a été déposé / sorti d'inventaire.</summary>
    DecommissionedAsset
}
