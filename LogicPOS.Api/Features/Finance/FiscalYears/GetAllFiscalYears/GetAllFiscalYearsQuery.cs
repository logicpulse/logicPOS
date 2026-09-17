using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears
{ 
    public class GetAllFiscalYearsQuery : IRequest<ErrorOr<IEnumerable<FiscalYear>>>
    {
    }
}
