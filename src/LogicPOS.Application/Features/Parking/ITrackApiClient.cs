namespace LogicPOS.Application.Features.Parking;

public interface ITrackApiClient
{
    Task<TrackParkTicketQuoteResult> QuoteAsync(string? payload, CancellationToken cancellationToken = default);

    Task<TrackParkTicketPaidResult> MarkPaidAsync(
        string? payload,
        decimal amountPaid,
        string? documentId,
        CancellationToken cancellationToken = default);
}

public sealed class TrackParkTicketQuoteResult
{
    public bool Success { get; init; }
    public string Error { get; init; } = string.Empty;
    public int StatusCode { get; init; } = 400;
    public int NPedido { get; init; }
    public string ArticleCode { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; }
    public decimal DueAmount { get; init; }
    public bool AlreadyPaid { get; init; }
    public bool AlreadyExit { get; init; }
}

public sealed class TrackParkTicketPaidResult
{
    public bool Success { get; init; }
    public string Error { get; init; } = string.Empty;
    public int StatusCode { get; init; } = 400;
    public bool AlreadyPaid { get; init; }
    public int NPedido { get; init; }
}
