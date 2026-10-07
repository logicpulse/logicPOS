using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ICompanyInformationHelper
{
    Task<SaftCompanyInformation> GetCompanyInformationAsync(CancellationToken ct);
}
