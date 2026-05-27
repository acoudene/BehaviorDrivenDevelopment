namespace WorkOrderManagement.Domain;

public enum DeclareInterventionOutcome
{
    /// <summary>La demande d'intervention a été créée.</summary>
    Submitted,

    /// <summary>L'asset référencé n'est pas éligible (inconnu / déposé selon l'ACL).</summary>
    AssetNotEligible,

    /// <summary>Les informations de panne fournies sont invalides.</summary>
    InvalidFailureReport
}

public sealed record DeclareInterventionResult
{
    public DeclareInterventionOutcome Outcome { get; }
    public InterventionRequest? Request { get; }

    private DeclareInterventionResult(DeclareInterventionOutcome outcome, InterventionRequest? request)
    {
        Outcome = outcome;
        Request = request;
    }

    public bool IsSubmitted => Outcome == DeclareInterventionOutcome.Submitted;

    public static DeclareInterventionResult Submitted(InterventionRequest request) =>
        new(DeclareInterventionOutcome.Submitted, request);

    public static DeclareInterventionResult AssetNotEligible() =>
        new(DeclareInterventionOutcome.AssetNotEligible, null);

    public static DeclareInterventionResult InvalidFailureReport() =>
        new(DeclareInterventionOutcome.InvalidFailureReport, null);
}
