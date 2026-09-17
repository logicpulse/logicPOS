using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("licensing")]
[Route("api/licensing")]
public sealed class LicensingController : ApiControllerBase
{
    private readonly LicensingService _licensingService;
    private readonly SystemVersionService _systemVersionService;

    public LicensingController(LicensingService licensingService, SystemVersionService systemVersionService)
    {
        _licensingService = licensingService;
        _systemVersionService = systemVersionService;
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        await _licensingService.RefreshAsync(cancellationToken);
        return Ok(new { });
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetData(CancellationToken cancellationToken)
    {
        return Ok(await _licensingService.GetDataAsync(cancellationToken));
    }

    [HttpGet("hardware-id")]
    public async Task<IActionResult> GetHardwareId(CancellationToken cancellationToken)
    {
        return Ok(await _licensingService.GetHardwareIdAsync(cancellationToken));
    }

    [HttpGet("connect")]
    public IActionResult Connect()
    {
        return Ok(_licensingService.GetConnectionStatus());
    }

    [HttpGet("countries")]
    public IActionResult GetCountries()
    {
        return Ok(_licensingService.GetCountries());
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateLicenseRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _licensingService.ActivateAsync(request, cancellationToken));
    }

    [HttpGet("system/lastest-version")]
    public IActionResult GetLatestVersion()
    {
        return Ok(new LatestVersionResponse
        {
            Version = _systemVersionService.GetApiVersion()
        });
    }
}
