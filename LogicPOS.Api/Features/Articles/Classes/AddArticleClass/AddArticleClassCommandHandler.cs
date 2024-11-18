using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Classes.AddArticleClass
{
    public class AddArticleClassCommandHandler : RequestHandler<AddArticleClassCommand, ErrorOr<Guid>>
    {
        public AddArticleClassCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleClassCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles/classes", command, cancellationToken);
        }
    }
}
