using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.StockManagement.UpdateUniqueArticle
{
    public class UpdateUniqueArticleCommandHandler :
        RequestHandler<UpdateUniqueArticleCommand, ErrorOr<Unit>>
    {
        public UpdateUniqueArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Unit>> Handle(UpdateUniqueArticleCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/uniques/{command.Id}", command, cancellationToken);
        }
    }
}
