using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Holidays.DeleteHoliday
{
    public class DeleteHolidayCommandHandler :
        RequestHandler<DeleteHolidayCommand, ErrorOr<bool>>
    {
        public DeleteHolidayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<bool>> Handle(DeleteHolidayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleDeleteCommandAsync($"holidays/{command.Id}", cancellationToken);
        }
    }
}
