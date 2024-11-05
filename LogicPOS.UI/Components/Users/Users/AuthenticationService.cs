using LogicPOS.Api.Entities;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Users
{
    public static class AuthenticationService
    {
        public static UserDetail User { get; private set; }
        public static List<string> Permissions { get; private set; }

        public static bool UserHasPermission(string permission)
        {
            if(Permissions == null)
            {
                return false;
            }

            return Permissions.Contains(permission);
        }

        public static void LoginUser(UserDetail user)
        {
            User = user;
            Permissions = new List<string>();
        }
    }
}
