using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("documents")]
[Route("api/documents")]
public sealed class DocumentsController : ApiControllerBase
{
    private readonly DocumentService _documentService;
    private readonly DocumentTypeService _documentTypeService;
    private readonly DocumentSeriesService _documentSeriesService;

    public DocumentsController(DocumentService documentService, DocumentTypeService documentTypeService, DocumentSeriesService documentSeriesService)
    {
        _documentService = documentService;
        _documentTypeService = documentTypeService;
        _documentSeriesService = documentSeriesService;
    }

    // --- Document types (seeded catalog: read + print-setting updates only) ---

    [HttpGet("types")]
    public async Task<IActionResult> GetTypes(CancellationToken cancellationToken) => Ok(await _documentTypeService.GetAllAsync(cancellationToken));

    [HttpGet("types/active")]
    public async Task<IActionResult> GetActiveTypes(CancellationToken cancellationToken) => Ok(await _documentTypeService.GetActiveAsync(cancellationToken));

    [HttpPut("types/{id:guid}")]
    public async Task<IActionResult> UpdateType(Guid id, [FromBody] UpdateDocumentTypeRequest request, CancellationToken cancellationToken)
    {
        return await _documentTypeService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    // --- Document series (sequential numbering) ---

    [HttpGet("series/active")]
    public async Task<IActionResult> GetActiveSeries(CancellationToken cancellationToken) => Ok(await _documentSeriesService.GetActiveAsync(cancellationToken));

    [HttpGet("series/has-active")]
    public async Task<IActionResult> HasActiveSeries([FromQuery] string? documentType, CancellationToken cancellationToken)
    {
        return Ok(await _documentSeriesService.HasActiveAsync(documentType, cancellationToken));
    }

    [HttpPost("series")]
    public async Task<IActionResult> CreateSeries([FromBody] CreateDocumentSeriesRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var (success, field, error, response) = await _documentSeriesService.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("series/{id:guid}")]
    public async Task<IActionResult> UpdateSeries(Guid id, [FromBody] UpdateDocumentSeriesRequest request, CancellationToken cancellationToken)
    {
        return await _documentSeriesService.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();
    }

    // --- Documents ---

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? page, [FromQuery] int? pageSize,
        [FromQuery] string[]? types, [FromQuery] Guid? customerId, [FromQuery] Guid? paymentMethodId,
        [FromQuery] int? paymentStatus, [FromQuery] char? status,
        CancellationToken cancellationToken)
    {
        string? paymentStatusName = paymentStatus switch { 1 => "Paid", 2 => "Unpaid", _ => null };
        return Ok(await _documentService.GetPagedAsync(page ?? 1, pageSize ?? 25, types, customerId, paymentMethodId, paymentStatusName, status, cancellationToken));
    }

    [HttpGet("unpaid-for-settlement")]
    public async Task<IActionResult> GetUnsettledForSettlement([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] Guid? customerId, CancellationToken cancellationToken)
    {
        return Ok(await _documentService.GetUnsettledForSettlementAsync(page ?? 1, pageSize ?? 25, customerId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var document = await _documentService.GetByIdAsync(id, cancellationToken);
        return document is null ? NotFound() : Ok(document);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        var details = await _documentService.GetDetailsAsync(id, cancellationToken);
        return details is null ? NotFound() : Ok(details);
    }

    [HttpGet("{id:guid}/from-order")]
    public async Task<IActionResult> IsFromOrder(Guid id, CancellationToken cancellationToken)
    {
        var isFromOrder = await _documentService.IsFromOrderAsync(id, cancellationToken);
        return isFromOrder is null ? NotFound() : Ok(isFromOrder.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Issue([FromBody] IssueDocumentRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error, response) = await _documentService.IssueAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelDocumentRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _documentService.CancelAsync(id, request.Reason, cancellationToken);
        if (!success)
        {
            return error == "Document não encontrado." ? NotFound() : BadRequest(new { errors = new[] { new { name = "id", reason = error } } });
        }

        return NoContent();
    }

    [HttpDelete("draft/{id:guid}")]
    public async Task<IActionResult> DeleteDraft(Guid id, CancellationToken cancellationToken)
    {
        return await _documentService.DeleteDraftAsync(id, cancellationToken) ? Ok() : NotFound();
    }

    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayDocumentsRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error, response) = await _documentService.PayAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
