using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Finance.Saft.GetSaft
{
    public class GetSaftQuery : IRequest<ErrorOr<TempFile>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetSaftQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
