using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.UpdateTerminal
{
    public class UpdateTerminalCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid? PrinterId { get; set; }
        public Guid? WeighingMachineId { get; set; }
        public Guid? PlaceId { get; set; }
        public Guid? ThermalPrinterId { get; set; }
        public Guid? BarcodeReaderId { get; set; }
        public Guid? CardReaderId { get; set; }
        public Guid? PoleDisplayId { get; set; }
        public uint TimerInterval { get; set; }
        public string Notes { get; set; }
    }
}
