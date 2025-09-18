using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.NeedToRegister
{
    public class NeedToRegisterQuery : IRequest<ErrorOr<NeedToRegisterResponse>>
    {
    }
}
