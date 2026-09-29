using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.GetAllUsers;
using LogicPOS.Api.Features.Users.GetUserNameById;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Users
{
    public static class UsersService
    {
        public static string GetUserNameById(Guid id)
        {
            var getUserNameResult = DependencyInjection.Mediator.Send(new GetUserNameByIdQuery(id)).Result;

            if (getUserNameResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getUserNameResult);
                return null;
            }

            return getUserNameResult.Value;
        }

        public static IEnumerable<User> GetAllUsers()
        {
            var getUsersResult = DependencyInjection.Mediator.Send(new GetAllUsersQuery()).Result;

            if (getUsersResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getUsersResult);
                return null;
            }

            return getUsersResult.Value.OrderBy(u => u.Code);
        }
    }
}
