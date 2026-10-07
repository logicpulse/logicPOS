namespace LogicPOS.Persistence.Seeding;

public readonly record struct PortalLoginCandidate(Guid Id, string Name, string? ProfileDesignation);

/// <summary>
/// Picks one proprietor when a database has users but nobody can sign in to the portal.
/// </summary>
public static class PortalLoginAssignment
{
    public const string Login = "admin";

    public static bool HasPortalIdentity(string? login, string? email) =>
        string.IsNullOrWhiteSpace(login) == false || string.IsNullOrWhiteSpace(email) == false;

    public static bool IsProprietorProfile(string? designation)
    {
        if (string.IsNullOrWhiteSpace(designation))
            return false;

        var normalized = designation.Trim().ToLowerInvariant()
            .Replace("á", "a")
            .Replace("à", "a")
            .Replace("â", "a")
            .Replace("ã", "a");
        return normalized.Contains("proprietario", StringComparison.Ordinal);
    }

    public static Guid? ChooseProprietor(IEnumerable<PortalLoginCandidate> users) =>
        users
            .Where(user => IsProprietorProfile(user.ProfileDesignation))
            .OrderBy(user => user.Name, StringComparer.OrdinalIgnoreCase)
            .Select(user => (Guid?)user.Id)
            .FirstOrDefault();
}
