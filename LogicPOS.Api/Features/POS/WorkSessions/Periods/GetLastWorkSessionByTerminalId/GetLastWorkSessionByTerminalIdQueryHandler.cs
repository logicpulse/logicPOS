using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.WorkSessions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticleById
{
    public class GetLastWorkSessionByTerminalIdQueryHandler :
        RequestHandler<GetLastWorkSessionByTerminalIdQuery, ErrorOr<WorkSessionPeriod>>
    {
        public GetLastWorkSessionByTerminalIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<WorkSessionPeriod>> Handle(GetLastWorkSessionByTerminalIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<WorkSessionPeriod>($"worksession/period/terminal/{query.TerminalId}", cancellationToken);
        }
    }
}
