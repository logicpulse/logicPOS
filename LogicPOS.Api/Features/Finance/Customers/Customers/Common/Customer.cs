using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.Common
{
    public class Customer : ApiEntity, IWithCode, IWithName
    {
        #region  Relations
        public RelatedEntity CustomerType { get; set; }
        public RelatedEntity PriceType { get; set; }
        public Country Country { get; set; }
        public RelatedEntity? DiscountGroup { get; set; }
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
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string FiscalNumber { get; set; } 
        public string CardNumber { get; set; }
        public decimal Discount { get; set; }
        public decimal CardCredit { get; set; }
        public bool Supplier { get; set; }
        public bool IsFinalConsumer => Name == "Consumidor Final";
    }

    public struct RelatedEntity 
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
    }

    public struct Country 
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
        public string Code2 { get; set; }
    }
}
