using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.System.Licensing.UpdateCurrentVersion
{
    public class UpdateCurrentVersionCommand : IRequest<ErrorOr<UpdateCurrentVersionResponse>>
    {
        public string HardwareId { get; set; }
        public string ProductId { get; set; }
        public string VersionApp { get; set; }
    }
}
