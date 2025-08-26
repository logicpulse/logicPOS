using ErrorOr;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.WorkSession.GetWorkSessionData
{
    public class GetWorkSessionDocumentsDataQuery : IRequest<ErrorOr<WorkSessionData>>
    {


        public GetWorkSessionDocumentsDataQuery(Guid dayId)
        {
            DayId = dayId;
        }

        public Guid DayId { get; set; }
    }
}
