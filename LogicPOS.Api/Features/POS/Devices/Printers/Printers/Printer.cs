using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Printer : ApiEntity, IWithCode, IWithDesignation
    {
        public const int DefaultThermalMaxCharsPerLineNormal = 48;
        public const int DefaultThermalMaxCharsPerLineNormalBold = 44;
        public const int DefaultThermalMaxCharsPerLineSmall = 64;

        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid TypeId { get; set; }
        public string NetworkName { get; set; }
        public PrinterType Type { get; set; }
        public int? ThermalMaxCharsPerLineNormal { get; set; }

        public int? ThermalMaxCharsPerLineNormalBold { get; set; }

        public int? ThermalMaxCharsPerLineSmall { get; set; }
    }
}
