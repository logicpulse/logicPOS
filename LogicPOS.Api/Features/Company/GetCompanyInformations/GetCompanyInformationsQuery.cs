using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Company.GetCompanyInformations
{
    public class GetCompanyInformationsQuery : IRequest<ErrorOr<CompanyInformations>>
    {
    }
}
