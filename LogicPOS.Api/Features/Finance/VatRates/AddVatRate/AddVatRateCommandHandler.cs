using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.VatRates.AddVatRate
{
    public class AddVatRateCommandHandler : RequestHandler<AddVatRateCommand, ErrorOr<Guid>>
    {
        public AddVatRateCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddVatRateCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("vatrates", command, cancellationToken);
        }
    }
}
