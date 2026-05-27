namespace WorkOrderManagement.Domain;

/// <summary>
/// Port de l'Anti-Corruption Layer.
///
/// WorkOrderManagement exprime ici *son* besoin : « donne-moi une référence
/// d'asset exploitable pour ouvrir une demande d'intervention ». L'adapter qui
/// implémente ce port a la responsabilité de dialoguer avec AssetManagement et
/// de traduire son modèle (Asset, statut, etc.) en <see cref="AssetReference"/>,
/// en filtrant ce qui n'est pas éligible (asset inconnu, déposé...).
/// </summary>
public interface IAssetCatalog
{
    /// <summary>
    /// Retourne la référence locale si l'asset est connu ET éligible à une
    /// demande d'intervention ; sinon <c>null</c>.
    /// </summary>
    AssetReference? ResolveForIntervention(Guid assetId);
}
