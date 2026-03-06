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
        private static Thread _spashScreenThread;
        private static bool _splashClosed = false;
        private static bool _splashShown = false;
        public static Dialog SplashScreen { get; set; }

        public static void InitializeGtk()
        {
            Log.Information("Initializing GTK...");
            Gtk.Application.Init();
            GtkThemeStyle.ParseTheme();
        }

        public static void ShowLoadingScreen()
        {
            Log.Information("Showing loading screen...");
            SplashScreen = Utils.CreateSplashScreen();
            _spashScreenThread = new Thread(() => SplashScreen.Show());
            _spashScreenThread.Start();
            _splashShown = true;
        }

        private static void CloseSplashScreen()
        {
            if (_splashClosed || _splashShown == false)
            {
                return;
            }
            _spashScreenThread.Abort();
            SplashScreen.Destroy();
            Log.Information("Loading screen closed.");
            _splashClosed = true;
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
            try
            {
                ConfigureGtkRuntime();
                ConfigureLogging();
                Log.Information("Initializing application...");

                if (IsFirstLaunch())
                {
                    Log.Information("First launch detected, starting migrator...");
                    MigratorUtility.LaunchMigrator();
                }

                if (ProgramIsAlreadyRunning())
                {
                    Quit();
                    return;
                }

                InitializeGtk();
                ShowLoadingScreen();

                if (DependencyInjection.Initialize() == false)
                {
                    Log.Fatal("Failed to initialize dependency injection.");
                    Quit();
                    return;
                }

                SystemVersionService.Initialize();
                SystemVersionService.CreateUpdateXml();
                ShowVersionAlerts();

                if (InitializeCulture() == false)
                {
                    Quit();
                    return;
                }

                CloseSplashScreen();
                KeepUIResponsive();
                StartApp();

                Log.Information("Application exiting gracefully.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred.");
                SimpleAlerts.Error()
                            .WithTitle("Erro inesperado")
                            .WithMessage($"Ocorreu um erro inesperado: {ex.Message}")
                            .ShowAlert();
            }
            finally
            {
                CloseSplashScreen();
            }
        }

        private static bool ProgramIsAlreadyRunning()
        {
            using (var singleProgramInstance = new SingleProgramInstance())
            {
                if (singleProgramInstance.IsSingleInstance == false)
                {
                    Log.Warning("Another instance is already running, exiting application.");
                    SimpleAlerts.ShowInstanceAlreadyRunningAlert();
                    return true;
                }
            }

            return false;
        }

        private static void ConfigureGtkRuntime()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string gtkPath = Path.Combine(baseDir, "GtkRuntime");
            Environment.SetEnvironmentVariable("GTK_BASEPATH", gtkPath);
            string gtkBinPath = Path.Combine(gtkPath, "bin");
            string currentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            Environment.SetEnvironmentVariable("PATH", $"{currentPath};{gtkBinPath}");
        }

        private static void ShowVersionAlerts()
        {
            if (SystemVersionService.ApiVersion != SystemVersionService.PosVersion)
            {
                SimpleAlerts.Warning()
                       .WithTitle("Atenção")
                       .WithMessage($"A versão da API ({SystemVersionService.ApiVersion}) difere da versão do aplicativo ({SystemVersionService.PosVersion}).\n Algumas partes do sistema podem não funcionar como esperado, convém usar versões iguais.")
                       .ShowAlert();
            }

            if (SystemVersionService.PosHasUpdate || SystemVersionService.ApiHasUpdate)
            {
                var message= $"Há uma actualização disponível para o sistema: versão {SystemVersionService.LastestVersion}\n\n" +
                             $"Versão atual do aplicativo: {SystemVersionService.PosVersion}\n" +
                             $"Versão atual da API: {SystemVersionService.ApiVersion}\n\n" +
                             $"Recomenda-se actualizar para a última versão para garantir a melhor experiência e acesso a novos recursos.";
                SimpleAlerts.Warning()
                            .WithTitle("Atenção")
                            .WithMessage(message)
                            .ShowAlert();
            }
        }

        private static bool IsFirstLaunch()
        {
            return File.Exists("terminal.id") == false;
        }

        public static void Quit()
        {
            Log.Information("Quitting application...");
            CloseSplashScreen();
            Gtk.Application.Quit();
            Environment.Exit(0);
        }

        private static void StartApp()
        {
            Log.Information("Starting application...");

            if (LicensingService.Initialize() == false)
            {
                return;
            }

            var terminalResult = TerminalService.InitializeTerminal();
            if (terminalResult.IsError)
            {
                Log.Error("Failed to initialize terminal: {Error}", string.Join(Environment.NewLine, terminalResult.FirstError.Description));
                SimpleAlerts.Error()
                            .WithTitle("Erro ao inicializar terminal")
                            .WithMessage(string.Join(Environment.NewLine, terminalResult.FirstError.Description))
                            .ShowAlert();
                return;
            }

            if (LicensingService.Data.IsLicensed == false)
            {
                Log.Warning("System no lincesed, showing registration modal...");
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
                    Log.Warning("No internet connection detected, checking for offline activation file...");
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
                Log.Information("Initializing culture...");
                if (SystemInformationService.SystemInformation == null)
                {
                    return false;
                }

                var culture = SystemInformationService.SystemInformation.Culture;
                LocalizedString.Instance = new LocalizedString(culture);
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
                Log.Information($"Culture initialized: {culture}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize culture.");
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
                              .WriteTo.File(
                                  path: "Logs/log.txt",
                                  rollingInterval: RollingInterval.Day,
                                  retainedFileCountLimit: 7,
                                  outputTemplate:
                                  "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}"
                                  )
                              .CreateLogger();
        }
    }
}