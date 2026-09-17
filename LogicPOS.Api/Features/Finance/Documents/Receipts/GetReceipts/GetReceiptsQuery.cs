using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Receipts.GetReceipts
{
    public class GetReceiptsQuery : PaginationQuery<ReceiptViewModel>
    {
        public Guid? CustomerId { get; set; }
        public Guid? PaymentMethodId { get; set; }

        protected override void BuildQuery(StringBuilder query)
        {
            if (CustomerId.HasValue)
            {
                query.Append($"&CustomerId={CustomerId}");
            }

            if (PaymentMethodId.HasValue)
            {
                query.Append($"&PaymentMethodId={PaymentMethodId}");
            }
        }
    }
}
