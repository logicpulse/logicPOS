using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Countries.AddCountry
{
    public class AddCountryCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Capital { get; set; }
        public string TLD { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public string FiscalNumberRegex { get; set; }
        public string ZipCodeRegex { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
