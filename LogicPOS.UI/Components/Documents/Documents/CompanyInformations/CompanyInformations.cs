

using FastReport.Cloud.OAuth;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace LogicPOS.UI
{
    public class CompanyInformations
    {
        public CompanyInformations()
        {
            var companyInformations = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetAllPreferenceParametersQuery()).Result.Value.Where(p => p.FormType == 1).ToList();
            Name = companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_NAME")).Value;
            BusinessName= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_BUSINESS_NAME")).Value;
//            ComercialName = (companyInformations.FirstOrDefault(p => p.Token.Contains("TICKET_PRINT_COMERCIAL_NAME")).Value == null) ? string.Empty:companyInformations.FirstOrDefault(p => p.Token.Contains("TICKET_PRINT_COMERCIAL_NAME")).Value;
            Logo = companyInformations.FirstOrDefault(p => p.Token.Contains("REPORT_FILENAME_LOGO"))?.Value;
            LogoBmp = companyInformations.FirstOrDefault(p => p.Token.Contains("TICKET_FILENAME_LOGO")).Value;
            Address = companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_ADDRESS")).Value;
            City=companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_CITY")).Value;
            Phone= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_TELEPHONE")).Value;
            MobilePhone= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_MOBILEPHONE")).Value;
            Email= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_EMAIL")).Value;
            Website= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_WEBSITE")).Value;
            FiscalNumber= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_FISCALNUMBER")).Value;
            StockCapital= companyInformations.FirstOrDefault(p => p.Token.Contains("COMPANY_STOCK_CAPITAL")).Value;
            DocumentFinalLine1 = companyInformations.FirstOrDefault(p => p.Token.Contains("REPORT_FOOTER_LINE1")).Value;
            DocumentFinalLine2 = companyInformations.FirstOrDefault(p => p.Token.Contains("REPORT_FOOTER_LINE2")).Value;
            TicketFinalLine1 = companyInformations.FirstOrDefault(p => p.Token.Contains("TICKET_FOOTER_LINE1")).Value;
            TicketFinalLine2 = companyInformations.FirstOrDefault(p => p.Token.Contains("TICKET_FOOTER_LINE2")).Value;
        }

        public string Name { get; set; }
        public string ComercialName {  get; set; }
        public string BusinessName { get; set; }
        public string Logo { get; set; }
        public string LogoBmp { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string FiscalNumber { get; set; }
        public string StockCapital { get; set; }
        public string DocumentFinalLine1 { get; set; }
        public string DocumentFinalLine2 { get; set; }
        public string TicketFinalLine1 { get; set; }
        public string TicketFinalLine2 { get; set; }
    }
}