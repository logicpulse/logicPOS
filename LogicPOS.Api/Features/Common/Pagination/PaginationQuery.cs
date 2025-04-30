using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocuments;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Common.Pagination
{
    public abstract class PaginationQuery<TEntity> : IRequest<ErrorOr<PaginatedResult<TEntity>>> where TEntity : class
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string Search { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public abstract void BuildQuery(StringBuilder urlQueryBuilder);
      
        public string GetUrlQuery()
        {
            var queryBuilder = new StringBuilder("?");

            if (StartDate.HasValue)
            {
                queryBuilder.Append($"startDate={StartDate:yyyy-MM-dd}");
            }

            if (EndDate.HasValue)
            {
                queryBuilder.Append($"&endDate={EndDate:yyyy-MM-dd}");
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                queryBuilder.Append($"&search={Search}");
            }

            if (Page.HasValue)
            {
                queryBuilder.Append($"&page={Page}");
            }

            if (PageSize.HasValue)
            {
                queryBuilder.Append($"&pageSize={PageSize}");
            }

            BuildQuery(queryBuilder);

            return queryBuilder.ToString();
        }
    }
}
