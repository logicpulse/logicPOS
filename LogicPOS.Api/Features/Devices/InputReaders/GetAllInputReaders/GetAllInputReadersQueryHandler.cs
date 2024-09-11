using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.InputReaders.GetAllInputReaders
{
    public class GetAllInputReadersQueryHandler :
        RequestHandler<GetAllInputReadersQuery, ErrorOr<IEnumerable<InputReader>>>
    {
        public GetAllInputReadersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<InputReader>>> Handle(GetAllInputReadersQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<InputReader>("inputreaders", cancellationToken);
        }
    }
}
