using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.InputReaders.UpdateInputReader
{
    public class UpdateInputReaderCommandHandler
         : RequestHandler<UpdateInputReaderCommand, ErrorOr<Success>>
    {
        public UpdateInputReaderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateInputReaderCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"inputreaders/{command.Id}", command, cancellationToken);
        }
    }
}
