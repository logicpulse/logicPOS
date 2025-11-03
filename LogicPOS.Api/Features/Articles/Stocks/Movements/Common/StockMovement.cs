using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class StockMovement : ApiEntity
    {
        public string DocumentNumber { get; set; }
        public string ExternalDocument { get; set; }
    }
}
