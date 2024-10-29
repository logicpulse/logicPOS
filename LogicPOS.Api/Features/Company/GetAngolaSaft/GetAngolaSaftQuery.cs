using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Company.GetAngolaSaft
{
    public class GetAngolaSaftQuery : IRequest<ErrorOr<byte[]>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetAngolaSaftQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
