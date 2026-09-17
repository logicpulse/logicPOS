using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Documents.GetDocuments
{
    public class GetDocumentsQuery : PaginationQuery<DocumentViewModel>
    {
        public string[] Types { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public Guid? PaymentConditionId { get; set; }
        public DocumentPaymentStatusFilter? PaymentStatus { get; set; }
        public char? Status { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (Types != null && Types.Length > 0)
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    urlQueryBuilder.Append($"&types={Types[i]}");
                }
            }

            if (CustomerId.HasValue)
            {
                urlQueryBuilder.Append($"&customerId={CustomerId}");
            }

            if (PaymentMethodId.HasValue)
            {
                urlQueryBuilder.Append($"&paymentMethodId={PaymentMethodId}");
            }

            if (PaymentConditionId.HasValue)
            {
                urlQueryBuilder.Append($"&paymentConditionId={PaymentConditionId}");
            }

            if (PaymentStatus != null && PaymentStatus != DocumentPaymentStatusFilter.All)
            {
                urlQueryBuilder.Append($"&paymentStatus={(int)PaymentStatus}");
            }

            if(Status != null)
            {
                urlQueryBuilder.Append($"&status={Status}");
            }
        }

    }
}
