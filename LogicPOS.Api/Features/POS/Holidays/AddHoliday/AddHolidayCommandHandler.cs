using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Holidays.AddHoliday
{
    public class AddHolidayCommandHandler : RequestHandler<AddHolidayCommand, ErrorOr<Guid>>
    {
        public AddHolidayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(
            AddHolidayCommand command,
            CancellationToken cancellationToken = default)
        {
          return await HandleAddCommandAsync("holidays", command, cancellationToken);
        }
    }
}
