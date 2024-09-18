using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PoleDisplays;
using System;

namespace LogicPOS.Api.Entities
{
    public class Terminal : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }

        public Printer Printer { get; set; }
        public Guid? PrinterId { get; set; }

        public WeighingMachine WeighingMachine { get; set; }
        public Guid? WeighingMachineId { get; set; }

        public Place Place { get; set; }
        public Guid? PlaceId { get; set; }

        public Printer ThermalPrinter { get; set; }
        public Guid? ThermalPrinterId { get; set; }

        public InputReader BarcodeReader { get; set; }
        public Guid? BarcodeReaderId { get; set; }

        public InputReader CardReader { get; set; }
        public Guid? CardReaderId { get; set; }

        public PoleDisplay PoleDisplay { get; set; }
        public Guid? PoleDisplayId { get; set; }

        public string HardwareId { get; set; }

        public uint TimerInterval { get; set; }
    }
}
