using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters
{
    public class GetAllPreferenceParametersQueryHandler : RequestHandler<GetAllPreferenceParametersQuery, ErrorOr<IEnumerable<PreferenceParameter>>>
    {
        public GetAllPreferenceParametersQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PreferenceParameter>>> Handle(GetAllPreferenceParametersQuery query,
                                                                               CancellationToken cancellationToken = default)
        {
            return await HandleGetListQueryAsync<PreferenceParameter>("preference-parameters", cancellationToken);
        }
    }
}
