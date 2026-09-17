using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.UpdateArticleChildren
{
    public class UpdateChildrenCommandHandler : RequestHandler<UpdateArticleChildrenCommand, ErrorOr<Success>>
    {
        public UpdateChildrenCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateArticleChildrenCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/{command.Id}/children", command, cancellationToken);
        }
    }
}
