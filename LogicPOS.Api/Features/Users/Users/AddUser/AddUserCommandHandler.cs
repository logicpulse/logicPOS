using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Users.AddUser
{
    public class AddUserCommandHandler :
        RequestHandler<AddUserCommand, ErrorOr<Guid>>
    {
        public AddUserCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddUserCommand request, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("users", request, cancellationToken);
        }
    }
}
