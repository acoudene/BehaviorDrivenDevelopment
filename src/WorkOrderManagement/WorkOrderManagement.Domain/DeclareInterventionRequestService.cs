namespace WorkOrderManagement.Domain;

public sealed record FailureReport(string Description, InterventionPriority Priority, string ReportedBy);

/// <summary>
/// Service applicatif du contexte WorkOrderManagement : à partir d'un AssetId
/// (obtenu en amont par l'interprétation d'un QR Code dans AssetManagement) et
/// d'un signalement de panne, ouvre une demande d'intervention.
///
/// La résolution de l'asset passe exclusivement par l'<see cref="IAssetCatalog"/>
/// (Anti-Corruption Layer) : le contexte ne dépend jamais directement du modèle
/// d'AssetManagement.
/// </summary>
public sealed class DeclareInterventionRequestService
{
    private readonly IAssetCatalog _assetCatalog;
    private readonly IInterventionRequestRepository _repository;
    private readonly TimeProvider _clock;

    public DeclareInterventionRequestService(
        IAssetCatalog assetCatalog,
        IInterventionRequestRepository repository,
        TimeProvider? clock = null)
    {
        _assetCatalog = assetCatalog ?? throw new ArgumentNullException(nameof(assetCatalog));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? TimeProvider.System;
    }

    public DeclareInterventionResult Declare(Guid assetId, FailureReport report)
    {
        if (report is null
            || string.IsNullOrWhiteSpace(report.Description)
            || string.IsNullOrWhiteSpace(report.ReportedBy))
        {
            return DeclareInterventionResult.InvalidFailureReport();
        }

        var assetRef = _assetCatalog.ResolveForIntervention(assetId);
        if (assetRef is null)
            return DeclareInterventionResult.AssetNotEligible();

        var request = new InterventionRequest(
            InterventionRequestId.New(),
            assetRef,
            report.Description,
            report.Priority,
            report.ReportedBy,
            _clock.GetUtcNow());

        _repository.Save(request);
        return DeclareInterventionResult.Submitted(request);
    }
}
