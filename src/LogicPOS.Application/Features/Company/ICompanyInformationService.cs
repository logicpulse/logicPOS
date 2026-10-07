using LogicPOS.Shared.Features.Company;

namespace LogicPOS.Application.Features.Company;

public interface ICompanyInformationService
{
    public Task<CompanyInformation> GetCompanyInformationAsync(CancellationToken ct = default);
}