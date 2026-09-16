using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("holidays")]
[Route("api/holidays")]
public sealed class HolidaysController : ApiControllerBase
{
    private readonly HolidayService _holidayService;

    public HolidaysController(HolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _holidayService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddHolidayRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var created = await _holidayService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHolidayRequest request, CancellationToken cancellationToken)
    {
        var updated = await _holidayService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _holidayService.DeleteAsync(id, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
