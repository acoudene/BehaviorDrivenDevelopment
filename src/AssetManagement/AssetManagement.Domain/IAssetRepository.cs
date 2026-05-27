namespace AssetManagement.Domain;

public interface IAssetRepository
{
    Asset? FindById(AssetId id);
}
