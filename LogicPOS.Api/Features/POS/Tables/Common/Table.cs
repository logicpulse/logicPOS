using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.POS.Tables.Common
{
    public class Table : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Place Place { get; set; }
        public Guid PlaceId { get; set; }
        public string ButtonImage { get; set; }
        public TableStatus Status { get; set; }
        public DateTime? OpennedAt { get; set; }
    }
}
