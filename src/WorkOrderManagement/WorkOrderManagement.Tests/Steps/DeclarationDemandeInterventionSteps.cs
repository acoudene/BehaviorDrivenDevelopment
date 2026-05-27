using AssetManagement.Domain;
using NUnit.Framework;
using Reqnroll;
using WorkOrderManagement.Domain;
using WorkOrderManagement.Tests.Acl;

namespace WorkOrderManagement.Tests.Steps;

[Binding]
public sealed class DeclarationDemandeInterventionSteps
{
    private readonly InMemoryAssetRepository _assetCatalogBackend = new();
    private readonly InMemoryInterventionRequestRepository _requests = new();
    private DeclareInterventionRequestService _service = null!;

    private Guid _resolvedAssetId;
    private DeclareInterventionResult? _result;

    [Given(@"le catalogue d'assets exposé par AssetManagement")]
    public void LeCatalogueDAssetsExposéParAssetManagement(DataTable table)
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
            _assetCatalogBackend.Add(new Asset(id, row["Tag"], row["Désignation"], row["Localisation"], status));
        }

        IAssetCatalog catalog = new AssetManagementCatalogAdapter(_assetCatalogBackend);
        _service = new DeclareInterventionRequestService(catalog, _requests);
    }

    [Given(@"l'équipement résolu d'identifiant ""(.*)""")]
    public void LÉquipementRésoluDIdentifiant(string assetId) => _resolvedAssetId = Guid.Parse(assetId);

    [When(@"le technicien ""(.*)"" déclare une panne ""(.*)"" en priorité ""(.*)""")]
    public void LeTechnicienDéclareUnePanneEnPriorité(string technicien, string description, string priorité)
    {
        var report = new FailureReport(description, TraduirePriorité(priorité), technicien);
        _result = _service.Declare(_resolvedAssetId, report);
    }

    [Then(@"une demande d'intervention est créée")]
    public void UneDemandeDInterventionEstCréée()
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(_result!.IsSubmitted, Is.True, $"Issue obtenue : {_result.Outcome}");
        Assert.That(_requests.All(), Has.Count.EqualTo(1));
    }

    [Then(@"aucune demande d'intervention n'est créée")]
    public void AucuneDemandeDInterventionNEstCréée()
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(_result!.IsSubmitted, Is.False);
        Assert.That(_requests.All(), Is.Empty);
    }

    [Then(@"la demande référence l'équipement ""(.*)""")]
    public void LaDemandeRéférenceLÉquipement(string label)
    {
        Assert.That(_result?.Request, Is.Not.Null);
        Assert.That(_result!.Request!.Asset.Label, Is.EqualTo(label));
        Assert.That(_result.Request.Asset.AssetId, Is.EqualTo(_resolvedAssetId));
    }

    [Then(@"la demande est en priorité ""(.*)""")]
    public void LaDemandeEstEnPriorité(string priorité)
    {
        Assert.That(_result?.Request, Is.Not.Null);
        Assert.That(_result!.Request!.Priority, Is.EqualTo(TraduirePriorité(priorité)));
    }

    [Then(@"la demande est déclarée par ""(.*)""")]
    public void LaDemandeEstDéclaréePar(string technicien)
    {
        Assert.That(_result?.Request, Is.Not.Null);
        Assert.That(_result!.Request!.ReportedBy, Is.EqualTo(technicien));
    }

    [Then(@"la demande est au statut ""(.*)""")]
    public void LaDemandeEstAuStatut(string statut)
    {
        Assert.That(_result?.Request, Is.Not.Null);
        var attendu = statut switch
        {
            "Soumise" => InterventionRequestStatus.Submitted,
            "Prise en compte" => InterventionRequestStatus.Acknowledged,
            "Clôturée" => InterventionRequestStatus.Closed,
            var s => throw new ArgumentException($"Statut inconnu : {s}")
        };
        Assert.That(_result!.Request!.Status, Is.EqualTo(attendu));
    }

    [Then(@"la déclaration est refusée avec le motif ""(.*)""")]
    public void LaDéclarationEstRefuséeAvecLeMotif(string motif)
    {
        Assert.That(_result, Is.Not.Null);
        Assert.That(MotifLisible(_result!.Outcome), Is.EqualTo(motif));
    }

    private static InterventionPriority TraduirePriorité(string priorité) => priorité switch
    {
        "Basse" => InterventionPriority.Low,
        "Normale" => InterventionPriority.Normal,
        "Haute" => InterventionPriority.High,
        "Critique" => InterventionPriority.Critical,
        var s => throw new ArgumentException($"Priorité inconnue : {s}")
    };

    private static string MotifLisible(DeclareInterventionOutcome outcome) => outcome switch
    {
        DeclareInterventionOutcome.Submitted => "Soumise",
        DeclareInterventionOutcome.AssetNotEligible => "Équipement non éligible",
        DeclareInterventionOutcome.InvalidFailureReport => "Signalement invalide",
        _ => outcome.ToString()
    };
}
