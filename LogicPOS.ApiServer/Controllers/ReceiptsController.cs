using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("receipts")]
[Route("api/receipts")]
public sealed class ReceiptsController : ApiControllerBase
{
    private readonly ReceiptService _receiptService;

    public ReceiptsController(ReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery(Name = "CustomerId")] Guid? customerId, [FromQuery(Name = "PaymentMethodId")] Guid? paymentMethodId, CancellationToken cancellationToken)
    {
        return Ok(await _receiptService.GetPagedAsync(page ?? 1, pageSize ?? 25, customerId, paymentMethodId, cancellationToken));
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelDocumentRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _receiptService.CancelAsync(id, request.Reason, cancellationToken);
        if (!success)
        {
            return error == "Document não encontrado." ? NotFound() : BadRequest(new { errors = new[] { new { name = "id", reason = error } } });
        }

        return NoContent();
    }
}
