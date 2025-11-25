using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.ListInvoices
{
    public class ListInvoicesQuery : IRequest<ErrorOr<IEnumerable<AgtInvoice>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ListInvoicesQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public string GetUrlQuery()
        {
            return $"?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
        }
    }
}
