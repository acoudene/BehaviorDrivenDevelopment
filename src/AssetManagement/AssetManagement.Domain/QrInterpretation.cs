namespace AssetManagement.Domain;

/// <summary>
/// Value Object résultat de l'interprétation d'un QR Code.
/// Immuable et auto-descriptif : porte l'issue et, si résolu, l'asset associé.
/// </summary>
public sealed record QrInterpretation
{
    public QrInterpretationOutcome Outcome { get; }
    public Asset? Asset { get; }

    private QrInterpretation(QrInterpretationOutcome outcome, Asset? asset)
    {
        Outcome = outcome;
        Asset = asset;
    }

    public bool IsResolved => Outcome == QrInterpretationOutcome.Resolved;

    public static QrInterpretation Resolved(Asset asset) =>
        new(QrInterpretationOutcome.Resolved, asset);

    public static QrInterpretation Malformed() =>
        new(QrInterpretationOutcome.MalformedQrCode, null);

    public static QrInterpretation Unknown() =>
        new(QrInterpretationOutcome.UnknownAsset, null);

    public static QrInterpretation Decommissioned(Asset asset) =>
        new(QrInterpretationOutcome.DecommissionedAsset, asset);
}
