using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("terminals")]
[Route("api/terminals")]
public sealed class TerminalsController : ApiControllerBase
{
    private readonly TerminalService _terminalService;

    public TerminalsController(TerminalService terminalService)
    {
        _terminalService = terminalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _terminalService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var terminal = await _terminalService.GetByIdAsync(id, cancellationToken);
        return terminal is null ? NotFound() : Ok(terminal);
    }

    [HttpGet("hardwareid/{hardwareId}")]
    public async Task<IActionResult> GetByHardwareId(string hardwareId, CancellationToken cancellationToken)
    {
        var terminal = await _terminalService.GetByHardwareIdAsync(hardwareId, cancellationToken);
        return terminal is null ? NotFound() : Ok(terminal);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTerminalRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.HardwareId))
        {
            return BadRequest(new { errors = new[] { new { name = "hardwareId", reason = "HardwareId é obrigatório." } } });
        }

        var created = await _terminalService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
