using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodDay
{
    public class OpenWorkSessionPeriodDayCommand : IRequest<ErrorOr<Guid>>
    {
        public string Notes { get; set; }
    }
}
