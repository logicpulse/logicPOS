using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.IsLicensed
{
    public class IsLicensedQuery : IRequest<ErrorOr<IsLicensedResponse>>
    {
    }
}
