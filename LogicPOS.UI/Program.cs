using Gtk;
using logicpos;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Utils;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Services;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Threading;


namespace LogicPOS.UI
{

    internal partial class Program
    {
        private static Thread _loadingThread;
        public static Dialog SplashScreen { get; set; }

        public static void InitializeGtk()
        {
            Gtk.Application.Init();
            GtkThemeStyle.ParseTheme();
        }

        public static void ShowLoadingScreen()
        {
            SplashScreen = Utils.CreateSplashScreen();

            _loadingThread = new Thread(() => SplashScreen.Run());
            _loadingThread.Start();
        }

        private static void KeepUIResponsive()
        {
            while (Gtk.Application.EventsPending())
            {
                Gtk.Application.RunIteration();
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureLogging();

            if (IsFirstLaunch())
            {
                Log.Information("First launch detected, starting migrator...");
                MigratorUtility.LaunchMigrator();
                
            }

            using (var singleProgramInstance = new SingleProgramInstance())
            {
                if (singleProgramInstance.IsSingleInstance == false)
                {
                    SimpleAlerts.ShowInstanceAlreadyRunningAlert();
                    return;
                }

                InitializeGtk();

                if (DependencyInjection.Initialize() == false)
                {
                    Quit();
                    return;
                }

                if (InitializeCulture() == false)
                {
                    Quit();
                    return;
                }

                ShowLoadingScreen();

                CloseLoadingScreen();

                KeepUIResponsive();

                StartApp();
            }

            Log.CloseAndFlush();
        }

        private static bool IsFirstLaunch()
        {
            return File.Exists("terminal.id") == false;
        }

        private static void Quit()
        {
            Gtk.Application.Quit();
            Environment.Exit(0);
        }

        private static void CloseLoadingScreen()
        {
            _loadingThread.Abort();
            SplashScreen.Destroy();
        }

        private static void StartApp()
        {
            if (LicensingService.Initialize() == false)
            {
                return;
            }

            var terminalResult = TerminalService.InitializeTerminal();
            if(terminalResult.IsError)
            {
                SimpleAlerts.Error()
                            .WithTitle("Erro ao inicializar terminal")
                            .WithMessage(string.Join(Environment.NewLine, terminalResult.FirstError.Description))
                            .ShowAlert();
                return;
            }

            if (LicensingService.Data.IsLicensed == false)
            {
                if (LicensingService.ConnectToWs())
                {
                    var result = LicensingService.ActivateFromFile();
                    if (result == false)
                    {
                        RegisterModal.ShowModal();
                    }
                }
                else
                {
                    if (!File.Exists(LicensingService.OFFLINE_ACTIVATION_FILE))
                    {
                        RegisterModal.ShowModal();
                    }
                }
            }

            LogicPOSApp app = new LogicPOSApp();
            app.Start();
        }

        public static bool InitializeCulture()
        {
            try
            {

                if (SystemInformationService.SystemInformation == null)
                {
                    return false;
                }

                var culture = SystemInformationService.SystemInformation.Culture;
                LocalizedString.Instance = new LocalizedString(culture);
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

                return true;
            }
            catch
            {
                SimpleAlerts.Error()
                            .WithTitle("Erro ao obter informações do sistema")
                            .WithMessage("Erro ao obter informações do sistema")
                            .ShowAlert();
                return false;
            }
        }

        public static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                              .Enrich.FromLogContext()
                              .WriteTo.File("Logs/log.txt",
                                            rollingInterval: RollingInterval.Year,
                                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}")
                              .CreateLogger();
        }
    }
}