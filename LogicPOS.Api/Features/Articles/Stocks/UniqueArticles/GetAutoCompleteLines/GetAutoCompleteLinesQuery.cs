using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GetAutoCompleteLines
{
    public class GetAutoCompleteLinesQuery : IRequest<ErrorOr<IEnumerable<AutoCompleteLine>>>
    {
        public GetAutoCompleteLinesQuery()
        {
        }
    }
}
