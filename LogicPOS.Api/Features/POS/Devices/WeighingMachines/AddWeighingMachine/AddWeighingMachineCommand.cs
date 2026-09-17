using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WeighingMachines.AddWeighingMachine
{
    public class AddWeighingMachineCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string ButtonImage { get; set; }
        public string PortName { get; set; }
        public uint BaudRate { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public uint DataBits { get; set; }
        public string Notes { get; set; }
    }
}
