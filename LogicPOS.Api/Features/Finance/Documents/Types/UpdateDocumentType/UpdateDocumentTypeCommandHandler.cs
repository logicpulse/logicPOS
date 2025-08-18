using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.DocumentTypes;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType
{
    public class UpdateDocumentTypeCommandHandler :
        RequestHandler<UpdateDocumentTypeCommand, ErrorOr<Unit>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;
        public UpdateDocumentTypeCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache=cache;
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateDocumentTypeCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            var result= await HandleUpdateCommandAsync($"documents/types/{command.Id}", command, cancellationToken);
            if (result.IsError == false) 
            { 
                DocumentTypesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
