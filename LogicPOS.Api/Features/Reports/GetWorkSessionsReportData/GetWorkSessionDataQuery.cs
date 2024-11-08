using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Reports.WorkSession.GetWorkSessionData
{
    public class GetWorkSessionDataQuery : IRequest<ErrorOr<WorkSessionData>>
    {
       

        public GetWorkSessionDataQuery(Guid id)
        {
            Id= id;
        }

        public Guid Id{ get; set;}
    }
}
