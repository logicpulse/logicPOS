using LogicPOS.Api.Features.Common;
using LogicPOS.Api.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System;

namespace LogicPOS.Api.Entities
{
    public class ArticleSubfamily : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }

        public ArticleFamily Family { get; set; }
        public Guid FamilyId { get; set; }

        public CommissionGroup CommissionGroup { get; set; }
        public Guid? CommissionGroupId { get; set; }

        public DiscountGroup DiscountGroup { get; set; }
        public Guid? DiscountGroupId { get; set; }

        public VatRate VatOnTable { get; set; }
        public Guid? VatOnTableId { get; set; }

        public VatRate VatDirectSelling { get; set; }
        public Guid? VatDirectSellingId { get; set; }

        public Button Button { get; set; }

        public Printer Printer { get; set; }
        public Guid? PrinterId { get; set; }
    }
}
