using ErrorOr;
using MediatR;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetMonthlySalesReportData
{
    public class GetMonthlySalesReportDataQuery : IRequest<ErrorOr<MonthlySalesReportData>>
    {
        public int Year { get; set; }

        public GetMonthlySalesReportDataQuery(int year) => Year = year;

        public string GetUrlQuery()
        {
            return $"?year={Year}";

        }
    }
}
