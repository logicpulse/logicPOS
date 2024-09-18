using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.AddTerminal
{
    public class AddTerminalCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string HardwareId { get; set; } 
        public uint TimerInterval { get; set; }
        public string Notes { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? PrinterId { get; set; }
        public Guid? ThermalPrinterId { get; set; }
        public Guid? PoleDisplayId { get; set; }
        public Guid? WeighingMachineId { get; set; }
        public Guid? BarcodeReaderId { get; set; }
        public Guid? CardReaderId { get; set; }
    }
}
