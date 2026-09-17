using ErrorOr;
using MediatR;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using System;

namespace LogicPOS.Api.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public Guid TypeId { get; set; }
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
        public decimal Discount { get; set; }
        public CardMode CardMode { get; set; }
        public bool Supplier { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
