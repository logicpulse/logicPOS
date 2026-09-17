using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Types;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.FiscalYears.CreateFiscalYear
{
    public class CreateFiscalYearCommandHandler : RequestHandler<CreateFiscalYearCommand, ErrorOr<Guid>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CreateFiscalYearCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Guid>> Handle(CreateFiscalYearCommand command, CancellationToken cancellationToken = default)
        {
            var result = await HandleAddCommandAsync("fiscal-years", command, cancellationToken);

            if (result.IsError == false)
            {
                DocumentTypesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
