using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.DeleteDraft
{
    public class DeleteDraftCommandHandler :
        RequestHandler<DeleteDraftCommand, ErrorOr<bool>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public DeleteDraftCommandHandler(IHttpClientFactory factory,IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteDraftCommand command, CancellationToken cancellationToken = default)
        {
            var result= await HandleDeleteCommandAsync($"documents/draft/{command.Id}", cancellationToken);
            if (result.IsError == false)
            {
                DocumentsCache.Clear(_keyedMemoryCache);
            }
            return result;
        }
    }
}
