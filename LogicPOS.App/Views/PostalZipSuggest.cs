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
/// Debounced postal-code suggestions under a zip text box. Picks fill the code before LostFocus runs.
/// </summary>
public static class PostalZipSuggest
{
    public static bool IgnoreLostFocus { get; private set; }

    private static int _suppressSearch;

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

        if (address is not null && string.IsNullOrWhiteSpace(match.Street) == false)
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

        var ticket = 0;
        var applying = false;
        box.TextChanged += async (_, _) =>
        {
            if (applying || _suppressSearch > 0)
            {
                return;
            }

            var term = box.Text?.Trim() ?? string.Empty;
            var mine = ++ticket;
            if (term.Length < 2)
            {
                list.Children.Clear();
                popup.IsOpen = false;
                return;
            }

            try
            {
                await Task.Delay(220);
                if (mine != ticket || applying)
                {
                    return;
                }

                var matches = await search(term);
                if (mine != ticket || applying)
                {
                    return;
                }

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (mine != ticket || applying)
                    {
                        return;
                    }

                    list.Children.Clear();
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
                            ticket++;
                            IgnoreLostFocus = true;
                            applying = true;
                            try
                            {
                                apply(chosen);
                            }
                            finally
                            {
                                applying = false;
                            }

                            list.Children.Clear();
                            popup.IsOpen = false;
                            args.Handled = true;
                            Dispatcher.UIThread.Post(() => IgnoreLostFocus = false, DispatcherPriority.Background);
                        }, RoutingStrategies.Tunnel, handledEventsToo: true);
                        list.Children.Add(button);
                    }

                    popup.IsOpen = list.Children.Count > 0;
                });
            }
            catch (Exception)
            {
                await Dispatcher.UIThread.InvokeAsync(() => popup.IsOpen = false);
            }
        };
    }
}
