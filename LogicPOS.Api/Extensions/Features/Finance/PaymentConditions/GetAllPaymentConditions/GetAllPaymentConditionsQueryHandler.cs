using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition
{
    public class GetAllPaymentConditionsQueryHandler :
        RequestHandler<GetAllPaymentConditionsQuery, ErrorOr<IEnumerable<PaymentCondition>>>
    {
        public GetAllPaymentConditionsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PaymentCondition>>> Handle(GetAllPaymentConditionsQuery query,
                                                                                  CancellationToken cancellationToken = default)
        {
            return await HandleGetEntitiesQueryAsync<PaymentCondition>("payment/conditions", cancellationToken);
        }
    }
}
