using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.GetCompanyInformations
{
    public class GetCompanyInformationsQueryHandler : RequestHandler<GetCompanyInformationsQuery, ErrorOr<CompanyInformation>>
    {
        public GetCompanyInformationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<CompanyInformation>> Handle(GetCompanyInformationsQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<CompanyInformation>("company/info", cancellationToken);
        }
    }
}

