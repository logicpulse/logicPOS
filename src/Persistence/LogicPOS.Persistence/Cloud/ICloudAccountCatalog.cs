namespace LogicPOS.Persistence.Cloud;

public interface ICloudAccountCatalog
{
    CloudAccountSnapshot Find(string clientId);

    /// <summary>Remove a cached tenant snapshot so the next <see cref="Find"/> hits the database.</summary>
    void Invalidate(string clientId);
}
