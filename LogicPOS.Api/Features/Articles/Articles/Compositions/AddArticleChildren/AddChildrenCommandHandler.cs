using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.AddArticleChildren
{
    public class AddChildrenCommandHandler : RequestHandler<AddArticleChildrenCommand, ErrorOr<Guid>>
    {
        public AddChildrenCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddArticleChildrenCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync($"articles/{command.Id}/children", command, cancellationToken);
        }
    }
}
