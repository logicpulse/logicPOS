using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Common
{
    public abstract class DeleteCommand : IRequest<ErrorOr<bool>>
    {
        public Guid Id { get; set; }

        public DeleteCommand(Guid id)
        {
            Id = id;
        }
    }
}
