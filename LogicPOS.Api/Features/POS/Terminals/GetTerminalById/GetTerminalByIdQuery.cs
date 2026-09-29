using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Terminals.GetTerminalById
{
    public class GetTerminalByIdQuery : IRequest<ErrorOr<Terminal>>
    {
        public Guid Id { get; set; }

        public GetTerminalByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
