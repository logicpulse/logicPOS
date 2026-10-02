using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace LogicPOS.App;

internal enum ToastKind
{
    Success,
    Error,
    Warning,
    Info
}

internal static class Toast
{
    private static readonly TimeSpan Duration = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan Tick = TimeSpan.FromMilliseconds(50);
    private static readonly Dictionary<TopLevel, StackPanel> Hosts = new();

    public static void Success(Visual? source, string title) => Show(source, ToastKind.Success, title);

    public static void Error(Visual? source, string title) => Show(source, ToastKind.Error, title);

    public static void Warning(Visual? source, string title) => Show(source, ToastKind.Warning, title);

    public static void Show(Visual? source, ToastKind kind, string title)
    {
        if (source is null || string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        var host = ResolveHost(TopLevel.GetTopLevel(source));
        if (host is null)
        {
            return;
        }

        var progress = new Border { Classes = { "app_toast_progress" } };
        var close = new Button { Classes = { "app_toast_close" }, Content = "×" };
        var body = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") };
        body.Children.Add(new Image { Classes = { "app_toast_icon" }, Source = LoadIcon(kind) });
        var text = new TextBlock { Classes = { "app_toast_title" }, Text = title };
        Grid.SetColumn(text, 1);
        body.Children.Add(text);
        Grid.SetColumn(close, 2);
        body.Children.Add(close);

        var layout = new DockPanel();
        DockPanel.SetDock(progress, Dock.Bottom);
        layout.Children.Add(progress);
        layout.Children.Add(body);

        var card = new Border { Classes = { "app_toast", ToastClass(kind) }, Child = layout };
        host.Children.Add(card);

        var remaining = Duration;
        var paused = false;
        var timer = new DispatcherTimer { Interval = Tick };

        void Dismiss()
        {
            timer.Stop();
            host.Children.Remove(card);
        }

        timer.Tick += (_, _) =>
        {
            if (paused)
            {
                return;
            }

            remaining -= Tick;
            if (remaining <= TimeSpan.Zero)
            {
                Dismiss();
                return;
            }

            progress.Width = card.Bounds.Width * (remaining.TotalMilliseconds / Duration.TotalMilliseconds);
        };

        card.PointerEntered += (_, _) => paused = true;
        card.PointerExited += (_, _) => paused = false;
        close.Click += (_, _) => Dismiss();
        timer.Start();
    }

    private static StackPanel? ResolveHost(TopLevel? topLevel)
    {
        if (topLevel is null)
        {
            return null;
        }

        if (Hosts.TryGetValue(topLevel, out var existing))
        {
            return existing;
        }

        if (topLevel.Content is not Visual content || AdornerLayer.GetAdornerLayer(content) is not { } layer)
        {
            return null;
        }

        var host = new StackPanel { Classes = { "app_toast_host" } };
        AdornerLayer.SetAdornedElement(host, content);
        layer.Children.Add(host);
        Hosts[topLevel] = host;
        topLevel.Closed += (_, _) => Hosts.Remove(topLevel);
        return host;
    }

    private static string ToastClass(ToastKind kind) => kind switch
    {
        ToastKind.Success => "app_toast_success",
        ToastKind.Error => "app_toast_error",
        ToastKind.Warning => "app_toast_warning",
        _ => "app_toast_info"
    };

    private static Bitmap LoadIcon(ToastKind kind)
    {
        var file = kind switch
        {
            ToastKind.Success => "notificacao_info_sucesso.png",
            ToastKind.Error => "notificacao_info_erro.png",
            ToastKind.Warning => "notificacao_info_alerta.png",
            _ => "notificacao_info_informacao.png"
        };
        return new Bitmap(AssetLoader.Open(new Uri($"avares://logicpos/Assets/Images/Notifications/{file}")));
    }
}
