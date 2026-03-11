using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.GetArticlePrinter
{
    public class GetArticlePrinterQueryHandler :
        RequestHandler<GetArticlePrinterQuery, ErrorOr<Printer>>
    {
        public GetArticlePrinterQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Printer>> Handle(GetArticlePrinterQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<Printer>($"articles/{query.Id}/printer", cancellationToken);
        }
    }
}
