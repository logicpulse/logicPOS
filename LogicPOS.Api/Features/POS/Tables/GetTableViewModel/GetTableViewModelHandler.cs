using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Tables.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetTableViewModel
{
    public class GetTableViewModelQueryHandler :
        RequestHandler<GetTableViewModelQuery, ErrorOr<TableViewModel>>
    {
        public GetTableViewModelQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TableViewModel>> Handle(GetTableViewModelQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<TableViewModel>($"tables/{query.Id}/view", cancellationToken);
        }
    }
}
