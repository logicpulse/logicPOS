using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class DocumentTypeService
{
    private readonly ApplicationDbContext _dbContext;

    public DocumentTypeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DocumentTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiDocumentTypes.AsNoTracking().OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<DocumentTypeResponse>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiDocumentTypes.AsNoTracking().Where(item => item.IsActive).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiDocumentTypes.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<ApiDocumentType?> GetByAcronymAsync(string acronym, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiDocumentTypes.AsNoTracking().SingleOrDefaultAsync(item => item.Acronym == acronym, cancellationToken);
    }

    public async Task<DocumentTypeResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiDocumentTypes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateDocumentTypeRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiDocumentTypes.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.PrintCopies = request.PrintCopies;
        item.PrintRequestConfirmation = request.PrintRequestConfirmation;
        item.PrintOpenDrawer = request.PrintOpenDrawer;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static DocumentTypeResponse Map(ApiDocumentType item)
    {
        return new DocumentTypeResponse
        {
            Id = item.Id,
            Notes = string.Empty,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = !item.IsActive,
            Order = item.Order,
            Code = item.Code,
            Designation = item.Designation,
            Acronym = item.Acronym,
            PrintCopies = item.PrintCopies,
            PrintRequestMotive = item.PrintRequestMotive,
            PrintRequestConfirmation = item.PrintRequestConfirmation,
            PrintOpenDrawer = item.PrintOpenDrawer,
            SaftDocumentType = (int)item.SaftDocumentType
        };
    }
}
