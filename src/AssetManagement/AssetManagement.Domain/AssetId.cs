namespace AssetManagement.Domain;

/// <summary>
/// Identité d'un Asset au sein du Bounded Context AssetManagement.
/// </summary>
public readonly record struct AssetId(Guid Value)
{
    public static AssetId New() => new(Guid.NewGuid());

    public static bool TryParse(string? raw, out AssetId assetId)
    {
        if (Guid.TryParse(raw, out var guid) && guid != Guid.Empty)
        {
            assetId = new AssetId(guid);
            return true;
        }

        assetId = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}
