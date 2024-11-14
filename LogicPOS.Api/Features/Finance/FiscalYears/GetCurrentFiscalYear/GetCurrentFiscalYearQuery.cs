using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;

namespace LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear
{
    public class GetCurrentFiscalYearQuery : IRequest<ErrorOr<FiscalYear>>
    {

    }
}
