using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class ReceiptService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DocumentService _documentService;

    public ReceiptService(ApplicationDbContext dbContext, DocumentService documentService)
    {
        _dbContext = dbContext;
        _documentService = documentService;
    }

    public async Task<ReceiptsPaginatedResponse> GetPagedAsync(int page, int pageSize, Guid? customerId, Guid? paymentMethodId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 25 : pageSize;

        var receiptTypeAcronyms = await _dbContext.ApiDocumentTypes.AsNoTracking()
            .Where(t => t.SaftDocumentType == ApiSaftDocumentType.Payments)
            .Select(t => t.Acronym)
            .ToListAsync(cancellationToken);

        var query = _dbContext.ApiDocuments.AsNoTracking().Include(d => d.PaymentMethods)
            .Where(d => receiptTypeAcronyms.Contains(d.Type));

        if (customerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == customerId.Value);
        }

        if (paymentMethodId.HasValue)
        {
            query = query.Where(d => d.PaymentMethods.Any(pm => pm.PaymentMethodId == paymentMethodId.Value));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var receipts = await query.OrderByDescending(d => d.CreatedUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        var items = _documentService.MapReceipts(receipts);

        return new ReceiptsPaginatedResponse
        {
            Items = items,
            ItemsCount = receipts.Count,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public Task<(bool Success, string? Error)> CancelAsync(Guid id, string? reason, CancellationToken cancellationToken = default)
    {
        return _documentService.CancelAsync(id, reason, cancellationToken);
    }
}
