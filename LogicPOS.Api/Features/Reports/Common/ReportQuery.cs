using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Reports.Common
{
    public abstract class ReportQuery : IRequest<ErrorOr<TempFile>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string DocumentType { get; set; }
        public Guid? TerminalId { get; set; }

        public ReportQuery(DateTime startDate, DateTime endDate, string documentType, Guid? terminalId)
        {
            StartDate = startDate;
            EndDate = endDate;
            DocumentType = documentType;
            TerminalId = terminalId;
        }

        protected abstract void BuildQuery(StringBuilder urlQueryBuilder);

        public string GetUrlQuery()
        {
            var queryBuilder = new StringBuilder("?");

            queryBuilder.Append($"startDate={StartDate:yyyy-MM-dd}");
            queryBuilder.Append($"&endDate={EndDate:yyyy-MM-dd}");

            if (!string.IsNullOrWhiteSpace(DocumentType))
            {
                queryBuilder.Append($"&DocumentType={DocumentType}");
            }

            if (TerminalId.HasValue)
            {
                queryBuilder.Append($"&TerminalId={TerminalId}");
            }

            BuildQuery(queryBuilder);

            return queryBuilder.ToString();
        }
    }
}
