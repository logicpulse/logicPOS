namespace LogicPOS.Domain.ValueObjects;

/// <summary>
/// Portuguese NIF (Número de Identificação Fiscal) validation — digits and mod-11 only.
/// No infrastructure dependencies. Call on save paths; do not use when materializing entities.
/// </summary>
/// <remarks>
/// Accepted prefixes (AT Portal das Finanças / RNPC tables, plus legacy sole-trader 8):
/// 1, 2, 3 — individuals; 45 — non-resident individuals (final withholding);
/// 5 — legal entities (NIPC); 6 — public administration;
/// 70, 74, 75 — undivided estates; 71 — non-resident entities (final withholding);
/// 72 — investment funds; 77 — ex-officio assignment; 78 — VAT refund non-residents;
/// 79 — exceptional Expo 98; 8 — sole traders (legacy / obsolete, still accepted);
/// 90, 91 — condominiums / irregular companies; 98 — non-residents without establishment;
/// 99 — civil societies without legal personality.
/// Final consumer <see cref="FinalConsumer"/> (999999990) is always valid.
/// Optional leading "PT" is stripped after trim (same convention as legacy UI checks).
/// </remarks>
public static class PortugueseNif
{
    public const string FinalConsumer = "999999990";

    // Longer prefixes first so "45" wins over a bare "4".
    private static readonly string[] ValidPrefixes =
    [
        "45", "70", "71", "72", "74", "75", "77", "78", "79", "90", "91", "98", "99",
        "1", "2", "3", "5", "6", "8"
    ];

    /// <summary>
    /// PT validation applies when the country is Portugal or when the country code is
    /// null/empty (defaults to PT, matching existing listing conventions).
    /// </summary>
    public static bool AppliesToCountry(string? countryCode2)
    {
        if (string.IsNullOrWhiteSpace(countryCode2))
        {
            return true;
        }

        return countryCode2.Trim().Equals("PT", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsValid(string? value) => TryValidate(value, out _);

    public static bool TryValidate(string? value, out PortugueseNifValidationResult result)
    {
        if (value is null)
        {
            result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.Empty);
            return false;
        }

        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.Empty);
            return false;
        }

        if (trimmed.StartsWith("PT", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = trimmed[2..].Trim();
        }

        if (trimmed.Length != 9)
        {
            result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.Length);
            return false;
        }

        for (var i = 0; i < 9; i++)
        {
            if (char.IsDigit(trimmed[i]) == false)
            {
                result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.NonDigits);
                return false;
            }
        }

        if (trimmed == FinalConsumer)
        {
            result = PortugueseNifValidationResult.Ok(trimmed);
            return true;
        }

        if (HasValidPrefix(trimmed) == false)
        {
            result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.Prefix);
            return false;
        }

        var sum = 0;
        for (var i = 0; i < 8; i++)
        {
            sum += (trimmed[i] - '0') * (9 - i);
        }

        var remainder = sum % 11;
        var check = remainder < 2 ? 0 : 11 - remainder;
        if (check != trimmed[8] - '0')
        {
            result = PortugueseNifValidationResult.Fail(PortugueseNifInvalidReason.CheckDigit);
            return false;
        }

        result = PortugueseNifValidationResult.Ok(trimmed);
        return true;
    }

    private static bool HasValidPrefix(string digits)
    {
        foreach (var prefix in ValidPrefixes)
        {
            if (digits.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}

public enum PortugueseNifInvalidReason
{
    None = 0,
    Empty,
    Length,
    NonDigits,
    Prefix,
    CheckDigit
}

public readonly struct PortugueseNifValidationResult
{
    private PortugueseNifValidationResult(bool isValid, PortugueseNifInvalidReason reason, string? normalized)
    {
        IsValid = isValid;
        Reason = reason;
        Normalized = normalized;
    }

    public bool IsValid { get; }
    public PortugueseNifInvalidReason Reason { get; }
    public string? Normalized { get; }

    public static PortugueseNifValidationResult Ok(string normalized) =>
        new(true, PortugueseNifInvalidReason.None, normalized);

    public static PortugueseNifValidationResult Fail(PortugueseNifInvalidReason reason) =>
        new(false, reason, null);
}
