using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Subfamilies.AddArticleSubfamily
{
    public class AddArticleSubfamilyCommandHandler :
        RequestHandler<AddArticleSubfamilyCommand, ErrorOr<Guid>>
    {
        public AddArticleSubfamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleSubfamilyCommand request,
                                                         CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles/subfamilies", request, cancellationToken);
        }
    }
}
