using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.financial.console.Test.WS
{
    class TestWSInterface
    {
        private static string _wsLogin = "admin";
        private static string _wsPassword = "admin";
        private static qws.WSInterface WebService { get { return GetWebService(); } }
        private static qws.WSInterface GetWebService() { return new qws.WSInterface(); }

        public static string GetSecurityToken() 
        {
            string securityToken = WebService.GetSecurityToken(_wsLogin, _wsPassword);
            Console.WriteLine(securityToken);
            return securityToken;
        }
    }
}
