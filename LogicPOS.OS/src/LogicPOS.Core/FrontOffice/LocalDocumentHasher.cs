using System.Security.Cryptography;
using System.Text;
using LogicPOS.Domain.Services;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Local document hash for this open-source host.
/// The certified SAFT private-key signature stays out of this tree.
/// </summary>
public sealed class LocalDocumentHasher : IDocumentHasher
{
    private readonly LogicPOSDbContext _database;

    public LocalDocumentHasher(LogicPOSDbContext database)
    {
        _database = database;
    }

    public async Task<string> GenerateDocumentHashAsync(
        Guid seriesId,
        string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default)
    {
        var lastHash = await _database.Documents
            .Where(document => document.SeriesId == seriesId)
            .OrderByDescending(document => document.CreatedAt)
            .Select(document => document.Hash)
            .FirstOrDefaultAsync(ct) ?? string.Empty;

        return Sign(documentDate, systemEntryDate, documentNumber, grossTotal, lastHash);
    }

    public Task<string> GenerateReceiptHashAsync(
        string lastReceiptNumber,
        string documentNumber,
        string documentDate,
        string systemEntryDate,
        decimal grossTotal,
        CancellationToken ct = default)
    {
        return Task.FromResult(Sign(documentDate, systemEntryDate, documentNumber, grossTotal, lastReceiptNumber));
    }

    public string GenerateHash4Chars(string hash)
    {
        if (hash.Length < 31)
        {
            return hash;
        }

        return string.Concat(hash[0], hash[10], hash[20], hash[30]);
    }

    private static string Sign(
        string documentDate,
        string systemEntryDate,
        string documentNumber,
        decimal grossTotal,
        string previous)
    {
        var plain = $"{documentDate};{systemEntryDate};{documentNumber};{grossTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)};{previous}";
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(plain)));
    }
}
