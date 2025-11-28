using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries
{
    public class ListOnlineSeriesQuery : IRequest<ErrorOr<IEnumerable<OnlineSeriesInfo>>>
    {

    }
}
