using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("currencies")]
[Route("api/currencies")]
public sealed class CurrenciesController : ApiControllerBase
{
    private readonly ReferenceDataService _referenceDataService;

    public CurrenciesController(ReferenceDataService referenceDataService)
    {
        _referenceDataService = referenceDataService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_referenceDataService.GetCurrencies());
    }
}
