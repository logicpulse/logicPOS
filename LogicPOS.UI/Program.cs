using Gtk;
using logicpos;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Services;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WinFormsApplication = System.Windows.Forms.Application;
using WinFormsMessageBox = System.Windows.Forms.MessageBox;
using WinFormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using WinFormsMessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using WinFormsThreadExceptionEventArgs = System.Threading.ThreadExceptionEventArgs;
using WinFormsUnhandledExceptionMode = System.Windows.Forms.UnhandledExceptionMode;


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
            // Exceptions in GTK# callbacks (signals, native→managed) do not reach AppDomain.UnhandledException.
            GLib.ExceptionManager.UnhandledException += OnGlibUnhandledException;
        }

        private static void OnGlibUnhandledException(GLib.UnhandledExceptionArgs args)
        {
            var ex = args.ExceptionObject as Exception;
            if (ex != null)
            {
                TryLogSerilogFatalAndFlush(ex, "Unhandled GLib/GTK# callback exception. IsTerminating={IsTerminating}", args.IsTerminating);
            }
            else
            {
                TryLogSerilogFatalNoExceptionAndFlush(
                    "Unhandled GLib/GTK# callback exception (non-Exception). Object={ExceptionObject} IsTerminating={IsTerminating}",
                    args.ExceptionObject,
                    args.IsTerminating);
            }

            ShowFatalErrorMessageBox(ex ?? new Exception(args.ExceptionObject?.ToString() ?? "Unknown error"));
            args.ExitApplication = true;
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
                TryRegisterWinFormsThreadExceptionRouting();
                ConfigureGtkRuntime();
                ConfigureLogging();
                RegisterAppDomainAndTaskExceptionHandlers();
                Log.Information("Initializing application...");
#if DEBUG
                Log.Information($"Configuration: Debug");
#else
                Log.Information($"Configuration: Release");
#endif

                if (MigratorService.HasOldPosSqliteDatabase())
                {
                    Log.Information("Old pos sqlite database file detected...");
                    MigratorService.LaunchMigrator();
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
                SystemUpdateService.CreateUpdateXml();
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
                TryLogSerilogFatalAndFlush(ex, "An unhandled exception occurred in Main.");
                SimpleAlerts.Error()
                            .WithTitle("Erro inesperado")
                            .WithMessage($"Ocorreu um erro inesperado: \nPor favor, contacte o suporte técnico.")
                            .ShowAlert();
            }
            finally
            {
                CloseSplashScreen();
            }
        }

        private static bool ProgramIsAlreadyRunning()
        {
            var ins = Process.GetProcessesByName("logicpos");
            if (ins.Length > 1)
            {
                Log.Warning("Another instance is already running, exiting application.");
                return true;
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

        /// <summary>
        /// Must run before any windows are created (GTK or WinForms) so the mode can be applied.
        /// </summary>
        private static void TryRegisterWinFormsThreadExceptionRouting()
        {
            try
            {
                WinFormsApplication.SetUnhandledExceptionMode(WinFormsUnhandledExceptionMode.CatchException);
                WinFormsApplication.ThreadException += OnWinFormsThreadException;
            }
            catch (InvalidOperationException)
            {
                // Already configured (e.g. by another component); ignore.
            }
        }

        private static void RegisterAppDomainAndTaskExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static void OnWinFormsThreadException(object sender, WinFormsThreadExceptionEventArgs e)
        {
            TryLogSerilogFatalAndFlush(e.Exception, "Unhandled WinForms-routed thread exception.");
            ShowFatalErrorMessageBox(e.Exception);
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                TryLogSerilogFatalAndFlush(ex, "Unhandled AppDomain exception. IsTerminating={IsTerminating}", e.IsTerminating);
            }
            else
            {
                TryLogSerilogFatalNoExceptionAndFlush(
                    "Unhandled AppDomain exception (non-Exception). Object={ExceptionObject} IsTerminating={IsTerminating}",
                    e.ExceptionObject,
                    e.IsTerminating);
            }

            ShowFatalErrorMessageBox(ex ?? new Exception(e.ExceptionObject?.ToString() ?? "Unknown error"));
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                TryLogSerilogErrorAndFlush(e.Exception, "Unobserved task exception.");
                e.SetObserved();
            }
            catch
            {
                // Avoid throwing from the handler.
            }
        }

        /// <summary>
        /// Logs with Serilog and flushes sinks so entries are not lost when the process terminates immediately after.
        /// </summary>
        private static void TryLogSerilogFatalAndFlush(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Fatal(exception, messageTemplate, propertyValues);
                Log.CloseAndFlush();
            }
            catch
            {
                // Serilog or disk failure — do not rethrow from an exception handler.
            }
        }

        private static void TryLogSerilogFatalNoExceptionAndFlush(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Fatal(messageTemplate, propertyValues);
                Log.CloseAndFlush();
            }
            catch
            {
            }
        }

        private static void TryLogSerilogErrorAndFlush(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Log.Error(exception, messageTemplate, propertyValues);
                Log.CloseAndFlush();
            }
            catch
            {
            }
        }

        private static void ShowFatalErrorMessageBox(Exception ex)
        {
            string details = BuildUserFacingErrorDetails(ex);
            try
            {
                WinFormsMessageBox.Show(
                    "Ocorreu um erro não tratado. A aplicação pode encerrar." + Environment.NewLine + Environment.NewLine + "Contacte o suporte técnico.",
                    "Erro inesperado — LogicPOS",
                    WinFormsMessageBoxButtons.OK,
                    WinFormsMessageBoxIcon.Error);
            }
            catch
            {
                Debug.WriteLine(details);
            }
        }

        private static string BuildUserFacingErrorDetails(Exception ex)
        {
            if (ex == null)
            {
                return "(sem detalhes)";
            }

            var parts = ex.Message;
            if (ex.InnerException != null)
            {
                parts += Environment.NewLine + ex.InnerException.Message;
            }

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                parts += Environment.NewLine + Environment.NewLine + TruncateForDialog(ex.StackTrace, 1800);
            }

            return parts;
        }

        private static string TruncateForDialog(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength) + Environment.NewLine + "…";
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