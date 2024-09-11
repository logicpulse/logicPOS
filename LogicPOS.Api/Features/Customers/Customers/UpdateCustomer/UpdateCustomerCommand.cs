using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public Guid? NewCustomerTypeId { get; set; }
        public Guid? NewDiscountGroupId { get; set; }
        public Guid? NewPriceTypeId { get; set; }
        public Guid? NewCountryId { get; set; }
        public string NewName { get; set; } 
        public string NewAddress { get; set; }
        public string NewLocality { get; set; }
        public string NewZipCode { get; set; }
        public string NewCity { get; set; }
        public DateTime? NewBirthDate { get; set; }
        public string NewPhone { get; set; }
        public string NewFax { get; set; }
        public string NewMobilePhone { get; set; }
        public string NewEmail { get; set; }
        public string NewWebSite { get; set; }
        public string NewFiscalNumber { get; set; }
        public string NewCardNumber { get; set; }
        public string NewDiscountType { get; set; }
        public decimal? NewDiscount { get; set; }
        public decimal? NewCardCredit { get; set; }
        public decimal? NewTotalDebt { get; set; }
        public decimal? NewTotalCredit { get; set; }
        public decimal? NewCurrentBalance { get; set; }
        public string NewCreditLine { get; set; }
        public string NewRemarks { get; set; }
        public bool? Supplier { get; set; } 
        public bool? Hidden { get; set; }
        public string NewNotes { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
