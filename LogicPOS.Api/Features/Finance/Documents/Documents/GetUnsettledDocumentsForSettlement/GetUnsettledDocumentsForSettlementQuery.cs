using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.GetUnsettledDocumentsForSettlement
{
    public class GetUnsettledDocumentsForSettlementQuery : PaginationQuery<DocumentViewModel>
    {
        public Guid? CustomerId { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (CustomerId.HasValue)
            {
                urlQueryBuilder.Append($"&customerId={CustomerId}");
            }
        }
    }
}
