using ErrorOr;
using MediatR;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Common.Pagination
{
    public abstract class PaginationQuery<TEntity> : IRequest<ErrorOr<PaginatedResult<TEntity>>> where TEntity : class
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = ApiSettings.Default.DefaultPageSize;
        public string Search { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IncludeDeleted { get; set; }

        public void GoToNextPage()
        {
            Page = (Page ?? 0) + 1;
        }

        public void GoToPreviousPage()
        {
            Page = Math.Max(0, (Page ?? 0) - 1);
        }

        protected abstract void BuildQuery(StringBuilder urlQueryBuilder);

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

            if (IncludeDeleted.HasValue)
            {
                queryBuilder.Append($"&includeDeleted={IncludeDeleted}");
            }

            BuildQuery(queryBuilder);

            return queryBuilder.ToString();
        }

    }
}
