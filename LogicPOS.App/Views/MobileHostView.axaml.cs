using Avalonia.Controls;
using Avalonia.Threading;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

/// <summary>
/// Android / single-view host: runs DI bootstrap then embeds the login UI.
/// </summary>
public partial class MobileHostView : UserControl
{
    private int _started;

    public MobileHostView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => _ = StartAsync();
    }

    private async Task StartAsync()
    {
        // AttachedToVisualTree can fire more than once on Android activity recreate.
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            return;
        }

        try
        {
            await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Loaded);
            await Task.Delay(50);

            var databaseReady = DatabaseStartup.ShowFirstTimeDatabaseMessage() == false;
            Status.Text = databaseReady
                ? "A abrir a base de dados…"
                : "A criar a base de dados…";

            // Leave the UI sync context so the post-bootstrap InvokeAsync cannot deadlock.
            await Task.Run(() =>
            {
                var bootstrap = App.Bootstrap ?? AppComposition.Configure;
                bootstrap();
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.WriteCrash("MobileHostView.Bootstrap", ex);
            if (string.IsNullOrWhiteSpace(AppComposition.StartupError))
            {
                AppComposition.UseExternal(null, ex.Message);
            }
        }

        try
        {
            LoginWindow? login = null;
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Status.Text = "A preparar o ecrã de login…";
                login = new LoginWindow();
                Host.Children.Clear();
                Host.Children.Add(login);
                DevEditionBanner.Attach(Host);
            });

            if (login is not null)
            {
                await Dispatcher.UIThread.InvokeAsync(login.ActivateHostedAsync);
            }
        }
        catch (Exception ex)
        {
            App.WriteCrash("MobileHostView.Login", ex);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Status.Text = $"Erro ao abrir login: {ex.Message}";
            });
        }
    }
}
