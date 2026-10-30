using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public sealed class ListingFilterChoice
{
    public Guid Id { get; init; }

    public string Label { get; init; } = "Todos";

    public string? Code { get; init; }

    public string? Detail { get; init; }

    public string SearchText { get; init; } = string.Empty;

    public override string ToString() => Label;
}

public static class ListingFilters
{
    public static Guid? CustomerId(AutoCompleteBox box)
    {
        var text = (box.Text ?? string.Empty).Trim();
        if (text.Length == 0 || text.Equals("Todos", StringComparison.CurrentCultureIgnoreCase))
        {
            return null;
        }

        return box.SelectedItem is ListingFilterChoice choice && choice.Id != Guid.Empty ? choice.Id : null;
    }

    public static string? TypeCode(ComboBox box)
        => box.SelectedItem is ListingFilterChoice choice && string.IsNullOrWhiteSpace(choice.Code) == false
            ? choice.Code
            : null;

    public static void EnableCustomerSearch(AutoCompleteBox box)
    {
        box.FilterMode = AutoCompleteFilterMode.Custom;
        box.ItemFilter = MatchCustomer;
        box.MinimumPrefixLength = 0;
        box.IsTextCompletionEnabled = false;
        box.ItemTemplate = new FuncDataTemplate<ListingFilterChoice>((choice, _) =>
        {
            var panel = new StackPanel { Spacing = 0 };
            panel.Children.Add(new TextBlock
            {
                Classes = { "bo_doc_filter_search_name" },
                Text = choice?.Label ?? string.Empty
            });
            if (string.IsNullOrWhiteSpace(choice?.Detail) == false)
            {
                panel.Children.Add(new TextBlock
                {
                    Classes = { "bo_doc_filter_search_meta" },
                    Text = choice.Detail
                });
            }

            return panel;
        });
        box.GotFocus += (_, _) =>
        {
            var text = box.Text ?? string.Empty;
            var selected = (box.SelectedItem as ListingFilterChoice)?.Label;
            if (string.IsNullOrEmpty(text) || text == "Todos" || text == selected)
            {
                box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault()?.SelectAll();
            }

            OpenDropDownAfterFocus(box);
        };
        AttachDropChevron(box);
    }

