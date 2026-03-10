using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.UI.Application
{
    public static class SystemVersionService
    {
        public static Version PosVersion { get; private set; }
        public static Version ApiVersion { get; private set; }
        public static Version LastestVersion { get; private set; }

        public static void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            PosVersion = Version.Parse(fileVersionInfo.ProductVersion);
            LastestVersion = LicensingService.GetLatestSystemVersion();
            ApiVersion = SystemInformationService.GetApiVersion();
            Log.Information("LogicPOS version: {Version}", SystemVersionService.PosVersion);
            Log.Information("API version: {Version}", SystemVersionService.ApiVersion);
        }
    }
}
