using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Types;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.FiscalYears.CloseFiscalYear
{
    public class CloseFiscalYearCommandHandler : RequestHandler<CloseFiscalYearCommand, ErrorOr<Success>>
    {
        private readonly IKeyedMemoryCache _keyedMemoryCache;

        public CloseFiscalYearCommandHandler(IHttpClientFactory factory, IKeyedMemoryCache cache) : base(factory)
        {
            _keyedMemoryCache = cache;
        }

        public override async Task<ErrorOr<Success>> Handle(CloseFiscalYearCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandleUpdateCommandAsync($"fiscal-years/{request.Id}/close", request, cancellationToken);

            if (result.IsError == false)
            {
                DocumentTypesCache.Clear(_keyedMemoryCache);
            }

            return result;
        }
    }
}
