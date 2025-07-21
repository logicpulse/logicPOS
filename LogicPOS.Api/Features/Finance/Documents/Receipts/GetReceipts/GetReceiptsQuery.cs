using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Receipts.GetReceipts
{
    public class GetReceiptsQuery : IRequest<ErrorOr<PaginatedResult<ReceiptViewModel>>>
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string Search { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? PaymentMethodId { get; set; }

        public string GetUrlQuery()
        {
            var query = new StringBuilder("?");

            if (StartDate.HasValue)
            {
                query.Append($"startDate={StartDate:yyyy-MM-dd}");
            }

            if (EndDate.HasValue)
            {
                query.Append($"&endDate={EndDate:yyyy-MM-dd}");
            }

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



            if (CustomerId.HasValue)
            {
                query.Append($"&customerId={CustomerId}");
            }

            if (PaymentMethodId.HasValue)
            {
                query.Append($"&paymentMethodId={PaymentMethodId}");
            }

            return query.ToString();
        }
    }
}
