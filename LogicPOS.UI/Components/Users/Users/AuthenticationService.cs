using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Authentication;
using LogicPOS.Api.Features.Users.GetUserPermissions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Users
{
    public static class AuthenticationService
    {
        public static User User { get; private set; }
        public static List<string> Permissions { get; private set; } = new List<string>();

        public static bool UserHasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public static void LoginUser(User user, string jwtToken)
        {
            User = user;
            LoadPermissions();
            AuthenticationData.Token = jwtToken;
        }

        public static void LogoutUser()
        {
            User = null;
            Permissions.Clear();
        }

        private static void LoadPermissions()
        {
            Permissions.Clear();

            var mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
            var getPermissionsResult = mediator.Send(new GetUserPermissionsQuery(User.Id)).Result;

            if(getPermissionsResult.IsError)
            {
                return;
            }

            Permissions.AddRange(getPermissionsResult.Value);
        }
    }
}
