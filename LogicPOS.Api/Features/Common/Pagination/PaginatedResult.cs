using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Common.Pagination
{
    public struct PaginatedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int ItemsCount { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
