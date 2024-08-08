using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;

namespace LogicPOS.UI.Services
{
    public class AuthenticationService
    {
        public static sys_userdetail CurrentUser => XPOSettings.LoggedUser;
    }
}
