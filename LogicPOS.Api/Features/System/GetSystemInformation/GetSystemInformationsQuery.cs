using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.GetSystemInformations
{
    public class GetSystemInformationsQuery : IRequest<ErrorOr<SystemInformation>>
    {
    }
}
