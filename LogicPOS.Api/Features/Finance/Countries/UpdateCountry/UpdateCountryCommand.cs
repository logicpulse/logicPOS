using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Countries.UpdateCountry
{
    public class UpdateCountryCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewCode2 { get; set; }
        public string NewCode3 { get; set; }
        public string NewCapital { get; set; }
        public string NewCurrency { get; set; }
        public string NewCurrencyCode { get; set; }
        public string NewFiscalNumberRegex { get; set; }
        public string NewZipCodeRegex { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
