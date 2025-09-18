using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetLicenseFilename
{
    public class GetLicenseFilenameQuery : IRequest<ErrorOr<GetLicenseFilenameResponse>>
    {
    }
}
