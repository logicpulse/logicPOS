using ErrorOr;
using MediatR;
using LogicPOS.Api.Entities;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseInformation
{
    public class GetLicenseDataQuery : IRequest<ErrorOr<GetLicenseDataResponse>>
    {

    }
}
