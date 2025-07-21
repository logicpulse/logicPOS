using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily
{
    public class UpdateArticleFamilyCommandHandler :
        RequestHandler<UpdateArticleFamilyCommand, ErrorOr<Unit>>
    {
        public UpdateArticleFamilyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateArticleFamilyCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"articles/families/{command.Id}", command, cancellationToken);
        }
    }
}
