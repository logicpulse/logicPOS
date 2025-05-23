using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Tables.AddTable
{
    public class AddTableCommandHandler
        : RequestHandler<AddTableCommand, ErrorOr<Guid>>
    {
        public AddTableCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddTableCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("tables", command, cancellationToken);
        }
    }
}
