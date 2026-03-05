using AutoUpdaterDotNET;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace LogicPOS.UI.Application
{
    public static class SystemVersionService
    {
        public static Version PosVersion { get; private set; }
        public static Version ApiVersion { get; private set; }
        public static Version LastestVersion { get; private set; }
        public static bool PosHasUpdate => LastestVersion > PosVersion;
        public static bool ApiHasUpdate => LastestVersion > ApiVersion;
        private static Gtk.Window _instance = null;

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

        public static string UpdaterPath => Path.Combine(Environment.CurrentDirectory, "AutoUpdater.Net.dll");

        public static void RunAutoUpdater(Gtk.Window instance = null)
        {

            _instance = instance;
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.TopMost = true;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.AppTitle = "LogicPOS";
            AutoUpdater.Icon = (Bitmap)Bitmap.FromFile("Assets\\Images\\application.ico");
            AutoUpdater.InstalledVersion = SystemVersionService.PosVersion;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
#if DEBUG
            AutoUpdater.Start("https://box.track.pt/files/latest/update.xml");
#else
            AutoUpdater.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.xml"));
#endif
        }

        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.IsUpdateAvailable)
            {
                if (_instance != null)
                {
                    _instance.Hide();
                }
                AutoUpdater.DownloadUpdate(args);
                Gtk.Application.Quit();
            }

        }

        public static void CreateUpdateXml()
        {
            var xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "false"),
                new XElement("item",
                    new XElement("version", LastestVersion.ToString()),
                    new XElement("url", "https://box.track.pt/files/latest/logicpos_release.zip"),
                    new XElement("mandatory", "true")
                )
            );

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.xml");
            xml.Save(path);
        }

    }
}
