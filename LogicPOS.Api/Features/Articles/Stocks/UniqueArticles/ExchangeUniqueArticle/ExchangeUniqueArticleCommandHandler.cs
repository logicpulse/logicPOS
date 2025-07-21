using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.ExchangeUniqueArticle
{
    public class ExchangeUniqueArticleCommandHandler :
        RequestHandler<ExchangeUniqueArticleCommand, ErrorOr<Unit>>
    {
        public ExchangeUniqueArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Unit>> Handle(ExchangeUniqueArticleCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync("articles/uniques/exchange", command, cancellationToken);
        }
    }
}
