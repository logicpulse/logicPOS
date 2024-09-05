using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Tables.GetAllTables
{
    public class GetAllTablesQuery : IRequest<ErrorOr<IEnumerable<Table>>>
    {
    }
}
