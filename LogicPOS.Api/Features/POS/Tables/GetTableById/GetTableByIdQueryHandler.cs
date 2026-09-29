using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.POS.Tables.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.GetTableById
{
    public class GetTableByIdQueryHandler :
        RequestHandler<GetTableByIdQuery, ErrorOr<Table>>
    {
        public GetTableByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Table>> Handle(GetTableByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<Table>($"tables/{query.Id}", cancellationToken);
        }
    }
}
