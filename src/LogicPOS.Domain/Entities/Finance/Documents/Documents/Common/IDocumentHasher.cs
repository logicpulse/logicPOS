namespace LogicPOS.Domain.Services;

public interface IDocumentHasher
{
    public Task<string> GenerateDocumentHashAsync(Guid seriesId,
        string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default);

    public Task<String> GenerateReceiptHashAsync(string lastReceiptNumber, string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default);

    public string GenerateHash4Chars(string hash);
}