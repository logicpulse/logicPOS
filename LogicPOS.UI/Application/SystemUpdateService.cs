using AutoUpdaterDotNET;
using Serilog;
using System;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace LogicPOS.UI.Application
{
    public static class SystemUpdateService
    {
        private static Gtk.Window _instance = null;
        public static bool PosHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.PosVersion;
        public static bool ApiHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.ApiVersion;

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
            AutoUpdater.Start("https://box.track.pt/files/latest/update.xml");
        }

        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {

            if (args.IsUpdateAvailable)
            {
                if (_instance != null)
                    _instance.Hide();

                bool result = AutoUpdater.DownloadUpdate(args);

                if (!result)
                {
                    _instance.Show();
                    return;
                }
                else
                {
                    Program.Quit();

                }
            }
            else
            {
                Log.Error("An error occurred while checking for updates: " + args.Error.Message);
            }

        }

        public static void CreateUpdateXml()
        {
            var xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "false"),
                new XElement("item",
                    new XElement("version", SystemVersionService.LastestVersion.ToString()),
                    new XElement("url", "https://box.track.pt/files/latest/logicpos_release.zip"),
                    new XElement("mandatory", "true")
                )
            );

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.xml");
            xml.Save(path);
        }

       
    }
}
