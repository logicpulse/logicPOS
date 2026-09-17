using ErrorOr;
using MediatR;
using System;
using System.Globalization;

namespace LogicPOS.Api.Features.Finance.Documents.Series.HasActiveSeries
{
    public class HasActiveDocumentSeriesQuery : IRequest<ErrorOr<HasActiveDocumentSeriesResponse>>
    {
        public string DocumentType { get; set; }

        public Guid? TerminalId { get; set; }

        public HasActiveDocumentSeriesQuery(string documentType, Guid? terminalId = null)
        {
            DocumentType = documentType;
            TerminalId = terminalId;
        }

        public string GetUrlQuery()
        {
            var query = $"?documentType={Uri.EscapeDataString(DocumentType ?? string.Empty)}";
            if (TerminalId.HasValue && TerminalId.Value != Guid.Empty)
            {
                query += $"&terminalId={TerminalId.Value.ToString("D", CultureInfo.InvariantCulture)}";
            }

            return query;
        }
    }
}
