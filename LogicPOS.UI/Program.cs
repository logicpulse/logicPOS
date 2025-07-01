using Gtk;
using logicpos;
using LogicPOS.Api.Features.System.GetSystemInformations;
using LogicPOS.Globalization;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using Serilog;
using System;
using System.Globalization;
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

            using (var singleProgramInstance = new SingleProgramInstance())
            {
                PathsSettings.InitializePaths();

                InitializeGtk();


                if (InitializeCulture() == false)
                {
                    SimpleAlerts.Error()
                                .WithTitle("Erro")
                                .WithMessage("Não foi possível initalizar o idioma do sistema.")
                                .ShowAlert();

                    return;
                }

                ShowLoadingScreen();

                InitializePlugins();

                CloseLoadingScreen();

                KeepUIResponsive();

                if (singleProgramInstance.IsSingleInstance == false)
                {
                    SimpleAlerts.ShowInstanceAlreadyRunningAlert();
                    return;
                }

                StartApp();
            }

            Log.CloseAndFlush();
        }

        private static void InitializePlugins()
        {
            PluginSettings.InitializeContainer();
            InitializeLicenseManagerPlugin();
        }

        private static void CloseLoadingScreen()
        {
            _loadingThread.Abort();
            SplashScreen.Destroy();
        }

        private static void InitializeLicenseManagerPlugin()
        {
            PluginSettings.LicenceManager = PluginSettings.PluginContainer.GetFirstPluginOfType<ILicenseManager>();
        }

        private static void StartApp()
        {
            var intializeTerminalResult = TerminalService.InitializeTerminalAsync().Result;

            if (intializeTerminalResult.IsError)
            {
                ErrorHandlingService.HandleApiError(intializeTerminalResult, true);
                return;
            }

            PosLicenceDialog.GetLicenseDetails("Vision");

            StartFrontOffice();
        }

        private static void OldStartApp()
        {
            if (PluginSettings.LicenceManager != null)
            {
                LicenseRouter licenseRouter = new LicenseRouter();
            }
            else
            {
                Thread thread = new Thread(new ThreadStart(StartFrontOffice));
                LogicPOSAppContext.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.NotifyLoadingIsDone));
                thread.Start();

                LogicPOSAppContext.LoadingDialog = Utils.CreateSplashScreen();
                LogicPOSAppContext.LoadingDialog.Run();
            }
        }

        public static void StartFrontOffice()
        {
            LogicPOSAppUtils appUtils = new LogicPOSAppUtils();
            appUtils.StartApp();
        }

        public static void StartBackOffice()
        {
            LogicPOSAppUtils logicPos = new LogicPOSAppUtils();
            logicPos.StartApp();
        }

        public static bool InitializeCulture()
        {
            var meditator = DependencyInjection.Mediator;
            var getSystemInformationsResult = meditator.Send(new GetSystemInformationsQuery()).Result;

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