using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Reports.SystemAudits
{
    public class GetSystemAuditsReportPdfQuery : IRequest<ErrorOr<TempFile>>
    {
        public Guid TerminalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
