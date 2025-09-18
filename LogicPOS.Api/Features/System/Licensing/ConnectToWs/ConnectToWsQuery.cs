using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.ConnectToWs
{
    public class ConnectToWsQuery : IRequest<ErrorOr<ConnectToWsResponse>>
    {
    }
}
