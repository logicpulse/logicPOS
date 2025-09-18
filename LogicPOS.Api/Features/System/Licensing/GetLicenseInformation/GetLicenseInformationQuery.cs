using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseInformation
{
    public class GetLicenseInformationQuery : IRequest<ErrorOr<GetLicenseInformationResponse>>
    {

    }
}
