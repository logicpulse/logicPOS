using LogicPOS.ApiServer.Data;
using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.ApiServer.Services;

public sealed class WarehouseService
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _dbContext.ApiWarehouses.AsNoTracking().Where(item => !item.IsDeleted).OrderBy(item => item.Order).ToListAsync(cancellationToken);
        if (warehouses.Count == 0)
        {
            return [];
        }

        var warehouseIds = warehouses.Select(item => item.Id).ToList();
        var locations = await _dbContext.ApiWarehouseLocations.AsNoTracking()
            .Where(item => warehouseIds.Contains(item.WarehouseId) && !item.IsDeleted)
            .ToListAsync(cancellationToken);

        return warehouses.Select(warehouse => Map(warehouse, locations.Where(l => l.WarehouseId == warehouse.Id).ToList())).ToList();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiWarehouses.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ApiWarehouseLocations.AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<WarehouseLocationResponse?> GetLocationByIdInternalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _dbContext.ApiWarehouseLocations.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (location is null)
        {
            return null;
        }

        var warehouse = await _dbContext.ApiWarehouses.AsNoTracking().SingleOrDefaultAsync(item => item.Id == location.WarehouseId, cancellationToken);
        return MapLocation(location, warehouse is null ? null : Map(warehouse, []));
    }

    public async Task<AddEntityIdResponse> CreateAsync(AddWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _dbContext.ApiWarehouses.CountAsync(cancellationToken);
        var warehouse = new ApiWarehouse
        {
            Id = Guid.NewGuid(),
            Order = (uint)(count + 1),
            Code = $"W{count + 1}",
            Designation = request.Designation.Trim(),
            IsDefault = request.IsDefault,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        _dbContext.ApiWarehouses.Add(warehouse);

        foreach (var locationName in request.Locations ?? [])
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                continue;
            }

            _dbContext.ApiWarehouseLocations.Add(new ApiWarehouseLocation
            {
                Id = Guid.NewGuid(),
                WarehouseId = warehouse.Id,
                Designation = locationName.Trim(),
                IsDefault = false,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddEntityIdResponse { Id = warehouse.Id };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.ApiWarehouses.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (warehouse is null)
        {
            return false;
        }

        warehouse.Order = request.Order;
        warehouse.Code = string.IsNullOrWhiteSpace(request.Code) ? warehouse.Code : request.Code;
        warehouse.Designation = request.Designation.Trim();
        warehouse.IsDefault = request.IsDefault;
        warehouse.Notes = request.Notes ?? string.Empty;
        warehouse.IsDeleted = request.IsDeleted;
        warehouse.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _dbContext.ApiWarehouses.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (warehouse is null)
        {
            return false;
        }

        _dbContext.ApiWarehouses.Remove(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AddLocationsAsync(Guid warehouseId, AddWarehouseLocationsRequest request, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(warehouseId, cancellationToken))
        {
            return false;
        }

        foreach (var locationName in request.Locations)
        {
            if (string.IsNullOrWhiteSpace(locationName))
            {
                continue;
            }

            _dbContext.ApiWarehouseLocations.Add(new ApiWarehouseLocation
            {
                Id = Guid.NewGuid(),
                WarehouseId = warehouseId,
                Designation = locationName.Trim(),
                IsDefault = false,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateLocationAsync(Guid id, UpdateWarehouseLocationRequest request, CancellationToken cancellationToken = default)
    {
        var location = await _dbContext.ApiWarehouseLocations.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (location is null)
        {
            return false;
        }

        location.Designation = request.Designation.Trim();
        location.IsDefault = request.IsDefault;
        location.UpdatedUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteLocationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _dbContext.ApiWarehouseLocations.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (location is null)
        {
            return false;
        }

        _dbContext.ApiWarehouseLocations.Remove(location);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    internal static WarehouseResponse Map(ApiWarehouse warehouse, List<ApiWarehouseLocation> locations)
    {
        var response = new WarehouseResponse
        {
            Id = warehouse.Id,
            Notes = warehouse.Notes,
            CreatedAt = warehouse.CreatedUtc,
            UpdatedAt = warehouse.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = warehouse.IsDeleted,
            Order = warehouse.Order,
            Code = warehouse.Code,
            Designation = warehouse.Designation,
            IsDefault = warehouse.IsDefault
        };

        // Nested locations never carry their own Warehouse back-reference here, to avoid a JSON serialization cycle
        // (Warehouse -> Locations -> Warehouse -> ...). GetLocationByIdInternalAsync sets it for the standalone case.
        response.Locations = locations.Select(location => MapLocation(location, null)).ToList();
        return response;
    }

    private static WarehouseLocationResponse MapLocation(ApiWarehouseLocation location, WarehouseResponse? warehouse)
    {
        return new WarehouseLocationResponse
        {
            Id = location.Id,
            Notes = location.Notes,
            CreatedAt = location.CreatedUtc,
            UpdatedAt = location.UpdatedUtc,
            UpdatedBy = Guid.Empty,
            IsDeleted = location.IsDeleted,
            Warehouse = warehouse,
            WarehouseId = location.WarehouseId,
            Designation = location.Designation,
            IsDefault = location.IsDefault
        };
    }
}
