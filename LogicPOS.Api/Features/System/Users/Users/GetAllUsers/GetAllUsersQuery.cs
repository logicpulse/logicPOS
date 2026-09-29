using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Users.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<ErrorOr<IEnumerable<User>>>
    {
    }
}
