using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.App.Themes;
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
    /// Host applies first-run edition (writes appsettings, may download API/webapp, re-composes DI).
    /// </summary>
    public static Func<ProductEdition, ProductHosting, string?, string?, string?, CloudClientSurface?, LicenseRegistrationFields?, Task<string?>>? ApplyProductEditionAsync { get; set; }

    public sealed record LicenseRegistrationFields(
        string Name,
        string Company,
        string FiscalNumber,
        string Email,
        string SoftwareKey);

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
                if (IsRepeatingLayoutCrash(args.Exception))
                    RecoverBlockedReports();
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

            LicenseRegistrationFields? registration = null;
            if (wizard.SelectedEdition == ProductEdition.Business
                && string.IsNullOrWhiteSpace(wizard.SoftwareKey) == false
                && string.IsNullOrWhiteSpace(wizard.RegistrationName) == false
                && string.IsNullOrWhiteSpace(wizard.RegistrationCompany) == false
                && string.IsNullOrWhiteSpace(wizard.RegistrationFiscalNumber) == false
                && string.IsNullOrWhiteSpace(wizard.RegistrationEmail) == false)
            {
                registration = new LicenseRegistrationFields(
                    wizard.RegistrationName!,
                    wizard.RegistrationCompany!,
                    wizard.RegistrationFiscalNumber!,
                    wizard.RegistrationEmail!,
                    wizard.SoftwareKey!);
            }

            var error = await ApplyProductEditionAsync!(
                wizard.SelectedEdition,
                wizard.SelectedHosting,
                wizard.ApiAddress,
                wizard.ClientId,
                wizard.SoftwareKey,
                wizard.SelectedEdition == ProductEdition.Cloud ? wizard.CloudSurface : null,
                registration).ConfigureAwait(true);
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

        await ApplyButtonGlossAsync().ConfigureAwait(true);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var login = new LoginShellWindow();
            desktop.MainWindow = login;
            login.Show();
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        });
    }

    private static async Task ApplyButtonGlossAsync()
    {
        try
        {
            var listing = AppComposition.Services?.GetService(typeof(IBackOfficeListingService)) as IBackOfficeListingService;
            string? value = null;
            if (listing is not null)
            {
                value = await listing.GetPreferenceValueAsync(GlassButtonTheme.PreferenceToken).ConfigureAwait(false);
            }

            await Dispatcher.UIThread.InvokeAsync(() => GlassButtonTheme.ApplyFromPreference(value));
        }
        catch (Exception exception)
        {
            WriteCrash("ApplyButtonGlossAsync", exception);
            await Dispatcher.UIThread.InvokeAsync(() => GlassButtonTheme.Apply(GlassButtonTheme.DefaultLevel));
        }
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

    private static int _layoutRecovery;

    /// <summary>
    /// Avalonia remeasures a broken Grid every frame. Marking the exception handled
    /// leaves the reports scrim white and blocks input until that overlay is hidden.
    /// </summary>
    private static bool IsRepeatingLayoutCrash(Exception? exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is ArgumentOutOfRangeException
                && current.StackTrace?.Contains("Grid.MeasureCellsGroup", StringComparison.Ordinal) == true)
            {
                return true;
            }
        }

        return false;
    }

    private static void RecoverBlockedReports()
    {
        if (Interlocked.Exchange(ref _layoutRecovery, 1) == 1)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                var window = (global::Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                if (window is null)
                    return;

                var hidden = false;
                foreach (var visual in window.GetVisualDescendants().OfType<Control>())
                {
                    if (visual.Name is "ReportsOverlay" or "FiltersOverlay" && visual.IsVisible)
                    {
                        visual.IsVisible = false;
                        hidden = true;
                    }
                }

                if (hidden)
                    Toast.Warning(window, "Não foi possível mostrar os relatórios. Volte a tentar.");
            }
            catch (Exception exception)
            {
                WriteCrash("RecoverBlockedReports", exception);
            }
            finally
            {
                Interlocked.Exchange(ref _layoutRecovery, 0);
            }
        }, DispatcherPriority.Send);
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
