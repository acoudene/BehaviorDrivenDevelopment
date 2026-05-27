using AssetManagement.Domain;
using Reqnroll;
using WorkOrderManagement.Domain;
using WorkOrderManagement.Tests.Acl;
using Xunit;

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
        Assert.NotNull(_result);
        Assert.True(_result!.IsSubmitted, $"Issue obtenue : {_result.Outcome}");
        Assert.Single(_requests.All());
    }

    [Then(@"aucune demande d'intervention n'est créée")]
    public void AucuneDemandeDInterventionNEstCréée()
    {
        Assert.NotNull(_result);
        Assert.False(_result!.IsSubmitted);
        Assert.Empty(_requests.All());
    }

    [Then(@"la demande référence l'équipement ""(.*)""")]
    public void LaDemandeRéférenceLÉquipement(string label)
    {
        Assert.NotNull(_result?.Request);
        Assert.Equal(label, _result!.Request!.Asset.Label);
        Assert.Equal(_resolvedAssetId, _result.Request.Asset.AssetId);
    }

    [Then(@"la demande est en priorité ""(.*)""")]
    public void LaDemandeEstEnPriorité(string priorité)
    {
        Assert.NotNull(_result?.Request);
        Assert.Equal(TraduirePriorité(priorité), _result!.Request!.Priority);
    }

    [Then(@"la demande est déclarée par ""(.*)""")]
    public void LaDemandeEstDéclaréePar(string technicien)
    {
        Assert.NotNull(_result?.Request);
        Assert.Equal(technicien, _result!.Request!.ReportedBy);
    }

    [Then(@"la demande est au statut ""(.*)""")]
    public void LaDemandeEstAuStatut(string statut)
    {
        Assert.NotNull(_result?.Request);
        var attendu = statut switch
        {
            "Soumise" => InterventionRequestStatus.Submitted,
            "Prise en compte" => InterventionRequestStatus.Acknowledged,
            "Clôturée" => InterventionRequestStatus.Closed,
            var s => throw new ArgumentException($"Statut inconnu : {s}")
        };
        Assert.Equal(attendu, _result!.Request!.Status);
    }

    [Then(@"la déclaration est refusée avec le motif ""(.*)""")]
    public void LaDéclarationEstRefuséeAvecLeMotif(string motif)
    {
        Assert.NotNull(_result);
        Assert.Equal(motif, MotifLisible(_result!.Outcome));
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
