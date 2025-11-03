using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.Common
{
    public abstract class StartAndEndDateReportQuery : IRequest<ErrorOr<TempFile>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public StartAndEndDateReportQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public virtual string GetUrlQuery()
        {
            return $"?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
        }
    }
}
