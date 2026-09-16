using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

// The client is inconsistent about the route: Add/Update/Delete call "vatrates" but GetAll calls
// "vat-rates" (LogicPOS.Api/Features/Finance/VatRates/GetAllVatRates/GetAllVatRatesQueryHandler.cs).
// Both are registered here so either works.
[AllowAnonymous]
[Route("vatrates")]
[Route("vat-rates")]
[Route("api/vatrates")]
[Route("api/vat-rates")]
public sealed class VatRatesController : ApiControllerBase
{
    private readonly VatRateService _service;

    public VatRatesController(VatRateService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddVatRateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        return StatusCode(StatusCodes.Status201Created, await _service.CreateAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVatRateRequest request, CancellationToken cancellationToken)
    {
        return await _service.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _service.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }
}
