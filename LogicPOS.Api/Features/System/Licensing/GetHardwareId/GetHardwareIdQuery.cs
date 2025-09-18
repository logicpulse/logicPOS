using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetHardwareId
{
    public class GetHardwareIdQuery : IRequest<ErrorOr<GetHardwareIdResponse>>
    {

    }
}
