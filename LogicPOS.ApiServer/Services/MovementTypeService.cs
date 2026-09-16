using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class MovementTypeService
{
    private readonly ApplicationDbContext _dbContext;

    public MovementTypeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MovementTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var movementTypes = await _dbContext.ApiMovementTypes
            .AsNoTracking()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Order)
            .ToListAsync(cancellationToken);

        return movementTypes.Select(Map).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiMovementTypes.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddMovementTypeRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiMovementTypes.CountAsync(cancellationToken);
        var movementType = new ApiMovementType
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"MT{count + 1}",
            Designation = request.Designation.Trim(),
            VatDirectSelling = request.VatDirectSelling,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiMovementTypes.Add(movementType);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddEntityIdResponse { Id = movementType.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateMovementTypeRequest request, CancellationToken cancellationToken = default)
    {
        var movementType = await _dbContext.ApiMovementTypes.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movementType is null)
        {
            return false;
        }

        movementType.Order = request.Order;
        movementType.Code = string.IsNullOrWhiteSpace(request.Code) ? movementType.Code : request.Code;
        movementType.Designation = request.Designation.Trim();
        movementType.VatDirectSelling = request.VatDirectSelling;
        movementType.Notes = request.Notes ?? string.Empty;
        movementType.IsDeleted = request.IsDeleted;
        movementType.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movementType = await _dbContext.ApiMovementTypes.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movementType is null)
        {
            return false;
        }

        _dbContext.ApiMovementTypes.Remove(movementType);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static MovementTypeResponse Map(ApiMovementType movementType)
    {
        return new MovementTypeResponse
        {
            Id = movementType.Id,
            Notes = movementType.Notes,
            CreatedAt = movementType.CreatedUtc,
            UpdatedAt = movementType.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = movementType.IsDeleted,
            Order = movementType.Order,
            Code = movementType.Code,
            Designation = movementType.Designation,
            VatDirectSelling = movementType.VatDirectSelling
        };
    }
}
