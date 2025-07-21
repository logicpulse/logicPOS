using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.GetUserNameById
{
    public class GetUserNameByIdQueryHandler :
       RequestHandler<GetUserNameByIdQuery, ErrorOr<string>>
    {
        public GetUserNameByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<string>> Handle(GetUserNameByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<string>($"users/{query.Id}/name", cancellationToken);
        }
    }
}
