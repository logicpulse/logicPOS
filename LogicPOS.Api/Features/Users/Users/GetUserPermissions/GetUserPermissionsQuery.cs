using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Users.GetUserPermissions
{
    public class GetUserPermissionsQuery : IRequest<ErrorOr<IEnumerable<string>>>
    {
        public Guid Id { get; set; }

        public GetUserPermissionsQuery(Guid id)
        {
            Id = id;
        }
    }
}
