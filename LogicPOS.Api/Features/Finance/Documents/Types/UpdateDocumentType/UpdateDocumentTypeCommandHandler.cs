using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType
{
    public class UpdateDocumentTypeCommandHandler :
        RequestHandler<UpdateDocumentTypeCommand, ErrorOr<Unit>>
    {
        public UpdateDocumentTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateDocumentTypeCommand command,
                                                   CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"documents/types/{command.Id}", command, cancellationToken);
        }
    }
}
