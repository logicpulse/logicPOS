using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogicPOS.ApiServer.Services;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("api/system")]
public sealed class SystemController : ApiControllerBase
{
    private readonly SystemVersionService _systemVersionService;

    public SystemController(SystemVersionService systemVersionService)
    {
        _systemVersionService = systemVersionService;
    }

    [HttpGet("api-version")]
    public IActionResult GetApiVersion()
    {
        return Ok(_systemVersionService.GetApiVersion());
    }
}
