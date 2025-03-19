using ErrorOr;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Holidays.UpdateHoliday
{
    public class UpdateHolidayCommandHandler :
        RequestHandler<UpdateHolidayCommand, ErrorOr<Unit>>
    {
        public UpdateHolidayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateHolidayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/holidays/{command.Id}", command, cancellationToken);
        }
    }
}
