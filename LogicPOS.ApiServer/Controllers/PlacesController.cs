using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("places")]
[Route("api/places")]
public sealed class PlacesController : ApiControllerBase
{
    private readonly PlaceService _placeService;
    private readonly MovementTypeService _movementTypeService;

    public PlacesController(PlaceService placeService, MovementTypeService movementTypeService)
    {
        _placeService = placeService;
        _movementTypeService = movementTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? placeId, CancellationToken cancellationToken)
    {
        return Ok(await _placeService.GetAllAsync(placeId, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddPlaceRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var (success, error, response) = await _placeService.CreateAsync(request, _movementTypeService, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = "movementTypeId", reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlaceRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _placeService.UpdateAsync(id, request, _movementTypeService, cancellationToken);
        if (!success)
        {
            return error == "Place não encontrado."
                ? NotFound()
                : BadRequest(new { errors = new[] { new { name = "movementTypeId", reason = error } } });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _placeService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