    public static void SelectEveryone(AutoCompleteBox box)
    {
        ListingFilterChoice? everyone = null;
        if (box.ItemsSource is System.Collections.IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is ListingFilterChoice choice && choice.Id == Guid.Empty)
                {
                    everyone = choice;
                    break;
                }
            }
        }

        // Text has to change first. Selecting the item alone leaves the previous
        // search text, and the box snaps back to that customer name.
        var label = everyone?.Label ?? "Todos";
        box.Text = label;
        if (everyone is not null && ReferenceEquals(box.SelectedItem, everyone) == false)
        {
            box.SelectedItem = everyone;
        }

        var editor = box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        if (editor is not null && editor.Text != label)
        {
            editor.Text = label;
        }
    }

    public static bool MatchCustomer(string? search, object? item)
    {
        if (item is not ListingFilterChoice choice)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(search) || string.Equals(search.Trim(), "Todos", StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (choice.Id == Guid.Empty)
        {
            return false;
        }

        var term = TextSearch.Fold(search.Trim());
        return TextSearch.Fold(choice.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
            || TextSearch.Fold(choice.SearchText).Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    public static async Task<IReadOnlyList<ListingFilterChoice>> CustomersAsync()
    {
        var service = AppComposition.Services?.GetService<IPosCustomerService>();
        if (service is null)
        {
            return [new ListingFilterChoice()];
        }

        var customers = await service.SearchAsync(string.Empty);
        var choices = new List<ListingFilterChoice> { new() };
        choices.AddRange(customers
            .Where(customer => customer.Id != Guid.Empty)
            .OrderBy(customer => customer.Name)
            .Select(customer => new ListingFilterChoice
            {
                Id = customer.Id,
                Code = customer.Code,
                Label = string.IsNullOrWhiteSpace(customer.Name) ? customer.FiscalNumber : customer.Name,
                Detail = JoinDetail(customer.Code, customer.FiscalNumber),
                SearchText = JoinSearch(customer.Name, customer.Code, customer.FiscalNumber, customer.CardNumber, customer.Phone, customer.Email)
            }));
        return choices;
    }

    public static void EnableLookupSearch(AutoCompleteBox box)
    {
        var count = Count(box.ItemsSource);
        box.FilterMode = AutoCompleteFilterMode.Custom;
        // When the text is still the current selection, show every option — otherwise the
        // filter keeps only that one row (e.g. "Documento de Conferência") and hides the rest.
        box.ItemFilter = (search, item) =>
        {
            if (box.SelectedItem is LookupOption selected
                && string.Equals((search ?? string.Empty).Trim(), selected.Label, StringComparison.CurrentCultureIgnoreCase))
            {
                return item is LookupOption;
            }

            return MatchLookup(search, item);
        };
        box.MinimumPrefixLength = count > 40 ? 1 : 0;
        box.IsTextCompletionEnabled = false;
        box.PlaceholderText = "Selecionar...";
        box.MaxDropDownHeight = 360;
        box.ItemTemplate = new FuncDataTemplate<LookupOption>((option, _) =>
        {
            var panel = new StackPanel { Spacing = 0 };
            panel.Children.Add(new TextBlock
            {
                Classes = { "bo_doc_filter_search_name" },
                Text = option?.Label ?? string.Empty
            });
            var extra = Extra(option);
            if (extra is not null)
            {
                panel.Children.Add(new TextBlock
                {
                    Classes = { "bo_doc_filter_search_meta" },
                    Text = extra
                });
            }

            return panel;
        });
        box.GotFocus += (_, _) =>
        {
            var text = box.Text ?? string.Empty;
            var selected = (box.SelectedItem as LookupOption)?.Label;
            if (string.IsNullOrEmpty(text) || text == "Todos" || text == "(nenhum)" || text == selected)
            {
                box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault()?.SelectAll();
            }

            // Open the list on focus so touch/POS users can pick without typing first.
            if (box.MinimumPrefixLength == 0)
            {
                OpenDropDownAfterFocus(box);
            }
        };
        AttachDropChevron(box);
    }

    public static void EnableDocumentSearch(AutoCompleteBox box)
    {
        box.FilterMode = AutoCompleteFilterMode.Custom;
        box.ItemFilter = MatchDocument;
        box.MinimumPrefixLength = 0;
        box.IsTextCompletionEnabled = false;
        box.PlaceholderText = "Nº, cliente ou NIF...";
        box.MaxDropDownHeight = 360;
        box.ItemTemplate = new FuncDataTemplate<PosLookupItem>((item, _) =>
        {
            var panel = new StackPanel { Spacing = 0 };
            // Empty Guid is the "none" row; Label may be blank so the field shows the watermark.
            var label = item is null
                ? string.Empty
                : item.Id == Guid.Empty && string.IsNullOrWhiteSpace(item.Label)
                    ? "—"
                    : item.Label;
            panel.Children.Add(new TextBlock
            {
                Classes = { "bo_doc_filter_search_name" },
                Text = label
            });
            if (string.IsNullOrWhiteSpace(item?.Detail) == false)
            {
                panel.Children.Add(new TextBlock
                {
                    Classes = { "bo_doc_filter_search_meta" },
                    Text = item.Detail
                });
            }

            return panel;
        });
        box.GotFocus += (_, _) =>
        {
            var text = box.Text ?? string.Empty;
            var selected = (box.SelectedItem as PosLookupItem)?.Label;
            if (string.IsNullOrEmpty(text) || text == "—" || text == selected)
            {
                box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault()?.SelectAll();
            }

            OpenDropDownAfterFocus(box);
        };
        AttachDropChevron(box);
    }

    /// <summary>
    /// Same PathIcon geometry as Fluent ComboBox DropDownGlyph — AutoCompleteBox has no built-in chevron.
    /// </summary>
    public static void AttachDropChevron(AutoCompleteBox box)
    {
        if (box.InnerRightContent is Control existing
            && existing.Classes.Contains("bo_lookup_chevron"))
        {
            return;
        }

        var icon = new PathIcon
        {
            Width = 12,
            Height = 12,
            Data = StreamGeometry.Parse("M1939 486L2029 576L1024 1581L19 576L109 486L1024 1401L1939 486Z"),
            Foreground = new SolidColorBrush(Color.Parse("#666666")),
            // Let the Button receive the click; PathIcon hit-testing can swallow pointer events.
            IsHitTestVisible = false
        };
        var button = new Button
        {
            Classes = { "bo_lookup_chevron" },
            Content = icon,
            Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        // Tunnel PointerPressed (not Click): Avalonia AutoCompleteBox closes the popup when
        // focus/pointer settles after a Button.Click. Defer open past that close.
        button.AddHandler(InputElement.PointerPressedEvent, (_, e) =>
        {
            if (e.GetCurrentPoint(button).Properties.IsLeftButtonPressed == false)
            {
                return;
            }

            e.Handled = true;
            if (box.IsDropDownOpen)
            {
                box.IsDropDownOpen = false;
                return;
            }

            FocusAutoComplete(box);
            OpenDropDownAfterFocus(box);
        }, RoutingStrategies.Tunnel);
        box.InnerRightContent = button;
    }

    private static void FocusAutoComplete(AutoCompleteBox box)
    {
        if (box.IsKeyboardFocusWithin)
        {
            return;
        }

        var editor = box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        if (editor is not null)
        {
            editor.Focus();
            return;
        }

        box.Focus();
    }

    /// <summary>
    /// Open after the current pointer/focus cycle so AutoCompleteBox does not immediately
    /// close the popup (first-click looked dead; second click worked).
    /// </summary>
    private static void OpenDropDownAfterFocus(AutoCompleteBox box)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (box.IsDropDownOpen)
            {
                return;
            }

            box.IsDropDownOpen = true;
        }, DispatcherPriority.Background);
    }

    public static Guid? SelectedDocumentId(AutoCompleteBox box)
    {
        if (box.SelectedItem is not PosLookupItem item || item.Id == Guid.Empty)
        {
            return null;
        }

        var text = (box.Text ?? string.Empty).Trim();
        if (text.Length == 0 || text == "—")
        {
            return null;
        }

        return text.Equals(item.Label, StringComparison.CurrentCultureIgnoreCase) ? item.Id : null;
    }

    public static void SelectDocument(AutoCompleteBox box, PosLookupItem? item)
    {
        PosLookupItem? target = item;
        if (target is null && box.ItemsSource is System.Collections.IEnumerable items)
        {
            foreach (var entry in items)
            {
                if (entry is PosLookupItem choice && choice.Id == Guid.Empty)
                {
                    target = choice;
                    break;
                }
            }
        }

        var label = target?.Label ?? string.Empty;
        box.Text = label;
        if (target is null)
        {
            box.SelectedItem = null;
        }
        else if (ReferenceEquals(box.SelectedItem, target) == false)
        {
            box.SelectedItem = target;
        }

        var editor = box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        if (editor is not null && editor.Text != label)
        {
            editor.Text = label;
        }
    }

    private static bool MatchDocument(string? search, object? item)
    {
        if (item is not PosLookupItem choice)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(search) || search.Trim() == "—")
        {
            return true;
        }

        if (choice.Id == Guid.Empty)
        {
            return false;
        }

        var term = TextSearch.Fold(search.Trim());
        return TextSearch.Fold(choice.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
            || TextSearch.Fold(choice.Detail).Contains(term, StringComparison.OrdinalIgnoreCase)
            || TextSearch.Fold(choice.Code).Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    public static LookupOption? SelectedLookup(AutoCompleteBox? box)
    {
        if (box?.SelectedItem is not LookupOption option || option.Id == Guid.Empty)
        {
            return null;
        }

        var text = (box.Text ?? string.Empty).Trim();
        if (text.Length == 0
            || text.Equals("Todos", StringComparison.CurrentCultureIgnoreCase)
            || text.Equals("(nenhum)", StringComparison.CurrentCultureIgnoreCase))
        {
            return null;
        }

        return text.Equals(option.Label, StringComparison.CurrentCultureIgnoreCase) ? option : null;
    }

    public static void SelectLookup(AutoCompleteBox box, LookupOption? option)
    {
        LookupOption? target = option;
        if (target is null && box.ItemsSource is System.Collections.IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is LookupOption choice && choice.Id == Guid.Empty)
                {
                    target = choice;
                    break;
                }
            }
        }

        var label = target?.Label ?? string.Empty;
        box.Text = label;
        if (target is null)
        {
            box.SelectedItem = null;
        }
        else if (ReferenceEquals(box.SelectedItem, target) == false)
        {
            box.SelectedItem = target;
        }

        var editor = box.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        if (editor is not null && editor.Text != label)
        {
            editor.Text = label;
        }
    }

    public static async Task<IReadOnlyList<ListingFilterChoice>> DocumentTypesAsync()
    {
        var service = AppComposition.Services?.GetService<IPosDocumentService>();
        if (service is null)
        {
            return [new ListingFilterChoice()];
        }

        var types = await service.ListDocumentTypesAsync();
        var choices = new List<ListingFilterChoice> { new() };
        choices.AddRange(types
            .Where(type => string.IsNullOrWhiteSpace(type.Acronym) == false)
            .Select(type => new ListingFilterChoice
            {
                Id = type.Id,
                Code = type.Acronym,
                Label = string.IsNullOrWhiteSpace(type.Designation) ? type.Acronym : type.Acronym + " - " + type.Designation
            }));
        return choices;
    }

    private static string? JoinDetail(string? code, string? fiscal)
    {
        var parts = new[] { code, fiscal }.Where(part => string.IsNullOrWhiteSpace(part) == false).ToArray();
        return parts.Length == 0 ? null : string.Join(" · ", parts);
    }

    private static string JoinSearch(params string?[] parts)
        => string.Join(' ', parts.Where(part => string.IsNullOrWhiteSpace(part) == false));

    private static bool MatchLookup(string? search, object? item)
    {
        if (item is not LookupOption option)
        {
            return false;
        }

        // Empty / placeholder text must list every choice — otherwise selecting "(nenhum)"
        // filters the dropdown down to only that row and nothing else can be picked.
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }

        var term = TextSearch.Fold(search.Trim());
        if (term.Equals("todos", StringComparison.OrdinalIgnoreCase)
            || term.Equals("(nenhum)", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (option.Id == Guid.Empty)
        {
            return TextSearch.Fold(option.Label).Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        return TextSearch.Fold(option.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
            || TextSearch.Fold(option.Search ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
            || TextSearch.Fold(option.Meta ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static string? Extra(LookupOption? option)
    {
        if (option is null || string.IsNullOrWhiteSpace(option.Search))
        {
            return null;
        }

        var parts = option.Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var barcode = parts.LastOrDefault();
        if (string.IsNullOrWhiteSpace(barcode) || option.Label.Contains(barcode, StringComparison.CurrentCultureIgnoreCase))
        {
            return null;
        }

        return barcode;
    }

    private static int Count(System.Collections.IEnumerable? items)
    {
        if (items is System.Collections.ICollection collection)
        {
            return collection.Count;
        }

        var count = 0;
        if (items is null)
        {
            return 0;
        }

        foreach (var _ in items)
        {
            count++;
        }

        return count;
    }
}
