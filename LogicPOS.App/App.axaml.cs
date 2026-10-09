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
        }

        base.OnFrameworkInitializationCompleted();
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

            var databaseExists = DatabaseStartup.SqliteDatabaseExists();
            await Dispatcher.UIThread.InvokeAsync(() => loading.ApplyMessage(databaseExists));

            await Task.Run(() =>
            {
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
            var login = new LoginWindow();
            desktop.MainWindow = login;
            login.Show();
            loading.Close();
        });
    }

    internal static void WriteCrash(string source, Exception? exception)
    {
        try
        {
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
