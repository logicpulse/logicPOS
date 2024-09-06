using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.PoleDisplays.GetAllPoleDisplays
{
    public class GetAllPoleDisplaysQuery : IRequest<ErrorOr<IEnumerable<PoleDisplay>>>
    {
    }
}
