using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles/subfamilies")]
[Route("api/articles/subfamilies")]
public sealed class ArticleSubfamiliesController : ApiControllerBase
{
    private readonly ArticleSubfamilyService _service;

    public ArticleSubfamiliesController(ArticleSubfamilyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? familyId, CancellationToken cancellationToken) => Ok(await _service.GetAllAsync(familyId, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddArticleSubfamilyRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Designation))
        {
            return BadRequest(new { errors = new[] { new { name = "designation", reason = "Designation é obrigatório." } } });
        }

        var (success, error, response) = await _service.CreateAsync(request, cancellationToken);
        if (!success)
        {
            return BadRequest(new { errors = new[] { new { name = "familyId", reason = error } } });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleSubfamilyRequest request, CancellationToken cancellationToken)
    {
        var (success, error) = await _service.UpdateAsync(id, request, cancellationToken);
        if (!success)
        {
            return error == "Subfamily não encontrada." ? NotFound() : BadRequest(new { errors = new[] { new { name = "familyId", reason = error } } });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _service.DeleteAsync(id, cancellationToken) ? Ok() : NotFound();
    }
}
