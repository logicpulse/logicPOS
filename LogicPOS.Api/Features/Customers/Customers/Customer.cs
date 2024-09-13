using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class Customer : ApiEntity, IWithCode
    {
        #region  Relations
        public CustomerType CustomerType { get; set; }
        public Guid CustomerTypeId { get; set; }

        public DiscountGroup DiscountGroup { get; set; }
        public Guid? DiscountGroupId { get; set; }

        public PriceType PriceType { get; set; }
        public Guid PriceTypeId { get; set; }

        public Country Country { get; set; }
        public Guid CountryId { get; set; }
        #endregion

        public uint Order { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string FiscalNumber { get; set; } 
        public string CardNumber { get; set; }
        public string DiscountType { get; set; }
        public decimal? Discount { get; set; }
        public decimal? CardCredit { get; set; }
        public decimal? TotalDebt { get; set; }
        public decimal? TotalCredit { get; set; }
        public decimal? CurrentBalance { get; set; }
        public string CreditLine { get; set; }
        public string Remarks { get; set; }
        public bool? Supplier { get; set; }
        public bool? Hidden { get; set; }
    }
}
