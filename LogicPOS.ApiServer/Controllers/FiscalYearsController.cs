using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("fiscal-years")]
[Route("api/fiscal-years")]
public sealed class FiscalYearsController : ApiControllerBase
{
    private readonly FiscalYearService _service;

    public FiscalYearsController(FiscalYearService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _service.GetAllAsync(cancellationToken));

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var current = await _service.GetCurrentAsync(cancellationToken);
        return current is null ? NotFound() : Ok(current);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddFiscalYearRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        return StatusCode(StatusCodes.Status201Created, await _service.CreateAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, [FromBody] CloseFiscalYearRequest request, CancellationToken cancellationToken)
    {
        return await _service.CloseAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
