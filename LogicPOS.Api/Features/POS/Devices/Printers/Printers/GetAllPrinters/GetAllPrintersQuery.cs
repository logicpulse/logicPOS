using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Printers.GetAllPrinters
{
    public class GetAllPrintersQuery : IRequest<ErrorOr<IEnumerable<Printer>>>
    {
    }
}
