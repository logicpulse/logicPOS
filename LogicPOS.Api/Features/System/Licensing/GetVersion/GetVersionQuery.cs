using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetVersion
{
    public class GetVersionQuery : IRequest<ErrorOr<GetVersionResponse>>
    {

    }
}
