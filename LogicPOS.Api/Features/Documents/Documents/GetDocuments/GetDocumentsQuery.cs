using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Documents.GetDocuments
{
    public class GetDocumentsQuery : IRequest<ErrorOr<PaginatedResult<Document>>>
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string Search { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] Types { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? PaymentConditionId { get; set; }

        public string GetUrlQuery()
        {
            var query = new StringBuilder($"?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}");

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query.Append($"&search={Search}");
            }

            if (Page.HasValue)
            {
                query.Append($"&page={Page}");
            }

            if (PageSize.HasValue)
            {
                query.Append($"&pageSize={PageSize}");
            }

            if (Types != null && Types.Length > 0)
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    query.Append($"&types={Types[i]}");
                }     
            }

            if (CustomerId.HasValue)
            {
                query.Append($"&customerId={CustomerId}");
            }

            if (PaymentMethodId.HasValue)
            {
                query.Append($"&paymentMethodId={PaymentMethodId}");
            }

            if (PaymentConditionId.HasValue)
            {
                query.Append($"&paymentConditionId={PaymentConditionId}");
            }

            return query.ToString();
        }
    }
}
