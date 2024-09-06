using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Features.PoleDisplays
{
    public class PoleDisplay : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string VendorId { get; set; }
        public string ProductId { get; set; }
        public string EndPoint { get; set; }
        public string CodeTable { get; set; }
        public string COMPort { get; set; }
        public uint CharactersPerLine { get; set; }
        public uint GoToStandByInSeconds { get; set; }
        public string StandByLine1 { get; set; }
        public string StandByLine2 { get; set; }
    }
}
