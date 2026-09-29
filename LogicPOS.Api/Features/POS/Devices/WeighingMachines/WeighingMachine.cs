using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class WeighingMachine : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string PortName { get; set; } 
        public uint BaudRate { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public uint DataBits { get; set; }
    }
}
