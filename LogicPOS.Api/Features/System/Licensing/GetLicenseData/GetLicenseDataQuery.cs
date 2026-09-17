using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseData
{
    public class GetLicenseDataQuery : IRequest<ErrorOr<GetLicenseDataResponse>>
    {

    }
}
