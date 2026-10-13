namespace LogicPOS.Application.Features.Finance.Saft;

/// <summary>
/// SAF-T PT validation result. <see cref="IsValid"/> is true only when no issue has severity Error.
/// </summary>
public sealed record SaftValidationReport
{
    /// <summary>
    /// Helpdesk form for SAF-T validation issues (category 6).
    /// </summary>
    public const string HelpdeskUrl = "https://helpdesk.logicpulse.com/index.php?a=add&category=6";

    /// <summary>
    /// Maximum number of issues copied into <see cref="ToHttpPayload"/>.
    /// </summary>
    public const int MaxHttpIssues = 50;

    public bool IsValid { get; }

    /// <summary>
    /// Total issues found. Stays the full count when <see cref="Issues"/> is truncated for HTTP.
    /// </summary>
    public int IssueCount { get; }

    public IReadOnlyList<SaftValidationIssue> Issues { get; }

    private SaftValidationReport(bool isValid, int issueCount, IReadOnlyList<SaftValidationIssue> issues)
    {
        IsValid = isValid;
        IssueCount = issueCount;
        Issues = issues;
    }

    /// <summary>
    /// Builds a report from every issue. Warnings alone leave <see cref="IsValid"/> true.
    /// </summary>
    public static SaftValidationReport Create(IReadOnlyList<SaftValidationIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        var snapshot = issues.ToArray();
        var hasError = false;
        for (var i = 0; i < snapshot.Length; i++)
        {
            if (snapshot[i].Severity == SaftValidationSeverity.Error)
            {
                hasError = true;
                break;
            }
        }

        return new SaftValidationReport(!hasError, snapshot.Length, snapshot);
    }

    /// <summary>
    /// Payload for the validation HTTP header: at most <see cref="MaxHttpIssues"/> issues,
    /// with <see cref="SaftValidationHttpPayload.IssueCount"/> equal to the real total.
    /// </summary>
    public SaftValidationHttpPayload ToHttpPayload()
    {
        IReadOnlyList<SaftValidationIssue> issues = Issues.Count <= MaxHttpIssues
            ? Issues
            : Issues.Take(MaxHttpIssues).ToArray();

        return new SaftValidationHttpPayload(IsValid, IssueCount, issues, HelpdeskUrl);
    }
}

/// <summary>
/// JSON shape of a SAF-T validation report sent over HTTP.
/// </summary>
public sealed record SaftValidationHttpPayload(
    bool IsValid,
    int IssueCount,
    IReadOnlyList<SaftValidationIssue> Issues,
    string HelpdeskUrl);
