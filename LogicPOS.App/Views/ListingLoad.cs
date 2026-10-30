using Avalonia.Controls;
using Avalonia.Threading;

namespace LogicPOS.App.Views;

internal static class ListingNotice
{
    public static void HideWhenEmpty(TextBlock block)
    {
        void Apply() => block.IsVisible = string.IsNullOrEmpty(block.Text) == false;
        block.PropertyChanged += (_, args) =>
        {
            if (args.Property == TextBlock.TextProperty)
            {
                Apply();
            }
        };
        Apply();
    }
}

internal sealed class ListingLoad
{
    private readonly Control _overlay;
    private int _depth;

    public ListingLoad(Control overlay) => _overlay = overlay;

    public async Task RunAsync(Func<Task> action)
    {
        _depth++;
        if (_depth == 1)
        {
            // Show immediately — a delay made Documentos look like it had no loading
            // when ListByPeriod finished under ~180ms.
            ShowOverlay();
        }

        try
        {
            await action();
        }
        finally
        {
            _depth--;
            if (_depth <= 0)
            {
                _depth = 0;
                HideOverlay();
            }
        }
    }

    private void ShowOverlay()
    {
        void Apply()
        {
            if (_overlay is ListingBusy busy)
            {
                busy.Show();
                return;
            }

            _overlay.IsVisible = true;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Apply();
        }
        else
        {
            Dispatcher.UIThread.Post(Apply);
        }
    }

    private void HideOverlay()
    {
        void Apply()
        {
            if (_overlay is ListingBusy busy)
            {
                busy.Hide();
                return;
            }

            _overlay.IsVisible = false;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Apply();
        }
        else
        {
            Dispatcher.UIThread.Post(Apply);
        }
    }
}
