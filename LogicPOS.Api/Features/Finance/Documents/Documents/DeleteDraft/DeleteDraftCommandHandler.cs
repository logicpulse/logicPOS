using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.DeleteDraft
{
    public class DeleteDraftCommandHandler :
        RequestHandler<DeleteDraftCommand, ErrorOr<bool>>
    {
        public DeleteDraftCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteDraftCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"documents/draft/{command.Id}", cancellationToken);
        }
    }
}
