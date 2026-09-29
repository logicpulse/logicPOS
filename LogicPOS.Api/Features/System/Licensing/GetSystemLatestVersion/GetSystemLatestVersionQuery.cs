using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.GetSystemLatestVersion
{
    public class GetSystemLatestVersionQuery : IRequest<ErrorOr<GetSystemLatestVersionResponse>>
    {

    }
}
