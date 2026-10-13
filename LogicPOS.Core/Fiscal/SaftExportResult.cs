namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Result of a SAF-T export. Cloud/API fills <see cref="Path"/> (temp file) and optional validation.
/// Local plugins without Infrastructure validation leave <see cref="IsValidationValid"/> true;
/// when the plugin can reach <c>ISaftPtValidator</c>, populate <see cref="ValidationNotice"/> the same way.
/// </summary>
public sealed class SaftExportResult
{
    public string? Path { get; init; }

    public string? Error { get; init; }

    /// <summary>
    /// True when validation found no errors, or validation was not run.
    /// </summary>
    public bool IsValidationValid { get; init; } = true;

    /// <summary>
    /// Human-readable validation issues plus helpdesk URL when invalid. Null when valid / skipped.
    /// </summary>
    public string? ValidationNotice { get; init; }

    public static SaftExportResult Fail(string error) => new() { Error = error };

    public static SaftExportResult Ok(string path, bool isValidationValid = true, string? validationNotice = null) => new()
    {
        Path = path,
        IsValidationValid = isValidationValid,
        ValidationNotice = validationNotice
    };

    /// <summary>
    /// Plugin stub completed with no file on disk.
    /// </summary>
    public static SaftExportResult CompletedWithoutFile() => new();
}
