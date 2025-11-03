using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Common.Responses;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf
{
    public class GetSystemAuditsReportPdfQueryHandler : RequestHandler<GetSystemAuditsReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSystemAuditsReportPdfQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetSystemAuditsReportPdfQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"reports/system-audits/pdf{request.GetUrlQuery()}"+
                                                                        $"&terminalId={request.TerminalId}");
        }
    }
}
