using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

// The client never creates document types via the API (no AddDocumentType command exists — see
// LogicPOS.Api/Features/Finance/Documents/Types), so this fixed set is seeded once at startup instead.
// Acronyms match LogicPOS.Api.Features.Documents.DocumentTypeAnalyzer, which switches on these exact strings.
public sealed class DocumentTypeSeeder
{
    private static readonly (string Acronym, string Designation, ApiSaftDocumentType Saft)[] DefaultTypes =
    [
        ("FT", "Fatura", ApiSaftDocumentType.SalesInvoices),
        ("FS", "Fatura Simplificada", ApiSaftDocumentType.SalesInvoices),
        ("FR", "Fatura-Recibo", ApiSaftDocumentType.SalesInvoices),
        ("NC", "Nota de Crédito", ApiSaftDocumentType.SalesInvoices),
        ("ND", "Nota de Débito", ApiSaftDocumentType.SalesInvoices),
        ("GR", "Guia de Remessa", ApiSaftDocumentType.MovementOfGoods),
        ("OR", "Orçamento", ApiSaftDocumentType.WorkingDocuments),
        ("RC", "Recibo", ApiSaftDocumentType.Payments)
    ];

    private readonly ApplicationDbContext _dbContext;

    public DocumentTypeSeeder(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingAcronyms = await _dbContext.ApiDocumentTypes.AsNoTracking().Select(item => item.Acronym).ToListAsync(cancellationToken);
        var missing = DefaultTypes.Where(type => !existingAcronyms.Contains(type.Acronym)).ToList();
        if (missing.Count == 0)
        {
            return;
        }

        var order = (uint)existingAcronyms.Count + 1;
        foreach (var (acronym, designation, saft) in missing)
        {
            _dbContext.ApiDocumentTypes.Add(new ApiDocumentType
            {
                Id = Guid.NewGuid(),
                Order = order++,
                Code = acronym,
                Designation = designation,
                Acronym = acronym,
                PrintCopies = 1,
                SaftDocumentType = saft,
                IsActive = true,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
