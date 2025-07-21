using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Classes.UpdateArticleClass
{
    public class UpdateArticleClassCommandHandler :
        RequestHandler<UpdateArticleClassCommand, ErrorOr<Unit>>
    {
        public UpdateArticleClassCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateArticleClassCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/articles/classes/{command.Id}", command, cancellationToken);
        }
    }
}
