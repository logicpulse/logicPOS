using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.GetApiVersion
{
    public class GetApiVersionQuery : IRequest<ErrorOr<string>>
    {
    }
}
