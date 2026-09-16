using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles/warehouse-articles")]
[Route("api/articles/warehouse-articles")]
public sealed class WarehouseArticlesController : ApiControllerBase
{
    private readonly WarehouseArticleService _warehouseArticleService;

    public WarehouseArticlesController(WarehouseArticleService warehouseArticleService)
    {
        _warehouseArticleService = warehouseArticleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] Guid? articleId, CancellationToken cancellationToken)
    {
        return Ok(await _warehouseArticleService.GetPagedAsync(page ?? 1, pageSize ?? 25, articleId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _warehouseArticleService.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _warehouseArticleService.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }
}
