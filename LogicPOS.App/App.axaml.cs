using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LogicPOS.App.Views;
using LogicPOS.Core;

namespace LogicPOS.App;

public partial class App : global::Avalonia.Application
{
    /// <summary>
    /// Host/Desktop set this before StartWithClassicDesktopLifetime so DI + DB run after the splash paints.
    /// </summary>
    public static Action? Bootstrap { get; set; }

    public override void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            WriteCrash("UnhandledException", args.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            WriteCrash("UnobservedTaskException", args.Exception);
            args.SetObserved();
        };

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += (_, args) =>
        {
            WriteCrash("UIThread.UnhandledException", args.Exception);
            args.Handled = true;
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var loading = new StartupLoadingWindow();
            desktop.MainWindow = loading;
            loading.Opened += (_, _) => _ = RunBootstrapAsync(desktop, loading);
            base.OnFrameworkInitializationCompleted();
            return;
        }

        // Android: call base first so AvaloniaView is ready, then assign MainView once.
        // Setting MainView before base makes InitializeAvaloniaView re-parent the same control → blank crash.
        base.OnFrameworkInitializationCompleted();

        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            if (singleView.MainView is null || singleView.MainView.Parent is not null)
            {
                singleView.MainView = new MobileHostView();
            }
        }
    }

    private static async Task RunBootstrapAsync(
        IClassicDesktopStyleApplicationLifetime desktop,
        StartupLoadingWindow loading)
    {
        try
        {
            // Let the splash paint before heavy DB work blocks a thread.
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Loaded);
            await Task.Delay(50);

            // Cloud keeps appsettings across auto-update; do not flash "criar BD" when UseCloud is on.
            var databaseReady = DatabaseStartup.ShowFirstTimeDatabaseMessage() == false;
            await Dispatcher.UIThread.InvokeAsync(() => loading.ApplyMessage(databaseReady));

            await Task.Run(() =>
            {
                // Free disk space from leftover update packages under %TEMP%.
                ApplicationUpdater.PurgeStaleUpdateFolders();

                var bootstrap = Bootstrap ?? AppComposition.Configure;
                bootstrap();
            });
        }
        catch (Exception ex)
        {
            WriteCrash("Bootstrap", ex);
            if (string.IsNullOrWhiteSpace(AppComposition.StartupError))
            {
                AppComposition.UseExternal(null, ex.Message);
            }
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var login = new LoginShellWindow();
            desktop.MainWindow = login;
            // Close splash before Show(): Opened may touch ILicenseModule (branding) and must not
            // leave the loader stuck if a sync licence probe blocks briefly.
            loading.Close();
            login.Show();
        });
    }

    internal static void WriteCrash(string source, Exception? exception)
    {
        try
        {
            AppLogging.Fatal(exception, "Crash: {Source}", source);
            var path = Path.Combine(Path.GetTempPath(), "logicpos-login-crash.txt");
            var text = $"{DateTime.Now:O}{Environment.NewLine}{source}{Environment.NewLine}{exception}{Environment.NewLine}";
            File.AppendAllText(path, text);
            Debug.WriteLine(text);
        }
        catch
        {
            // Ignore logging failures.
        }
    }
}
