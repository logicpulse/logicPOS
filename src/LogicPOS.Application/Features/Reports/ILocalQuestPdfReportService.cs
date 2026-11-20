namespace LogicPOS.Application.Features.Reports;

public sealed record LocalReportPdfRequest
{
    public required string Key { get; init; }

    public DateTime Start { get; init; }

    public DateTime End { get; init; }

    public string? DocumentTypeAcronym { get; init; }

    public Guid? TerminalId { get; init; }

    public Guid? CustomerId { get; init; }

    public Guid? ArticleId { get; init; }

    public Guid? WarehouseId { get; init; }

    public Guid? VatRateId { get; init; }

    public Guid? FamilyId { get; init; }

    public Guid? SubfamilyId { get; init; }

    public string? SerialNumber { get; init; }

    public string? DocumentNumber { get; init; }
}

public interface ILocalQuestPdfReportService
{
    /// <summary>Returns PDF bytes for a known catalog key, or null if the key is unsupported.</summary>
    Task<byte[]?> TryGenerateAsync(LocalReportPdfRequest request, CancellationToken cancellationToken = default);
}
