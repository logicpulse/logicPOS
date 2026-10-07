using LogicPOS.Domain.Services;
using LogicPOS.Persistence.Database;

namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Placeholder hasher for the public edition. Certification hashes belong to a plugin.
/// </summary>
public sealed class LocalDocumentHasher : IDocumentHasher
{
    public LocalDocumentHasher(LogicPOSDbContext database)
    {
    }

    public Task<string> GenerateDocumentHashAsync(
        Guid seriesId,
        string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default)
        => Task.FromResult(string.Empty);

    public Task<string> GenerateReceiptHashAsync(
        string lastReceiptNumber,
        string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default)
        => Task.FromResult(string.Empty);

    public string GenerateHash4Chars(string hash) => string.Empty;
}
