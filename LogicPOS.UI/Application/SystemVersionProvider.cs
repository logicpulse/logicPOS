using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.UI.Application
{
    public static class SystemVersionProvider
    {
        public static string Version { get; private set; }
        static SystemVersionProvider()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Version = fileVersionInfo.ProductVersion;
        }


    }
}
