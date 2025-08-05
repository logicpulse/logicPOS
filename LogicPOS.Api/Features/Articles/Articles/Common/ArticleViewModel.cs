using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Common
{
    public class ArticleViewModel : ApiEntity, IWithCode, IWithDesignation
    {
        public string Code { get; set; } 
        public bool IsComposed { get; set; }
        public string Family { get; set; } 
        public string Subfamily { get; set; } 
        public string Designation { get; set; } 
        public string Type { get; set; } 
        public string ButtonLabel { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal Price { get; set; }
        public decimal? VatDirectSelling { get; set; }
        public decimal Discount { get; set; }
        public string Unit { get; set; }
        public Guid SubfamilyId { get; set; }
        public Guid FamilyId { get; set; }

        
    }
}
