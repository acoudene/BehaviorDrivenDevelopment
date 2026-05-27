using AssetManagement.Domain;
using NUnit.Framework;
using Reqnroll;

namespace AssetManagement.Tests.Steps;

[Binding]
public sealed class InterpretationQrCodeSteps
{
    private readonly InMemoryAssetRepository _repository = new();
    private QrCodeInterpreter _interpreter = null!;
    private string? _scannedContent;
    private QrInterpretation? _result;

    [Given(@"le référentiel d'assets suivant")]
    public void LeRéférentielDAssetsSuivant(DataTable table)
    {
        foreach (var row in table.Rows)
        {
            var id = new AssetId(Guid.Parse(row["Identifiant"]));
            var status = row["Statut"].Trim() switch
            {
                "Actif" => AssetStatus.Active,
                "Déposé" => AssetStatus.Decommissioned,
                var s => throw new ArgumentException($"Statut inconnu : {s}")
            };

            _repository.Add(new Asset(id, row["Tag"], row["Désignation"], row["Localisation"], status));
        }

        _interpreter = new QrCodeInterpreter(_repository);
    }

    [Given(@"un QR Code encodant l'URL ""(.*)""")]
    public void UnQrCodeEncodantLUrl(string url) => _scannedContent = url;

    [When(@"le technicien scanne le QR Code")]
    public void LeTechnicienScanneLeQrCode()
    {
        _interpreter ??= new QrCodeInterpreter(_repository);
        _result = _interpreter.Interpret(_scannedContent);
    }

    [Then(@"le QR Code est résolu avec succès")]
    public void LeQrCodeEstRésoluAvecSuccès()
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(_result!.IsResolved, Is.True, $"Issue obtenue : {_result.Outcome}");
        Assert.That(_result.Asset, Is.Not.Null);
    }

    [Then(@"l'équipement identifié porte le tag ""(.*)""")]
    public void LÉquipementIdentifiéPorteLeTag(string tag)
    {
        Assert.That(_result?.Asset, Is.Not.Null);
        Assert.That(_result!.Asset!.Tag, Is.EqualTo(tag));
    }

    [Then(@"l'équipement identifié est situé à ""(.*)""")]
    public void LÉquipementIdentifiéEstSituéÀ(string location)
    {
        Assert.That(_result?.Asset, Is.Not.Null);
        Assert.That(_result!.Asset!.Location, Is.EqualTo(location));
    }

    [Then(@"l'interprétation échoue avec le motif ""(.*)""")]
    public void LInterprétationÉchoueAvecLeMotif(string motif)
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(_result!.IsResolved, Is.False);
        Assert.That(MotifLisible(_result.Outcome), Is.EqualTo(motif));
    }

    [Then(@"l'interprétation aboutit au motif ""(.*)""")]
    public void LInterprétationAboutitAuMotif(string motif)
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(MotifLisible(_result!.Outcome), Is.EqualTo(motif));
    }

    private static string MotifLisible(QrInterpretationOutcome outcome) => outcome switch
    {
        QrInterpretationOutcome.Resolved => "Résolu",
        QrInterpretationOutcome.MalformedQrCode => "QR Code malformé",
        QrInterpretationOutcome.UnknownAsset => "Équipement inconnu",
        QrInterpretationOutcome.DecommissionedAsset => "Équipement déposé",
        _ => outcome.ToString()
    };
}
