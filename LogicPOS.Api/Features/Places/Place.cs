using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class Place : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public PriceType PriceType { get; set; }
        public MovementType MovementType { get; set; }
    }
}
