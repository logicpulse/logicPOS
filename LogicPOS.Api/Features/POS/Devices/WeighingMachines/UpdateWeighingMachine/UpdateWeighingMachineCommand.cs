using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WeighingMachines.UpdateWeighingMachine
{
    public class UpdateWeighingMachineCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string PortName { get; set; }
        public uint BaudRate { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public uint DataBits { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
