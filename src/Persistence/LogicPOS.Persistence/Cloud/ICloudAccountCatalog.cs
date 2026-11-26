namespace LogicPOS.Persistence.Cloud;

public interface ICloudAccountCatalog
{
    CloudAccountSnapshot Find(string clientId);

    /// <summary>Remove a cached tenant snapshot so the next <see cref="Find"/> hits the database.</summary>
    void Invalidate(string clientId);

    /// <summary>
    /// Writes <c>Accounts.PosOperationMode</c> for the active cloud account and drops the cached snapshot.
    /// The default does nothing; the SQL catalog is the one that reaches the ERP.
    /// </summary>
    void SetOperationMode(string clientId, string mode)
    {
    }
}
