using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;

namespace LogicPOS.Api.Features.Terminals.GetTerminalByHardwareId
{
    public class GetTerminalByHardwareIdQuery : IRequest<ErrorOr<Terminal>>
    {
        public string HardwareId { get; set; }

        public GetTerminalByHardwareIdQuery(string hardwareId)
        {
            HardwareId = hardwareId;
        }
    }
}
