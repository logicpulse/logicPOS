using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LogicPOS.App.Views;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.App;

public partial class App : global::Avalonia.Application
{
    /// <summary>
    /// Host/Desktop set this before StartWithClassicDesktopLifetime so DI + DB run after the splash paints.
    /// </summary>
    public static Action? Bootstrap { get; set; }

    /// <summary>
    /// Host applies first-run edition (writes appsettings, may ADDLOCAL API/portal, re-composes DI).
    /// </summary>
    public static Func<ProductEdition, ProductHosting, string?, string?, string?, Task<string?>>? ApplyProductEditionAsync { get; set; }

    /// <summary>
    /// Android: MainActivity define esta acção para chamar FinishAffinity + kill.
    /// Login/POS/BO invocam-na quando IClassicDesktopStyleApplicationLifetime não existe.
    /// </summary>
    public static Action? ExitApplication { get; set; }

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

        // Avoid OnMainWindowClose killing the process while we swap splash → edition → login.
        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Close splash before dialogs: Opened may touch ILicenseModule (branding).
            loading.Close();
        });

        var configuration = AppComposition.Services?
            .GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration))
            as Microsoft.Extensions.Configuration.IConfiguration;
        var needsEdition = ApplyProductEditionAsync is not null
            && (configuration is null || ProductEditionSettings.HasEdition(configuration) == false);

        if (needsEdition)
        {
            // Wait for Closed outside InvokeAsync so the UI thread keeps pumping input.
            var wizard = await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var window = new EditionSetupWindow();
                desktop.MainWindow = window;
                window.Show();
                return window;
            });

            var closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            wizard.Closed += (_, _) => closed.TrySetResult(true);
            await closed.Task.ConfigureAwait(true);

            if (wizard.Completed == false)
            {
                desktop.Shutdown();
                return;
            }

            var error = await ApplyProductEditionAsync!(
                wizard.SelectedEdition,
                wizard.SelectedHosting,
                wizard.ApiAddress,
                wizard.ClientId,
                wizard.SoftwareKey).ConfigureAwait(true);
            if (string.IsNullOrWhiteSpace(error) == false)
            {
                // Still continue — Business server without licence works as local until activated.
                AppLogging.Warning(null, "First-run edition apply: {Error}", error);
            }

            // Re-compose so Edition/Hosting/licence gates match the choice.
            await Task.Run(() =>
            {
                var bootstrap = Bootstrap ?? AppComposition.Configure;
                bootstrap();
            }).ConfigureAwait(true);
        }

        if (await NeedsFirstUserAsync().ConfigureAwait(true))
        {
            var firstUser = await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var window = new FirstUserSetupWindow();
                desktop.MainWindow = window;
                window.Show();
                return window;
            });

            var closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            firstUser.Closed += (_, _) => closed.TrySetResult(true);
            await closed.Task.ConfigureAwait(true);

            if (firstUser.Completed == false)
            {
                desktop.Shutdown();
                return;
            }
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var login = new LoginShellWindow();
            desktop.MainWindow = login;
            login.Show();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        });
    }

    private static async Task<bool> NeedsFirstUserAsync()
    {
        var wizard = AppComposition.Services?.GetService(typeof(IFirstUserSetupWizard)) as IFirstUserSetupWizard;
        if (wizard is null)
            return false;

        try
        {
            return await wizard.NeedsSetupAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            WriteCrash("NeedsFirstUserAsync", exception);
            return false;
        }
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
