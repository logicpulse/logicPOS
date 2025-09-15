using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.POS.Tables.Common
{
    public class TableViewModel : ApiEntity, IWithDesignation, IWithCode
    {
        public string Code { get; set; }
        public string Place { get; set; }
        public int PriceTypeEnum { get; set; }
        public string Designation { get; set; }
        public TableStatus Status { get; set; }
        public DateTime? OpennedAt { get; set; }
        public Guid PlaceId { get; set; }
    }
}
