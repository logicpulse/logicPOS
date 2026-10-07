namespace LogicPOS.Persistence.Cloud;

public interface IConnectionStringResolver
{
    string GetConnectionString();

    /// <summary>
    /// False when cloud mode has no client id and the local connection string is empty.
    /// A request without a client id uses the local connection string.
    /// </summary>
    bool TryGetConnectionString(out string connectionString);

    bool IsValidTenant(string? clientId);
}
