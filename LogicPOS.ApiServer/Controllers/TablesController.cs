using LogicPOS.ApiServer.Data.Entities;
using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("tables")]
[Route("api/tables")]
public sealed class TablesController : ApiControllerBase
{
    private readonly TableService _tableService;

    public TablesController(TableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? status, [FromQuery] Guid? placeId, CancellationToken cancellationToken)
    {
        var parsedStatus = status.HasValue ? (ApiTableStatus)status.Value : (ApiTableStatus?)null;
        return Ok(await _tableService.GetAllAsync(parsedStatus, placeId, cancellationToken));
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefault(CancellationToken cancellationToken)
    {
        var table = await _tableService.GetDefaultAsync(cancellationToken);
        return table is null ? NotFound() : Ok(table);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var table = await _tableService.GetByIdAsync(id, cancellationToken);
        return table is null ? NotFound() : Ok(table);
    }

    [HttpGet("{id:guid}/view")]
    public async Task<IActionResult> GetViewModel(Guid id, CancellationToken cancellationToken)
    {
        var table = await _tableService.GetViewModelAsync(id, cancellationToken);
        return table is null ? NotFound() : Ok(table);
    }

    [HttpGet("{id:guid}/total")]
    public async Task<IActionResult> GetTotal(Guid id, CancellationToken cancellationToken)
    {
        var total = await _tableService.GetTotalAsync(id, cancellationToken);
        return total is null ? NotFound() : Ok(total);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddTableRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var (success, error, response) = await _tableService.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = "placeId", reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTableRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _tableService.UpdateAsync(id, request, cancellationToken);
        if (!success)
        {
            return error == "Table não encontrada."
                ? NotFound()
                : BadRequest(new { errors = new[] { new { name = "placeId", reason = error } } });
        }

        return NoContent();
    }

    [HttpPut("{id:guid}/free")]
    public async Task<IActionResult> Free(Guid id, CancellationToken cancellationToken)
    {
        var freed = await _tableService.FreeAsync(id, cancellationToken);
        return freed ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/reserve")]
    public async Task<IActionResult> Reserve(Guid id, CancellationToken cancellationToken)
    {
        var reserved = await _tableService.ReserveAsync(id, cancellationToken);
        return reserved ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _tableService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
