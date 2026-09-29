using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using MediatR;

namespace LogicPOS.Api.Features.Finance.Customers.Customers.ExportCustomersToExcel
{
    public class ExportCustomersToExcelQuery : IRequest<ErrorOr<TempFile>>
    {

    }
}
