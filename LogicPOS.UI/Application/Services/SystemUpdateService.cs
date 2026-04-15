using AutoUpdaterDotNET;
using LogicPOS.Api.Features.System.Monitor.CreateUpdateSignal;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Licensing;
using Serilog;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogicPOS.UI.Application.Services
{
    public static class SystemUpdateService
    {
        private static Gtk.Window _instance = null;
        public static bool PosHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.PosVersion;
        public static bool ApiHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.ApiVersion;

        public static string UpdaterPath => Path.Combine(Environment.CurrentDirectory, "AutoUpdater.Net.dll");

        public static async Task<bool> UpdateZipFileIsAvailable(string url = "https://box.track.pt/files/latest/logicpos_1.5.zip")
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

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
            if (UpdateZipFileIsAvailable().Result)
            {
#if DEBUG
                AutoUpdater.Start("https://box.track.pt/files/latest/update.xml");
#else
                AutoUpdater.Start(Directory.GetCurrentDirectory()+"\\update.xml");
#endif
            }
            else
            {
                Log.Error("The update zip file is not available at the specified URL.");
                CustomAlerts.Error()
                    .WithMessage("Ocorreu um erro ao tentar baixar os arquivos de atualização. \n\nTente novamente mais tarde.")
                    .ShowAlert();
            }
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
                    new XElement("version", SystemVersionService.LatestVersionFromLicense.ToString()),
                    new XElement("url", "https://box.track.pt/files/latest/logicpos_release.zip"),
                    new XElement("mandatory", "true")
                )
            );

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.xml");
            xml.Save(path);
        }

        public static bool SendUpdateSignalToApi()
        {
            var result = DependencyInjection.Mediator.Send(new CreateUpdateSignalCommand()).Result;
            return !result.IsError;
        }

    }
}
