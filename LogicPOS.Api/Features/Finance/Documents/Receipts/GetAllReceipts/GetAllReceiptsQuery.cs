using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Receipts.GetAllReceipts
{
    public class GetAllReceiptsQuery : IRequest<ErrorOr<IEnumerable<Receipt>>>
    {

    }
}
