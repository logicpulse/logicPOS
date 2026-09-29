using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.UpdateCompanyDetails
{
    public class UpdateCompanyDetailsCommandHandler : RequestHandler<UpdateCompanyDetailsCommand, ErrorOr<Success>>
    {
        public UpdateCompanyDetailsCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

   
        public override async Task<ErrorOr<Success>> Handle(UpdateCompanyDetailsCommand request, CancellationToken ct = default)
        {
            return await HandleUpdateCommandAsync("company/details", request, ct);
        }
    }
}
