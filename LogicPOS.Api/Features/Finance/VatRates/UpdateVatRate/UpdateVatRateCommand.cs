using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.VatRates.UpdateVatRate
{
    public class UpdateVatRateCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public decimal Value { get; set; }
        public string ReasonCode { get; set; }
        public string TaxType { get; set; }
        public string TaxCode { get; set; }
        public string CountryRegionCode { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
