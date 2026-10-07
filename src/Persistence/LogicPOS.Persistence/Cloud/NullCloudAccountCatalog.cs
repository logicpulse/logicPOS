namespace LogicPOS.Persistence.Cloud;

/// <summary>Used when the API is not in cloud mode. Never returns an account.</summary>
public sealed class NullCloudAccountCatalog : ICloudAccountCatalog
{
    public CloudAccountSnapshot Find(string clientId) => CloudAccountSnapshot.NotFound;
}
