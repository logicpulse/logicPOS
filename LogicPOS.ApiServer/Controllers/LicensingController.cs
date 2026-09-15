using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("licensing/system")]
[Route("api/licensing/system")]
public sealed class LicensingController : ApiControllerBase
{
    private readonly SystemVersionService _systemVersionService;

    public LicensingController(SystemVersionService systemVersionService)
    {
        _systemVersionService = systemVersionService;
    }

    [HttpGet("lastest-version")]
    public IActionResult GetLatestVersion()
    {
        return Ok(new LatestVersionResponse
        {
            Version = _systemVersionService.GetApiVersion()
        });
    }
}
