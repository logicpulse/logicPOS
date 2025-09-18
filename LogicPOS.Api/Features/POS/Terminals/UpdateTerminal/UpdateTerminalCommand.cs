using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.UpdateTerminal
{
    public class UpdateTerminalCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid? NewPrinterId { get; set; }
        public Guid? NewWeighingMachineId { get; set; }
        public Guid? NewPlaceId { get; set; }
        public Guid? NewThermalPrinterId { get; set; }
        public Guid? NewBarcodeReaderId { get; set; }
        public Guid? NewCardReaderId { get; set; }
        public Guid? NewPoleDisplayId { get; set; }
        public uint? NewTimerInterval { get; set; }
        public string NewNotes { get; set; }
    }
}
