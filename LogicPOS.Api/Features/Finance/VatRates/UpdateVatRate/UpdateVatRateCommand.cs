using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.VatRates.UpdateVatRate
{
    public class UpdateVatRateCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public decimal NewValue { get; set; }
        public string NewReasonCode { get; set; }
        public string NewTaxType { get; set; }
        public string NewTaxCode { get; set; }
        public string NewCountryRegionCode { get; set; }
        public DateTime NewExpirationDate { get; set; }
        public string NewDescription { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
