using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.ValueObjects;
using System;

namespace LogicPOS.Api.Entities
{
    public class ArticleFamily : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }

        public CommissionGroup CommissionGroup { get; set; }
        public Guid? CommissionGroupId { get; set; }

        public DiscountGroup DiscountGroup { get; set; }
        public Guid? DiscountGroupId { get; set; }

        public Printer Printer { get; set; }
        public Guid? PrinterId { get; set; }

        public Button Button { get; set; }
    }
}
