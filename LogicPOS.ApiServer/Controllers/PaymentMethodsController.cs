using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

// The client is inconsistent about the route: Add/Update/GetAll call "payment/methods" but Delete calls
// "payment-methods" (LogicPOS.Api/Features/Finance/PaymentMethods/DeletePaymentMethod). Both are registered.
[AllowAnonymous]
[Route("payment/methods")]
[Route("payment-methods")]
[Route("api/payment/methods")]
[Route("api/payment-methods")]
public sealed class PaymentMethodsController : ApiControllerBase
{
    private readonly PaymentMethodService _service;

    public PaymentMethodsController(PaymentMethodService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok(await _service.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        return StatusCode(StatusCodes.Status201Created, await _service.CreateAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentMethodRequest request, CancellationToken cancellationToken)
    {
        return await _service.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _service.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }
}
