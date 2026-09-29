using ErrorOr;
using LogicPOS.Api.Extensions;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.SalesByDate.GetSalesTotalForDay
{
    public class GetSalesTotalForDayQuery : IRequest<ErrorOr<TotalSalesForDay>>
    {
        public DateTime Day { get; set; }

        public GetSalesTotalForDayQuery(DateTime day) => Day = day;

        public string GetUrlQuery()
        {
            return $"?Day={Day.ToISO8601DateOnly()}";

        }
    }
}
