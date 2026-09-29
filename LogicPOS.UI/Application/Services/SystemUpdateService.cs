using AutoUpdaterDotNET;
using LogicPOS.Api.Features.System.Monitor.CreateUpdateSignal;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Licensing;
using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gtk;

namespace LogicPOS.UI.Application.Services
{
    public static class SystemUpdateService
    {
        private static Gtk.Window _instance = null;
        public static bool PosHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.PosVersion;
        public static bool ApiHasUpdate => SystemVersionService.LastestVersion > SystemVersionService.ApiVersion;

        public static string UpdaterPath => Path.Combine(Environment.CurrentDirectory, "AutoUpdater.Net.dll");

        public const string RetailInstallerUrl = "https://box.track.pt/files/latest/logicPOS_v1.5.exe";

        public static async Task<bool> UpdateZipFileIsAvailable(string url = "https://box.track.pt/files/latest/logicpos_1.5.zip")
        {
            return await RemoteFileIsAvailable(url).ConfigureAwait(false);
        }

        public static async Task<bool> InstallerIsAvailable(string url = RetailInstallerUrl)
        {
            return await RemoteFileIsAvailable(url).ConfigureAwait(false);
        }

        private static async Task<bool> RemoteFileIsAvailable(string url)
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(request).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static void RunAutoUpdater(Gtk.Window instance = null)
        {
            _instance = instance;
            if (!InstallerIsAvailable().GetAwaiter().GetResult())
            {
                Log.Error("The MSI installer is not available at the configured URL.");
                CustomAlerts.Error()
                    .WithMessage("Ocorreu um erro ao tentar obter o instalador de atualização. \n\nTente novamente mais tarde.")
                    .ShowAlert();
                return;
            }

            _ = RunInstallerSilentUpdateAsync();
        }

        private static async Task RunInstallerSilentUpdateAsync()
        {
            Dialog progressDialog = null;
            Label statusLabel = null;
            ProgressBar progressBar = null;

            try
            {
                if (_instance != null)
                    _instance.Hide();

                CreateUpdateProgressDialog(out progressDialog, out statusLabel, out progressBar);
                progressDialog.ShowAll();
                PumpGtk();

                if (ApiHasUpdate)
                    SendUpdateSignalToApi();

                const string installerUrl = RetailInstallerUrl;
                var tempPath = Path.Combine(Path.GetTempPath(), $"logicPOS_update_{Guid.NewGuid():N}.exe");

                UpdateProgressUi(statusLabel, progressBar, "A descarregar a atualização…", null);
                PumpGtk();

                await DownloadInstallerAsync(installerUrl, tempPath, (read, total) =>
                {
                    Gtk.Application.Invoke(delegate
                    {
                        if (total > 0)
                        {
                            var fraction = Math.Min(1.0, (double)read / total);
                            var mbRead = read / (1024.0 * 1024.0);
                            var mbTotal = total / (1024.0 * 1024.0);
                            UpdateProgressUi(
                                statusLabel,
                                progressBar,
                                string.Format("A descarregar… {0:0}% ({1:0.0}/{2:0.0} MB)", fraction * 100, mbRead, mbTotal),
                                fraction);
                        }
                        else
                        {
                            progressBar.Pulse();
                            statusLabel.Text = "A descarregar a atualização…";
                        }

                        PumpGtk();
                    });
                }).ConfigureAwait(true);

                UpdateProgressUi(statusLabel, progressBar, "A iniciar o instalador…", 1.0);
                PumpGtk();

                Process.Start(new ProcessStartInfo
                {
                    FileName = tempPath,
                    Arguments = "--pos-update",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                progressDialog.Destroy();
                progressDialog = null;
                Quit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Silent update failed.");
                if (progressDialog != null)
                    progressDialog.Destroy();

                if (_instance != null)
                    _instance.Show();

                CustomAlerts.Error()
                    .WithMessage("Ocorreu um erro ao atualizar o logicPOS.\n\n" + ex.Message)
                    .ShowAlert();
            }
        }

        private static void CreateUpdateProgressDialog(out Dialog dialog, out Label statusLabel, out ProgressBar progressBar)
        {
            dialog = new Dialog(
                "logicPOS",
                _instance,
                DialogFlags.Modal | DialogFlags.DestroyWithParent);

            dialog.WindowPosition = WindowPosition.CenterAlways;
            dialog.SetSizeRequest(420, 140);
            dialog.Decorated = true;
            dialog.Resizable = false;
            dialog.KeepAbove = true;
            dialog.ActionArea.Hide();

            var box = new VBox(false, 12)
            {
                BorderWidth = 16
            };

            var title = new Label("A atualizar o logicPOS…")
            {
                Xalign = 0f
            };
            title.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 11 Bold"));

            statusLabel = new Label("A preparar…")
            {
                Xalign = 0f
            };
            statusLabel.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 9"));

            progressBar = new ProgressBar
            {
                Fraction = 0
            };

            box.PackStart(title, false, false, 0);
            box.PackStart(statusLabel, false, false, 0);
            box.PackStart(progressBar, false, false, 0);
            dialog.VBox.PackStart(box, true, true, 0);
        }

        private static void UpdateProgressUi(Label statusLabel, ProgressBar progressBar, string text, double? fraction)
        {
            if (statusLabel != null)
                statusLabel.Text = text;

            if (progressBar == null)
                return;

            if (fraction.HasValue)
                progressBar.Fraction = Math.Max(0, Math.Min(1.0, fraction.Value));
            else
                progressBar.Pulse();
        }

        private static void PumpGtk()
        {
            while (Gtk.Application.EventsPending())
                Gtk.Application.RunIteration();
        }

        private static async Task DownloadInstallerAsync(
            string url,
            string destinationPath,
            Action<long, long> onProgress)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var total = response.Content.Headers.ContentLength ?? -1L;
                using (var remote = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var local = File.Create(destinationPath))
                {
                    var buffer = new byte[81920];
                    long readTotal = 0;
                    long lastReported = -1;
                    var lastUi = DateTime.UtcNow.AddSeconds(-1);
                    int read;
                    while ((read = await remote.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                    {
                        await local.WriteAsync(buffer, 0, read).ConfigureAwait(false);
                        readTotal += read;

                        var now = DateTime.UtcNow;
                        var percent = total > 0 ? (readTotal * 100) / total : -1;
                        if ((now - lastUi).TotalMilliseconds >= 200 || percent != lastReported || (total > 0 && readTotal >= total))
                        {
                            lastUi = now;
                            lastReported = percent;
                            onProgress?.Invoke(readTotal, total);
                        }
                    }
                }
            }
        }

        private static void Quit() => Program.Quit();

        [Obsolete("Legacy zip updater retained for reference; retail MSI updates use RunInstallerSilentUpdateAsync.")]
        public static void RunLegacyZipAutoUpdater(Gtk.Window instance = null)
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
                AutoUpdater.Start(Directory.GetCurrentDirectory()+"\\update.xml");
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
                    new XElement("url", RetailInstallerUrl),
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
