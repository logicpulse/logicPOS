using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Finance.FiscalYears.GetFiscalYearCreationData
{
    public class GetFiscalYearCreationDataQuery : IRequest<ErrorOr<FiscalYearCreationData>>
    {

    }
}
