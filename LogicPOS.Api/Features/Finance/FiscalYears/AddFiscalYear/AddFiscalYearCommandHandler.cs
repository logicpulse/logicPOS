using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.AddFiscalYear
{
    public class AddFiscalYearCommandHandler : RequestHandler<AddFiscalYearCommand, ErrorOr<Guid>>
    {
        public AddFiscalYearCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddFiscalYearCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("fiscalyears", command, cancellationToken);
        }
    }
}
