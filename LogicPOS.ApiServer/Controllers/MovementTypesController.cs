using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("movementtypes")]
[Route("api/movementtypes")]
public sealed class MovementTypesController : ApiControllerBase
{
    private readonly MovementTypeService _movementTypeService;

    public MovementTypesController(MovementTypeService movementTypeService)
    {
        _movementTypeService = movementTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _movementTypeService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddMovementTypeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var created = await _movementTypeService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMovementTypeRequest request, CancellationToken cancellationToken)
    {
        var updated = await _movementTypeService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _movementTypeService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
