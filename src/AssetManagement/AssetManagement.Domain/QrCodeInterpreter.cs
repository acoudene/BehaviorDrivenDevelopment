namespace AssetManagement.Domain;

/// <summary>
/// Domain Service : interprète le contenu brut d'un QR Code (une URL) scanné
/// sur un équipement et le résout vers un <see cref="Asset"/> du référentiel.
///
/// Format attendu du QR Code : une URL absolue dont le dernier segment de chemin
/// est l'identifiant de l'asset, ex :
///   https://gmao.exemple.fr/assets/3f2504e0-4f89-41d3-9a0c-0305e82c3301
/// </summary>
public sealed class QrCodeInterpreter
{
    private const string ExpectedPathSegment = "assets";

    private readonly IAssetRepository _assets;

    public QrCodeInterpreter(IAssetRepository assets)
    {
        _assets = assets ?? throw new ArgumentNullException(nameof(assets));
    }

    public QrInterpretation Interpret(string? scannedContent)
    {
        if (!TryExtractAssetId(scannedContent, out var assetId))
            return QrInterpretation.Malformed();

        var asset = _assets.FindById(assetId);
        if (asset is null)
            return QrInterpretation.Unknown();

        return asset.IsActive
            ? QrInterpretation.Resolved(asset)
            : QrInterpretation.Decommissioned(asset);
    }

    private static bool TryExtractAssetId(string? scannedContent, out AssetId assetId)
    {
        assetId = default;

        if (string.IsNullOrWhiteSpace(scannedContent))
            return false;

        if (!Uri.TryCreate(scannedContent.Trim(), UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme is not ("http" or "https"))
            return false;

        var segments = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        // On attend .../assets/{id}
        if (segments.Length < 2)
            return false;

        if (!segments[^2].Equals(ExpectedPathSegment, StringComparison.OrdinalIgnoreCase))
            return false;

        return AssetId.TryParse(segments[^1], out assetId);
    }
}
