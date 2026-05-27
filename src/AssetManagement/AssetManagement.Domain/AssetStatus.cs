namespace AssetManagement.Domain;

/// <summary>
/// Cycle de vie d'un Asset dans le patrimoine immobilier.
/// </summary>
public enum AssetStatus
{
    /// <summary>Équipement en service, susceptible de tomber en panne.</summary>
    Active,

    /// <summary>Équipement déposé / sorti d'inventaire : son QR Code ne doit plus déclencher d'intervention.</summary>
    Decommissioned
}
