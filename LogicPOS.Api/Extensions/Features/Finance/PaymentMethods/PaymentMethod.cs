using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class PaymentMethod : ApiEntity, IWithDesignation, IWithCode
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Token { get; set; }
        public string Acronym { get; set; }

    }
}
