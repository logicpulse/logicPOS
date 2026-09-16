using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles/stocks")]
[Route("api/articles/stocks")]
public sealed class StocksController : ApiControllerBase
{
    private readonly WarehouseService _warehouseService;
    private readonly WarehouseArticleService _warehouseArticleService;
    private readonly StockMovementService _stockMovementService;

    public StocksController(WarehouseService warehouseService, WarehouseArticleService warehouseArticleService, StockMovementService stockMovementService)
    {
        _warehouseService = warehouseService;
        _warehouseArticleService = warehouseArticleService;
        _stockMovementService = stockMovementService;
    }

    // --- Warehouses ---

    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses(CancellationToken cancellationToken) => Ok(await _warehouseService.GetAllAsync(cancellationToken));

    [HttpPost("warehouses")]
    public async Task<IActionResult> CreateWarehouse([FromBody] AddWarehouseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        return StatusCode(StatusCodes.Status201Created, await _warehouseService.CreateAsync(request, cancellationToken));
    }

    [HttpPut("warehouses/{id:guid}")]
    public async Task<IActionResult> UpdateWarehouse(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        return await _warehouseService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("warehouses/{id:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id, CancellationToken cancellationToken)
    {
        return await _warehouseService.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }

    // --- Warehouse locations ---

    [HttpPost("warehouses/{id:guid}/locations")]
    public async Task<IActionResult> AddLocations(Guid id, [FromBody] AddWarehouseLocationsRequest request, CancellationToken cancellationToken)
    {
        return await _warehouseService.AddLocationsAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpPut("warehouses/locations/{id:guid}")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateWarehouseLocationRequest request, CancellationToken cancellationToken)
    {
        return await _warehouseService.UpdateLocationAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("warehouses/locations/{id:guid}")]
    public async Task<IActionResult> DeleteLocation(Guid id, CancellationToken cancellationToken)
    {
        return await _warehouseService.DeleteLocationAsync(id, cancellationToken) ? Ok() : NotFound();
    }

    // --- Warehouse article location/quantity change ---

    [HttpPut("{warehouseArticleId:guid}/location")]
    public async Task<IActionResult> ChangeLocation(Guid warehouseArticleId, [FromBody] ChangeArticleLocationRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _warehouseArticleService.ChangeLocationAsync(warehouseArticleId, request, cancellationToken);
        if (!success)
        {
            return error == "WarehouseArticle não encontrado." ? NotFound() : BadRequest(new { errors = new[] { new { name = "locationId", reason = error } } });
        }

        return NoContent();
    }

    // --- Totals ---

    [HttpGet("totals")]
    public async Task<IActionResult> GetTotals([FromQuery] Guid[]? articleIds, CancellationToken cancellationToken)
    {
        return Ok(await _warehouseArticleService.GetTotalsAsync(articleIds ?? [], cancellationToken));
    }

    // --- Stock movements ---

    [HttpGet("movements")]
    public async Task<IActionResult> GetMovements([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] Guid? articleId, CancellationToken cancellationToken)
    {
        return Ok(await _stockMovementService.GetPagedAsync(page ?? 1, pageSize ?? 25, articleId, cancellationToken));
    }

    [HttpGet("movements/{id:guid}")]
    public async Task<IActionResult> GetMovementById(Guid id, CancellationToken cancellationToken)
    {
        var movement = await _stockMovementService.GetByIdAsync(id, cancellationToken);
        return movement is null ? NotFound() : Ok(movement);
    }

    [HttpPost("movements")]
    public async Task<IActionResult> CreateMovement([FromBody] AddStockMovementRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _stockMovementService.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }

    [HttpPut("movements/{id:guid}")]
    public async Task<IActionResult> UpdateMovement(Guid id, [FromBody] UpdateStockMovementRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _stockMovementService.UpdateAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    // --- Article histories ---

    [HttpGet("histories")]
    public async Task<IActionResult> GetHistories([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] Guid? articleId, CancellationToken cancellationToken)
    {
        return Ok(await _stockMovementService.GetHistoriesAsync(page ?? 1, pageSize ?? 25, articleId, cancellationToken));
    }
}
