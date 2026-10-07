namespace LogicPOS.Application.Features.System.Users;

/// <summary>
/// JWT purpose for the portal sign-in steps. A missing claim is a full session (Windows PIN login).
/// </summary>
public static class PortalSessionPurpose
{
    public const string ClaimType = "purpose";
    public const string PasswordSetup = "password_setup";
    public const string TwoFactor = "two_factor";

    public static bool IsLimited(string? purpose) =>
        purpose == PasswordSetup || purpose == TwoFactor;
}

/// <summary>
/// Paths a limited portal token may call. Every other request is rejected.
/// </summary>
public static class PortalLimitedSessionGate
{
    public static bool IsAllowed(string? purpose, string method, string? path)
    {
        if (PortalSessionPurpose.IsLimited(purpose) == false)
            return true;

        var normalized = (path ?? string.Empty).Trim().Trim('/');
        var verb = (method ?? string.Empty).Trim().ToUpperInvariant();

        if (purpose == PortalSessionPurpose.PasswordSetup)
        {
            return verb == "POST"
                && normalized.Equals("auth/portal-password", StringComparison.OrdinalIgnoreCase);
        }

        if (purpose == PortalSessionPurpose.TwoFactor)
        {
            if (verb == "GET" && normalized.Equals("auth/two-factor/channels", StringComparison.OrdinalIgnoreCase))
                return true;

            return verb == "POST"
                && (normalized.Equals("auth/two-factor/send", StringComparison.OrdinalIgnoreCase)
                    || normalized.Equals("auth/two-factor/verify", StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}

public static class PortalPasswordRules
{
    public const int MinimumLength = 8;

    public static bool HasPortalPassword(string? storedPassword) =>
        string.IsNullOrWhiteSpace(storedPassword) == false;

    public static bool IsStrongEnough(string? password) =>
        string.IsNullOrWhiteSpace(password) == false
        && password.Trim().Length >= MinimumLength;

    /// <summary>
    /// When a portal password exists, the PIN must not match the portal sign-in.
    /// </summary>
    public static bool SecretMatches(bool hasPortalPassword, bool passwordMatches, bool pinMatches) =>
        hasPortalPassword ? passwordMatches : pinMatches;
}

public enum PortalSignInStep
{
    InvalidCredentials,
    SetPassword,
    TwoFactorUnavailable,
    TwoFactor,
    Complete
}

public static class PortalSignInRules
{
    public static PortalSignInStep Decide(
        bool hasPortalPassword,
        bool secretMatches,
        bool needsTwoFactor,
        bool canDeliverCode)
    {
        if (secretMatches == false)
            return PortalSignInStep.InvalidCredentials;

        if (hasPortalPassword == false)
            return PortalSignInStep.SetPassword;

        if (needsTwoFactor && canDeliverCode == false)
            return PortalSignInStep.TwoFactorUnavailable;

        if (needsTwoFactor)
            return PortalSignInStep.TwoFactor;

        return PortalSignInStep.Complete;
    }
}

public static class PortalVerificationCodeRules
{
    public const int LifetimeMinutes = 5;
    public const int CodeLength = 6;

    public static bool IsExpired(DateTime createdAt, DateTime now) =>
        now > createdAt.AddMinutes(LifetimeMinutes);

    public static bool CanUse(DateTime? usedAt, DateTime createdAt, DateTime now) =>
        usedAt is null && IsExpired(createdAt, now) == false;
}

public static class PortalTwoFactorContacts
{
    public static string? FirstEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var first = email
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

        return string.IsNullOrWhiteSpace(first) ? null : first;
    }

    public static string? ResolveMobile(string? mobilePhone, string? phone)
    {
        var raw = string.IsNullOrWhiteSpace(mobilePhone) ? phone : mobilePhone;
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var normalized = new string(raw.Where(character => char.IsDigit(character) || character == '+').ToArray());
        return normalized.Length == 0 ? null : normalized;
    }
}
