using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
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
            return await HandleGetEntityQueryAsync<Table>($"tables/{query.Id}", cancellationToken);
        }
    }
}
