using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class PaymentMethodService
{
    private readonly ApplicationDbContext _dbContext;

    public PaymentMethodService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PaymentMethodResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.ApiPaymentMethods.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiPaymentMethods.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<PaymentMethodResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiPaymentMethods.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddPaymentMethodRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiPaymentMethods.CountAsync(cancellationToken);
        var item = new ApiPaymentMethod
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"PM{count + 1}",
            Designation = request.Designation.Trim(),
            Token = request.Token,
            Acronym = request.Acronym,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiPaymentMethods.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = item.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePaymentMethodRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiPaymentMethods.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        item.Order = request.Order;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? item.Code : request.Code;
        item.Designation = request.Designation.Trim();
        item.Token = request.Token;
        item.ResourceString = request.ResourceString;
        item.ButtonIcon = request.ButtonIcon;
        item.Acronym = request.Acronym;
        item.AllowPayback = request.AllowPayback;
        item.Symbol = request.Symbol;
        item.Notes = request.Notes ?? string.Empty;
        item.IsDeleted = request.IsDeleted;
        item.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.ApiPaymentMethods.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        _dbContext.ApiPaymentMethods.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static PaymentMethodResponse Map(ApiPaymentMethod item)
    {
        return new PaymentMethodResponse
        {
            Id = item.Id,
            Notes = item.Notes,
            CreatedAt = item.CreatedUtc,
            UpdatedAt = item.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = item.IsDeleted,
            Order = item.Order,
            Code = item.Code,
            Designation = item.Designation,
            Token = item.Token,
            ResourceString = item.ResourceString,
            ButtonIcon = item.ButtonIcon,
            Acronym = item.Acronym,
            AllowPayback = item.AllowPayback,
            Symbol = item.Symbol
        };
    }
}
