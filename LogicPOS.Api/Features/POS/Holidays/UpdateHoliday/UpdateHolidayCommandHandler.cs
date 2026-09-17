using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Holidays.UpdateHoliday
{
    public class UpdateHolidayCommandHandler :
        RequestHandler<UpdateHolidayCommand, ErrorOr<Success>>
    {
        public UpdateHolidayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateHolidayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/holidays/{command.Id}", command, cancellationToken);
        }
    }
}
