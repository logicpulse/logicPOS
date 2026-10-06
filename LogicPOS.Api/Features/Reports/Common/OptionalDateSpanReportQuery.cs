using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.Common
{
    public abstract class OptionalDateSpanReportQuery : IRequest<ErrorOr<TempFile>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string GetUrlQuery()
        {
            // The API only filters when both dates are sent.
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                return string.Empty;
            }

            return $"?startDate={StartDate.Value:yyyy-MM-dd}&endDate={EndDate.Value:yyyy-MM-dd}";
        }
    }
}
