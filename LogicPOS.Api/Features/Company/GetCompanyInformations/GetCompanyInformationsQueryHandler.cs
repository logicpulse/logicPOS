using ErrorOr;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.GetCompanyInformations
{
    public class GetCompanyInformationsQueryHandler : RequestHandler<GetCompanyInformationsQuery, ErrorOr<CompanyInformations>>
    {
        public GetCompanyInformationsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<CompanyInformations>> Handle(GetCompanyInformationsQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<CompanyInformations>("/companyinformations", cancellationToken);
        }
    }
}

