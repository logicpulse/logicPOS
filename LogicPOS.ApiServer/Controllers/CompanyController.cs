using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("company")]
[Route("api/company")]
public sealed class CompanyController : ApiControllerBase
{
    private readonly CompanyService _companyService;
    private readonly ReferenceDataService _referenceDataService;

    public CompanyController(CompanyService companyService, ReferenceDataService referenceDataService)
    {
        _companyService = companyService;
        _referenceDataService = referenceDataService;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
    {
        return Ok(await _companyService.GetAsync(cancellationToken));
    }

    [HttpGet("currency")]
    public async Task<IActionResult> GetCurrency(CancellationToken cancellationToken)
    {
        var company = await _companyService.GetAsync(cancellationToken);
        return Ok(_referenceDataService.GetCurrencyByCode(company.CurrencyCode));
    }

    [HttpPut("details")]
    public async Task<IActionResult> UpdateDetails([FromBody] UpdateCompanyDetailsRequest request, CancellationToken cancellationToken)
    {
        await _companyService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }
}
