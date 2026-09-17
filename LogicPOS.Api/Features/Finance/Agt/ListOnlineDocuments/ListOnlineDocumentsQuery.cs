using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments
{
    public class ListOnlineDocumentsQuery : IRequest<ErrorOr<IEnumerable<OnlineDocument>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ListOnlineDocumentsQuery(DateTime startDate, DateTime endDate)
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
