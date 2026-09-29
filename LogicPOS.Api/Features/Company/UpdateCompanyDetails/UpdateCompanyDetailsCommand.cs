using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Company.UpdateCompanyDetails
{
    public class UpdateCompanyDetailsCommand : IRequest<ErrorOr<Success>>
    {
        public string CompanyName { get; set; } 
        public string BusinessName { get; set; } 
        public string FiscalNumber { get; set; }
        public string CountryCode2 { get; set; } 
        public string TaxEntity { get; set; } 
        public string City { get; set; } 
        public string Address { get; set; } 
        public string StockCapital { get; set; } 
        public string PostalCode { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Website { get; set; }
        public string Fax { get; set; }
        public string Country { get; set; } 
    }
}
