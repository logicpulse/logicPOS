using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("countries")]
[Route("api/countries")]
public sealed class CountriesController : ApiControllerBase
{
    private readonly ReferenceDataService _referenceDataService;

    public CountriesController(ReferenceDataService referenceDataService)
    {
        _referenceDataService = referenceDataService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_referenceDataService.GetCountries());
    }
}
