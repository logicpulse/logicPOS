using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Tables.GetTableById
{
    public class GetTableByIdQuery : IRequest<ErrorOr<Table>>
    {
        public Guid Id { get; set; }
        public GetTableByIdQuery(Guid id) => Id = id;
    }
}
