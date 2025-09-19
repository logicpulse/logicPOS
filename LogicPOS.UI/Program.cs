using Gtk;
using logicpos;
using LogicPOS.Api.Features.System.GetSystemInformations;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Licensing;
using LogicPOS.UI.Application.Utils;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;


namespace LogicPOS.UI
{

    internal partial class Program
    {
        private static Thread _loadingThread;
        public static Dialog SplashScreen { get; set; }

        public static void InitializeGtk()
        {
            Gtk.Application.Init();
            Theme.ParseTheme();
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

            if(IsFirstLaunch())
            {
                Log.Information("First launch detected, starting migrator...");
                MigratorUtility.LaunchMigrator();
                DisableFirstLaunch();
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
            return File.Exists("_booted.pos") == false;
        }


        private static void DisableFirstLaunch()
        {
            try
            {
               File.CreateText("_booted.pos").Close();
            }
            catch (Exception ex)
            {
                Log.Error("Error disabling first launch: " + ex.Message, ex);
            }
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
            LicenseRouter licenseRouter = new LicenseRouter();

            if (licenseRouter.LoadApp == false)
            {
                return;
            }

            var intializeTerminalResult = TerminalService.InitializeTerminal();

            if (intializeTerminalResult.IsError)
            {
                ErrorHandlingService.HandleApiError(intializeTerminalResult, true);
                return;
            }

            if (LicenseRouter.NeedToRegister())
            {
                ShowLicenseDialog();
            }

            LogicPOSApp app = new LogicPOSApp();
            app.Start();
        }

        private static void ShowLicenseDialog()
        {
            string hardWareId = TerminalService.Terminal.HardwareId;
            PosLicenceDialog.GetLicenseDetails(hardWareId);
        }

        public static bool InitializeCulture()
        {
            try
            {
                var getSystemInformationsResult = DependencyInjection.Mediator.Send(new GetSystemInformationsQuery()).Result;

                if (getSystemInformationsResult.IsError)
                {
                    SimpleAlerts.ShowApiErrorAlert(getSystemInformationsResult.FirstError);

                    return false;
                }

                var culture = getSystemInformationsResult.Value.Culture;
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

#if (DEBUG)
        public static readonly bool DebugMode = true;
#else
        public static readonly bool DebugMode = false;
#endif

    }
}