using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogicPOS.ApiServer.Services;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("system")]
[Route("api/system")]
public sealed class SystemController : ApiControllerBase
{
    private readonly SystemVersionService _systemVersionService;
    private readonly ApiSystemInformationService _apiSystemInformationService;

    public SystemController(
        SystemVersionService systemVersionService,
        ApiSystemInformationService apiSystemInformationService)
    {
        _systemVersionService = systemVersionService;
        _apiSystemInformationService = apiSystemInformationService;
    }

    [HttpGet("api-version")]
    public IActionResult GetApiVersion()
    {
        return Ok(_systemVersionService.GetApiVersion());
    }

    [HttpGet("information")]
    public IActionResult GetInformation()
    {
        return Ok(_apiSystemInformationService.GetSystemInformation());
    }
}
