using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class Currency : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Acronym { get; set; }
        public string Symbol { get; set; }
        public string Entity { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
