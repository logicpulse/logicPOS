using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.PrinterTypes.GetAllPrinterTypes
{
    public class GetAllPrinterTypesQuery : IRequest<ErrorOr<IEnumerable<PrinterType>>>
    {
    }
}
