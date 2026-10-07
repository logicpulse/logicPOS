using LogicPOS.Shared.Features.Finance.Saft.Entities;

namespace LogicPOS.Application.Features.Finance.Saft.Helpers;

public interface ISoftwareInformationHelper
{
    Task<SaftSoftwareInformation> GetSoftwareInformationAsync(CancellationToken ct);
}
