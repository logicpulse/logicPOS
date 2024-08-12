using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems
{
    public class GetAllPermissionItemsQuery : IRequest<ErrorOr<IEnumerable<PermissionItem>>>
    {

    }
}
