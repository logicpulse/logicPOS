using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("orders")]
[Route("api/orders")]
public sealed class OrdersController : ApiControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOpenOrders([FromQuery] Guid? tableId, CancellationToken cancellationToken)
    {
        return Ok(await _orderService.GetOpenOrdersAsync(tableId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error, response) = await _orderService.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("{orderId:guid}/tickets")]
    public async Task<IActionResult> AddTicket(Guid orderId, [FromBody] AddTicketRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error, response) = await _orderService.AddTicketAsync(orderId, request, cancellationToken);
        if (!success)
        {
            return error == "Order não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{orderId:guid}/change-table")]
    public async Task<IActionResult> ChangeTable(Guid orderId, [FromBody] ChangeOrderTableRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _orderService.ChangeTableAsync(orderId, request, cancellationToken);
        if (!success)
        {
            return error == "Order não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = "newTableId", reason = error } } });
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/move-item")]
    public async Task<IActionResult> MoveItem(Guid orderId, [FromBody] MoveTicketItemRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _orderService.MoveTicketItemAsync(orderId, request, cancellationToken);
        if (!success)
        {
            return error == "Order não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/reduce-items")]
    public async Task<IActionResult> ReduceItems(Guid orderId, [FromBody] ReduceOrderItemsRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _orderService.ReduceItemsAsync(orderId, request, cancellationToken);
        if (!success)
        {
            return error == "Order não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }

    [HttpPut("{orderId:guid}/split-ticket")]
    public async Task<IActionResult> SplitTicket(Guid orderId, [FromBody] SplitTicketRequest request, CancellationToken cancellationToken)
    {
        var (success, field, error) = await _orderService.SplitTicketAsync(orderId, request, cancellationToken);
        if (!success)
        {
            return error == "Order não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = field, reason = error } } });
        }

        return NoContent();
    }

    // Two client operations share this one route: a plain DELETE closes the order (finalizes it, e.g. to
    // proceed to payment), while DELETE ?Delete=true&Reason=... cancels it with an audit reason instead.
    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> CloseOrDelete(Guid orderId, [FromQuery(Name = "Delete")] bool? delete, [FromQuery(Name = "Reason")] string? reason, CancellationToken cancellationToken)
    {
        if (delete == true)
        {
            var deleted = await _orderService.DeleteAsync(orderId, reason, cancellationToken);
            return deleted ? Ok(true) : NotFound();
        }

        var closed = await _orderService.CloseAsync(orderId, cancellationToken);
        return closed ? Ok(true) : NotFound();
    }
}
