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

    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
    {
        return Ok(await _companyService.GetAsync(cancellationToken));
    }

    [HttpPut("details")]
    public async Task<IActionResult> UpdateDetails([FromBody] UpdateCompanyDetailsRequest request, CancellationToken cancellationToken)
    {
        await _companyService.UpdateAsync(request, cancellationToken);
        return NoContent();
    }
}
