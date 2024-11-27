using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.WorkSessions.Periods.DayIsOpen
{
    public class DayIsOpenQueryHandler :
        RequestHandler<DayIsOpenQuery, ErrorOr<bool>>
    {
        public DayIsOpenQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<bool>> Handle(DayIsOpenQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<bool>("worksessions/periods/day-is-open", cancellationToken);
        }
    }
}
