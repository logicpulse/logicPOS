using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Documents.Receipts.GetReceiptPdf
{
    public class GetReceiptPdfQueryHandler :
        RequestHandler<GetReceiptPdfQuery, ErrorOr<TempFile>>
    {
        public GetReceiptPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<TempFile>> Handle(GetReceiptPdfQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetFileQueryAsync($"receipts/{query.Id}/pdf?copyNumber={query.CopyNumber}");
        }
    }
}
