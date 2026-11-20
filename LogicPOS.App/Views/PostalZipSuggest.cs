using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>
/// Postal-code pickers: no live API calls on keystrokes; explicit lists use <see cref="ShowSuggestions"/>.
/// </summary>
public static class PostalZipSuggest
{
    public static bool IgnoreLostFocus { get; private set; }

    private static int _suppressSearch;

    private sealed class SuggestHost
    {
        public required StackPanel List { get; init; }

        public required Popup Popup { get; init; }

        public required Action<PostalLocalityMatch> Apply { get; set; }

        public int Ticket { get; set; }
    }

    private static readonly ConditionalWeakTable<TextBox, SuggestHost> Hosts = new();

    public static async Task<IReadOnlyList<PostalLocalityMatch>> SearchCompanyAsync(string term)
    {
        var listing = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (listing is null)
        {
            return [];
        }

        return await listing.SearchCompanyPostalLocalitiesAsync(term, 12);
    }

    public static void RunWithoutSearch(Action action)
    {
        _suppressSearch++;
        try
        {
            action();
        }
        finally
        {
            Dispatcher.UIThread.Post(() => _suppressSearch--, DispatcherPriority.Background);
        }
    }

    public static void Apply(
        PostalLocalityMatch match,
        TextBox? postal,
        TextBox? address,
        TextBox? locality,
        TextBox? city,
        TextBox? region = null)
    {
        _suppressSearch++;
        try
        {
            if (postal is not null)
            {
                postal.Text = match.ZipCode;
            }

            if (locality is not null)
            {
                locality.Text = match.Locality;
            }

            if (city is not null)
            {
                city.Text = match.City;
            }

            if (address is not null
                && string.IsNullOrWhiteSpace(address.Text)
                && string.IsNullOrWhiteSpace(match.Street) == false)
            {
                address.Text = match.Street;
            }

            if (region is not null
                && string.IsNullOrWhiteSpace(region.Text)
                && string.IsNullOrWhiteSpace(match.Locality) == false
                && string.Equals(match.Locality, match.City, StringComparison.OrdinalIgnoreCase) == false)
            {
                region.Text = match.Locality;
            }
        }
        finally
        {
            Dispatcher.UIThread.Post(() => _suppressSearch--, DispatcherPriority.Background);
        }
    }

    public static void Attach(
        TextBox box,
        Panel host,
        Func<string, Task<IReadOnlyList<PostalLocalityMatch>>> search,
        Action<PostalLocalityMatch> apply)
    {
        var state = EnsureHost(box, host, apply);
        _ = search;
        _ = state;
    }

    public static void ShowSuggestions(
        TextBox box,
        Panel host,
        IReadOnlyList<PostalLocalityMatch> matches,
        Action<PostalLocalityMatch> apply)
    {
        var state = EnsureHost(box, host, apply);
        var mine = ++state.Ticket;
        Dispatcher.UIThread.Post(() =>
        {
            if (mine != state.Ticket)
            {
                return;
            }

            state.List.Children.Clear();
            foreach (var match in matches.Take(12))
            {
                var chosen = match;
                var button = new Button
                {
                    Classes = { "bo_report_row" },
                    Content = chosen.Label,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                button.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
                {
                    state.Ticket++;
                    IgnoreLostFocus = true;
                    try
                    {
                        apply(chosen);
                    }
                    finally
                    {
                        state.List.Children.Clear();
                        state.Popup.IsOpen = false;
                        args.Handled = true;
                        Dispatcher.UIThread.Post(() => IgnoreLostFocus = false, DispatcherPriority.Background);
                    }
                }, RoutingStrategies.Tunnel, handledEventsToo: true);
                state.List.Children.Add(button);
            }

            state.Popup.IsOpen = state.List.Children.Count > 0;
        });
    }

    private static SuggestHost EnsureHost(TextBox box, Panel host, Action<PostalLocalityMatch> apply)
    {
        if (Hosts.TryGetValue(box, out var existing))
        {
            existing.Apply = apply;
            return existing;
        }

        var list = new StackPanel();
        var popup = new Popup
        {
            PlacementTarget = box,
            Placement = PlacementMode.Bottom,
            IsLightDismissEnabled = true,
            Child = new Border
            {
                Classes = { "bo_new_doc_suggest" },
                MinWidth = 320,
                Child = new ScrollViewer
                {
                    MaxHeight = 220,
                    Content = list
                }
            }
        };
        if (host is DockPanel)
        {
            DockPanel.SetDock(popup, Dock.Left);
            host.Children.Insert(0, popup);
        }
        else
        {
            host.Children.Add(popup);
        }

        var created = new SuggestHost
        {
            List = list,
            Popup = popup,
            Apply = apply
        };
        Hosts.Add(box, created);
        return created;
    }
}