using System;

namespace logicpos.financial.console.Test.WS
{
    internal class TestWSInterface
    {
        private static readonly string _wsLogin = "admin";
        private static readonly string _wsPassword = "admin";
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
