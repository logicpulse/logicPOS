using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Documents.GetDocumentPrint
{
    public class GetDocumentThermalPrintingQueryHandler : RequestHandler<GetDocumentThermalPrintingQuery, ErrorOr<bool>>
    {
        public GetDocumentThermalPrintingQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(GetDocumentThermalPrintingQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<bool>($"documents/prints/{query.DocumentId}", cancellationToken);
        }
    }
}
