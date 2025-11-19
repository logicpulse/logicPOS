using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.ListSeries
{
    public class ListAgtSeriesQuery : IRequest<ErrorOr<IEnumerable<AgtSeriesInfo>>>
    {

    }
}
