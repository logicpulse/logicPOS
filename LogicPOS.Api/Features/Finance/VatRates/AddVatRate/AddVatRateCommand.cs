using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.VatRates.AddVatRate
{
    public class AddVatRateCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public decimal Value { get; set; }
        public string ReasonCode { get; set; }
        public string TaxType { get; set; }
        public string TaxCode { get; set; }
        public string CountryRegionCode { get; set; } 
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    }
}
