using ErrorOr;
using LogicPOS.Api.Features.POS.Tables.Common;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.GetTableViewModel
{
    public class GetTableViewModelQuery : IRequest<ErrorOr<TableViewModel>>
    {
        public Guid Id { get; set; }
        public GetTableViewModelQuery(Guid id) => Id = id;
    }
}
