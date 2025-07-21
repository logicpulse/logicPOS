using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.AddArticle
{
    public class AddArticleCommandHandler :
        RequestHandler<AddArticleCommand, ErrorOr<Guid>>
    {
        public AddArticleCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles", request, cancellationToken);
        }
    }
}
