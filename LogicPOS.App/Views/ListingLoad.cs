using Avalonia.Controls;

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
        var gate = new CancellationTokenSource();
        if (_depth == 1)
        {
            _ = ShowWhenSlowAsync(gate.Token);
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
                gate.Cancel();
                _overlay.IsVisible = false;
            }
        }
    }

    private async Task ShowWhenSlowAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(180, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (cancellationToken.IsCancellationRequested == false && _depth > 0)
        {
            _overlay.IsVisible = true;
        }
    }
}
