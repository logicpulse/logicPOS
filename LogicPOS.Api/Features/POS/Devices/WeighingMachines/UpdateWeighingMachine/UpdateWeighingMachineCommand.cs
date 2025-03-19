using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WeighingMachines.UpdateWeighingMachine
{
    public class UpdateWeighingMachineCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewPortName { get; set; }
        public uint NewBaudRate { get; set; }
        public string NewParity { get; set; }
        public string NewStopBits { get; set; }
        public uint NewDataBits { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }

    }
}
