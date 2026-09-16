using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class PlaceService
{
    private readonly ApplicationDbContext _dbContext;

    public PlaceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PlaceResponse>> GetAllAsync(Guid? placeId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ApiPlaces.AsNoTracking().Where(item => !item.IsDeleted);
        if (placeId.HasValue)
        {
            query = query.Where(item => item.Id == placeId.Value);
        }

        var places = await query.OrderBy(item => item.Order).ToListAsync(cancellationToken);
        if (places.Count == 0)
        {
            return [];
        }

        var movementTypes = await _dbContext.ApiMovementTypes
            .AsNoTracking()
            .Where(item => places.Select(place => place.MovementTypeId).Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);

        return places.Select(place => Map(place, movementTypes.GetValueOrDefault(place.MovementTypeId))).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiPlaces.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<PlaceResponse?> GetByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var place = await _dbContext.ApiPlaces.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (place is null)
        {
            return null;
        }

        var movementType = await _dbContext.ApiMovementTypes.AsNoTracking().SingleOrDefaultAsync(item => item.Id == place.MovementTypeId, cancellationToken);
        return Map(place, movementType);
    }

    public async Task<(bool Success, string? Error, AddEntityIdResponse? Response)> CreateAsync(AddPlaceRequest request, MovementTypeService movementTypeService, CancellationToken cancellationToken = default)
    {
        if (!await movementTypeService.ExistsAsync(request.MovementTypeId, cancellationToken))
        {
            return (false, "MovementTypeId não existe.", null);
        }

        var count = await _dbContext.ApiPlaces.CountAsync(cancellationToken);
        var place = new ApiPlace
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"P{count + 1}",
            Designation = request.Designation.Trim(),
            PriceTypeId = request.PriceTypeId,
            MovementTypeId = request.MovementTypeId,
            ButtonImage = request.ButtonImage,
            Notes = request.Notes ?? string.Empty,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiPlaces.Add(place);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (true, null, new AddEntityIdResponse { Id = place.Id });
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, UpdatePlaceRequest request, MovementTypeService movementTypeService, CancellationToken cancellationToken = default)
    {
        var place = await _dbContext.ApiPlaces.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (place is null)
        {
            return (false, "Place não encontrado.");
        }

        if (!await movementTypeService.ExistsAsync(request.MovementTypeId, cancellationToken))
        {
            return (false, "MovementTypeId não existe.");
        }

        place.Order = request.Order;
        place.Code = string.IsNullOrWhiteSpace(request.Code) ? place.Code : request.Code;
        place.Designation = request.Designation.Trim();
        place.PriceTypeId = request.PriceTypeId;
        place.MovementTypeId = request.MovementTypeId;
        place.ButtonImage = request.ButtonImage;
        place.Notes = request.Notes ?? string.Empty;
        place.IsDeleted = request.IsDeleted;
        place.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var place = await _dbContext.ApiPlaces.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (place is null)
        {
            return false;
        }

        _dbContext.ApiPlaces.Remove(place);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static PlaceResponse Map(ApiPlace place, ApiMovementType? movementType)
    {
        return new PlaceResponse
        {
            Id = place.Id,
            Notes = place.Notes,
            CreatedAt = place.CreatedUtc,
            UpdatedAt = place.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = place.IsDeleted,
            Order = place.Order,
            Code = place.Code,
            Designation = place.Designation,
            PriceType = 0,
            MovementType = movementType is null ? null : MovementTypeService.Map(movementType),
            ButtonImage = place.ButtonImage
        };
    }
}
