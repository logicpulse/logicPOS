using System.Globalization;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
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
        };
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

        var term = Fold(search.Trim());
        return Fold(choice.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
            || Fold(choice.SearchText).Contains(term, StringComparison.OrdinalIgnoreCase);
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
        box.ItemFilter = MatchLookup;
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
                box.IsDropDownOpen = true;
            }
        };
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

        var term = Fold(search.Trim());
        if (term.Equals("todos", StringComparison.OrdinalIgnoreCase)
            || term.Equals("(nenhum)", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (option.Id == Guid.Empty)
        {
            return Fold(option.Label).Contains(term, StringComparison.OrdinalIgnoreCase);
        }

        return Fold(option.Label).Contains(term, StringComparison.OrdinalIgnoreCase)
            || Fold(option.Search ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
            || Fold(option.Meta ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase);
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

    private static string Fold(string text)
    {
        var form = text.Normalize(NormalizationForm.FormD);
        var chars = form.Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC);
    }
}
