using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.AddCustomer
{
    public class AddCustomerCommand : IRequest<ErrorOr<Guid>>
    {
        public Guid CustomerTypeId { get; set; }
        public Guid? DiscountGroupId { get; set; }
        public Guid PriceTypeId { get; set; }
        public Guid CountryId { get; set; }
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
        public decimal Discount { get; set; }
        public decimal CardCredit { get; set; }
        public bool Supplier { get; set; }
        public string Notes { get; set; }
    }
}
