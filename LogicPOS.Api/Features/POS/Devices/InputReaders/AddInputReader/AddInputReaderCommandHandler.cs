using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.InputReaders.AddInputReader
{
    public class AddInputReaderCommandHandler
        : RequestHandler<AddInputReaderCommand, ErrorOr<Guid>>
    {
        public AddInputReaderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddInputReaderCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("inputreaders", command, cancellationToken);
        }
    }
}
