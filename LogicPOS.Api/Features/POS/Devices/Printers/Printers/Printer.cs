using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Printer : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid TypeId { get; set; }
        public string NetworkName { get; set; }
        public PrinterType Type { get; set; }
    }
}
