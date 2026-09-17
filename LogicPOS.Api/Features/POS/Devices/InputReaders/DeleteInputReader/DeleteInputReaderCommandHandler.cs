using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.InputReaders.DeleteInputReader
{
    public class DeleteInputReaderCommandHandler :
        RequestHandler<DeleteInputReaderCommand, ErrorOr<bool>>
    {
        public DeleteInputReaderCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DeleteInputReaderCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"inputreaders/{command.Id}", cancellationToken);
        }
    }
}
