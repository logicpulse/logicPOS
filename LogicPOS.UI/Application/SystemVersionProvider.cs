using System;
using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.UI.Application
{
    public static class SystemVersionProvider
    {
        public static Version Version { get; private set; }
        static SystemVersionProvider()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Version = Version.Parse("1.0.0"/*fileVersionInfo.ProductVersion*/);
        }


    }
}
