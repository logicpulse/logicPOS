using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.RefreshLicense
{
    public class RefreshLicenseCommand : IRequest<ErrorOr<Success>>
    {
    }
}
