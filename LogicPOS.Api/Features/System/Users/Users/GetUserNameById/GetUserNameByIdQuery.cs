using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Users.GetUserNameById
{
    public class GetUserNameByIdQuery : IRequest<ErrorOr<string>>
    {
        public Guid Id { get; set; }

        public GetUserNameByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
