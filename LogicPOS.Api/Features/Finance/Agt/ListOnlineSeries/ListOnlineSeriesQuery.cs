using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries
{
    public class ListOnlineSeriesQuery : IRequest<ErrorOr<IEnumerable<OnlineSeriesInfo>>>
    {
        public string Code { get; set; }
        public string Year { get; set; }
        public string Status { get; set; }
        public string DocumentType { get; set; }
        public string EstablishmentNumber { get; set; }

        public string GetUrlQuery()
        {
            return $"?code={Code}&year={Year}&status={Status}&documentType={DocumentType}&establishmentNumber{EstablishmentNumber}";
        }
    }
}
