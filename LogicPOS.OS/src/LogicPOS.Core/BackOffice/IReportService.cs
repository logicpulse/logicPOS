namespace LogicPOS.Core.BackOffice;

public sealed class ReportDefinition
{
    public required string Key { get; init; }

    public required string Name { get; init; }

    public required string Group { get; init; }

    public bool Dated { get; init; } = true;

    public bool NeedsCustomer { get; init; }

    public bool NeedsArticle { get; init; }

    public string? Endpoint { get; init; }
}

public interface IReportService
{
    IReadOnlyList<ReportDefinition> Catalog { get; }

    Task<ListingSaveResult> GenerateAsync(
        string key,
        DateTime start,
        DateTime end,
        Guid? customerId,
        Guid? articleId,
        CancellationToken cancellationToken = default);
}
