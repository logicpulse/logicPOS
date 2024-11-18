using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodSession
{
    public class OpenWorkSessionPeriodSessionCommand : IRequest<ErrorOr<Guid>>
    {
        public decimal? Amount { get; set; }
        public string Notes { get; set; }
    }
}
