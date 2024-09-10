using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Types.AddArticleType
{
    public class AddArticleTypeCommandHandler : RequestHandler<AddArticleTypeCommand, ErrorOr<Guid>>
    {
        public AddArticleTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddArticleTypeCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("article/types", command, cancellationToken);
        }
    }
}
