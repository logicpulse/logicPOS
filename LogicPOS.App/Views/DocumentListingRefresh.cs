using Avalonia.Controls;
using Avalonia.Threading;
using LogicPOS.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

internal static class DocumentListingRefresh
{
    private static readonly TimeSpan CloudRefreshInterval = TimeSpan.FromSeconds(15);
    private static event Action<Control>? Changed;

    public static void NotifyChanged(Control source) => Changed?.Invoke(source);

    public static void Attach(Control listing, Func<Task> reload, Func<bool> canPoll, Func<Task> poll, Func<bool>? isRelevant = null)
    {
        var timer = new DispatcherTimer { Interval = CloudRefreshInterval };
        var polling = false;

        void OnChanged(Control source)
        {
            if (source != listing && listing.IsEffectivelyVisible && (isRelevant?.Invoke() ?? true))
            {
                _ = reload();
            }
        }

        timer.Tick += async (_, _) =>
        {
            if (polling || listing.IsEffectivelyVisible == false || canPoll() == false)
            {
                return;
            }

            polling = true;
            try
            {
                await poll();
            }
            finally
            {
                polling = false;
            }
        };

        listing.AttachedToVisualTree += (_, _) =>
        {
            Changed += OnChanged;
            var configuration = AppComposition.Services?.GetService<IConfiguration>();
            if (bool.TryParse(configuration?["DatabaseSettings:UseCloud"], out var useCloud) && useCloud)
            {
                timer.Start();
            }
        };
        listing.DetachedFromVisualTree += (_, _) =>
        {
            timer.Stop();
            Changed -= OnChanged;
        };
    }
}
