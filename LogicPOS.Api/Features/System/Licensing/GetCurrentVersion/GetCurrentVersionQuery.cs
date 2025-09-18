using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetCurrentVersion
{
    public class GetCurrentVersionQuery : IRequest<ErrorOr<GetCurrentVersionResponse>>
    {
    }
}
