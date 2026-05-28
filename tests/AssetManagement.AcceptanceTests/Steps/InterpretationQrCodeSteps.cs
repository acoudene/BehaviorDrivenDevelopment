using AssetManagement.Domain;
using Reqnroll;
using Xunit;

namespace AssetManagement.AcceptanceTests.Steps;

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
        Assert.NotNull(_result);
        Assert.True(_result!.IsResolved, $"Issue obtenue : {_result.Outcome}");
        Assert.NotNull(_result.Asset);
    }

    [Then(@"l'équipement identifié porte le tag ""(.*)""")]
    public void LÉquipementIdentifiéPorteLeTag(string tag)
    {
        Assert.NotNull(_result?.Asset);
        Assert.Equal(tag, _result!.Asset!.Tag);
    }

    [Then(@"l'équipement identifié est situé à ""(.*)""")]
    public void LÉquipementIdentifiéEstSituéÀ(string location)
    {
        Assert.NotNull(_result?.Asset);
        Assert.Equal(location, _result!.Asset!.Location);
    }

    [Then(@"l'interprétation échoue avec le motif ""(.*)""")]
    public void LInterprétationÉchoueAvecLeMotif(string motif)
    {
        Assert.NotNull(_result);
        Assert.False(_result!.IsResolved);
        Assert.Equal(motif, MotifLisible(_result.Outcome));
    }

    [Then(@"l'interprétation aboutit au motif ""(.*)""")]
    public void LInterprétationAboutitAuMotif(string motif)
    {
        Assert.NotNull(_result);
        Assert.Equal(motif, MotifLisible(_result!.Outcome));
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
