using Avalonia.Controls;
using Avalonia.Threading;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

/// <summary>
/// Android / single-view host: runs DI bootstrap then embeds the login UI.
/// </summary>
public partial class MobileHostView : UserControl
{
    private static MobileHostView? _current;
    private int _started;
    private LoginWindow? _login;

    public MobileHostView()
    {
        InitializeComponent();
        _current = this;
        DevEditionBanner.Apply(DevBanner);
        AttachedToVisualTree += (_, _) =>
        {
            _current = this;
            _ = StartAsync();
        };
        DetachedFromVisualTree += (_, _) =>
        {
            if (ReferenceEquals(_current, this))
            {
                _current = null;
            }
        };
    }

    /// <summary>
    /// Swap the hosted view after a successful login (POS / BackOffice as UserControl).
    /// </summary>
    public static void ShowSession(Control session, Action? onLoggedOut = null)
    {
        var host = _current;
        if (host is null)
        {
            return;
        }

        RunOnUi(() =>
        {
            SetHostedContentCore(host, session);
            if (onLoggedOut is null)
            {
                return;
            }

            if (session is PosWindow pos)
            {
                pos.LoggedOut += (_, _) =>
                {
                    onLoggedOut();
                    host.ShowLoginAgain();
                };
            }
            else if (session is BackOfficeWindow office)
            {
                office.LoggedOut += (_, _) =>
                {
                    onLoggedOut();
                    host.ShowLoginAgain();
                };
            }
        });
    }

    /// <summary>
    /// Replace the current hosted control without attaching logout handlers (POS ↔ BO).
    /// </summary>
    public static void SetHostedContent(Control content)
    {
        var host = _current;
        if (host is null)
        {
            return;
        }

        RunOnUi(() => SetHostedContentCore(host, content));
    }

    /// <summary>
    /// Mostra o overlay de loading por cima do conteúdo actual (login permanece por baixo).
    /// Chamar imediatamente no toque em Entrar, antes de auth / construção de vistas.
    /// </summary>
    public static void ShowLoadingOverlay(string message)
    {
        RunOnUi(() =>
        {
            var host = _current;
            if (host is null)
            {
                return;
            }

            host.Status.Text = message;
            host.LoadingLogo.IsVisible = true;
            host.LoadingBar.IsVisible = true;
            host.LoadingBar.IsIndeterminate = true;
            host.LoadingOverlay.IsVisible = true;
            host.LoadingOverlay.IsHitTestVisible = true;
        });
    }

    /// <summary>
    /// Actualiza o texto de progresso sem esconder o overlay.
    /// </summary>
    public static void UpdateLoadingMessage(string message)
    {
        RunOnUi(() =>
        {
            var host = _current;
            if (host is null || host.LoadingOverlay.IsVisible == false)
            {
                return;
            }

            host.Status.Text = message;
        });
    }

    /// <summary>
    /// Esconde o overlay e devolve o toque ao conteúdo do Host.
    /// </summary>
    public static void HideLoadingOverlay()
    {
        RunOnUi(() =>
        {
            var host = _current;
            if (host is null)
            {
                return;
            }

            host.LoadingBar.IsIndeterminate = false;
            host.LoadingOverlay.IsVisible = false;
            host.LoadingOverlay.IsHitTestVisible = false;
        });
    }

    /// <summary>
    /// Cede o UI thread para o Avalonia pintar o overlay antes de trabalho pesado.
    /// </summary>
    public static async Task YieldForPaintAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Task.Delay(50);
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Loaded);
    }

    private void ShowLoginAgain()
    {
        RunOnUi(() =>
        {
            Host.Children.Clear();
            if (_login is null)
            {
                _login = new LoginWindow();
                _ = _login.ActivateHostedAsync();
            }
            else
            {
                _login.IsVisible = true;
            }

            Host.Children.Add(_login);
            HideLoadingOverlayCore(this);
        });
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
            ShowLoadingOverlay("A carregar logicPOS…");
            await YieldForPaintAsync();

            var databaseReady = DatabaseStartup.ShowFirstTimeDatabaseMessage() == false;
            ShowLoadingOverlay(databaseReady
                ? "A abrir a base de dados…"
                : "A criar a base de dados…");

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
            // Após ConfigureAwait(false) a continuação está numa thread pool —
            // criar LoginWindow, activar e esconder overlay só na UI thread.
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                ShowLoadingOverlayCore(this, "A preparar o ecrã de login…");
                _login = new LoginWindow();
                Host.Children.Clear();
                Host.Children.Add(_login);
                await _login.ActivateHostedAsync();
                HideLoadingOverlayCore(this);
            });
        }
        catch (Exception ex)
        {
            App.WriteCrash("MobileHostView.Login", ex);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadingBar.IsIndeterminate = false;
                LoadingBar.IsVisible = false;
                LoadingLogo.IsVisible = false;
                Status.Text = $"Erro ao abrir login: {ex.Message}";
                LoadingOverlay.IsVisible = true;
            });
        }
    }

    /// <summary>
    /// Garante que mutações de controlos Avalonia correm no Dispatcher.UIThread.
    /// </summary>
    private static void RunOnUi(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }

        Dispatcher.UIThread.Post(action);
    }

    private static void SetHostedContentCore(MobileHostView host, Control content)
    {
        host.Host.Children.Clear();
        host.Host.Children.Add(content);
    }

    private static void ShowLoadingOverlayCore(MobileHostView host, string message)
    {
        host.Status.Text = message;
        host.LoadingLogo.IsVisible = true;
        host.LoadingBar.IsVisible = true;
        host.LoadingBar.IsIndeterminate = true;
        host.LoadingOverlay.IsVisible = true;
        host.LoadingOverlay.IsHitTestVisible = true;
    }

    private static void HideLoadingOverlayCore(MobileHostView host)
    {
        host.LoadingBar.IsIndeterminate = false;
        host.LoadingOverlay.IsVisible = false;
        host.LoadingOverlay.IsHitTestVisible = false;
    }
}
