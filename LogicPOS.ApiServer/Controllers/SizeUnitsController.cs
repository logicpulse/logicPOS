using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles/sizeunits")]
[Route("api/articles/sizeunits")]
public sealed class SizeUnitsController : ApiControllerBase
{
    private readonly SizeUnitService _service;

    public SizeUnitsController(SizeUnitService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddSizeUnitRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        return StatusCode(StatusCodes.Status201Created, await _service.CreateAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSizeUnitRequest request, CancellationToken cancellationToken)
    {
        return await _service.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _service.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }
}
