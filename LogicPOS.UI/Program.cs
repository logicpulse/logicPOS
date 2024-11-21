using Gtk;
using logicpos;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.License;
using LogicPOS.Api.Features.System.GetSystemInformations;
using LogicPOS.Globalization;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Globalization;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace LogicPOS.UI
{

    internal class Program
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Thread _loadingThread;
        public static Dialog SplashScreen { get; set; }

        public static void InitializeSettings()
        {
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
        }

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
            using (var singleProgramInstance = new SingleProgramInstance())
            {
                PathsSettings.InitializePaths();

                InitializeGtk();

                if (true)
                {
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
            }
        }

        private static void InitializePlugins()
        {
            PluginSettings.InitializeContainer();
            InitializeSoftwareVendorPlugin();
            InitializeStockModule();
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

        private static void InitializeStockModule()
        {
            ModulesSettings.StockManagementModule = PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>();
        }

        private static void InitializeSoftwareVendorPlugin()
        {
            PluginSettings.SoftwareVendor = PluginSettings.PluginContainer.GetFirstPluginOfType<ISoftwareVendor>();

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                PluginSettings.InitializeSoftwareVendorPluginSettings();
            }
        }

        private static void StartApp()
        {
            var intializeTerminalResult = TerminalService.InitializeTerminalAsync().Result;

            if (intializeTerminalResult.IsError)
            {
                ErrorHandlingService.HandleApiError(intializeTerminalResult.FirstError,true);
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
            appUtils.StartApp(AppMode.FrontOffice);
        }

        public static void StartBackOffice()
        {
            LogicPOSAppUtils logicPos = new LogicPOSAppUtils();
            logicPos.StartApp(AppMode.Backoffice);
        }

        public static bool InitializeCulture()
        {
            var meditator = DependencyInjection.Services.GetRequiredService<ISender>();
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


#if (DEBUG)
        public static readonly bool DebugMode = true;
#else
        public static readonly bool DebugMode = false;
#endif

    }
}