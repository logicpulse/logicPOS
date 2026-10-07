using Avalonia.Controls;
using Avalonia.Input.TextInput;
using Avalonia.VisualTree;
using LogicPOS.App.Views;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace LogicPOS.App;

internal static class TouchFields
{
    public static void Attach(Control root)
    {
        var boxes = new List<TextBox>();
        var searches = new List<AutoCompleteBox>();
        Collect(root, boxes, searches);
        foreach (var box in boxes)
        {
            Wrap(box, () =>
            {
                box.Focus();
                return IsNumeric(box);
            }, () =>
            {
                if (IsNumeric(box))
                {
                    TextInputOptions.SetContentType(box, TextInputContentType.Number);
                }
            });
        }

        foreach (var search in searches)
        {
            Wrap(search, () =>
            {
                search.ApplyTemplate();
                var editor = search.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
                if (editor is null)
                {
                    search.Focus();
                }
                else
                {
                    editor.Focus();
                }

                return false;
            }, null);
        }
    }

    private static void Wrap(Control box, Func<bool> showKeyboard, Action? prepare)
    {
        if (box.IsVisible == false
            || box.Classes.Contains("bo_touch_ready")
            || box.Classes.Contains("login_pin_entry")
            || box is TextBox { IsReadOnly: true })
        {
            return;
        }

        if (box.Parent is DockPanel existing && existing.Classes.Contains("bo_touch_row"))
        {
            return;
        }

        prepare?.Invoke();
        var button = new Button
        {
            Classes = { "bo_touch_key" },
            Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://logicpos/"))
            {
                Classes = { "bo_touch_key_icon" },
                Path = "avares://logicpos/Assets/Images/Listing/botao_teclado.svg"
            }
        };
        button.Click += (_, _) => TouchKeyboard.Show(showKeyboard());
        if (box is TextBox text)
        {
            text.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
            {
                if (args.Pointer.Type == PointerType.Touch)
                {
                    text.Focus();
                    TouchKeyboard.Show(IsNumeric(text));
                }
            }, RoutingStrategies.Tunnel);
        }

        var row = new DockPanel { Classes = { "bo_touch_row" } };
        DockPanel.SetDock(button, Dock.Right);
        row.Children.Add(button);
        box.Classes.Add("bo_touch_ready");

        if (box.Parent is Panel panel)
        {
            var index = panel.Children.IndexOf(box);
            if (index < 0)
            {
                return;
            }

            panel.Children.RemoveAt(index);
            row.Children.Add(box);
            panel.Children.Insert(index, row);
            return;
        }

        if (box.Parent is Decorator decorator && ReferenceEquals(decorator.Child, box))
        {
            decorator.Child = null;
            row.Children.Add(box);
            decorator.Child = row;
            return;
        }

        if (box.Parent is ContentControl content && ReferenceEquals(content.Content, box))
        {
            content.Content = null;
            row.Children.Add(box);
            content.Content = row;
        }
    }

    public static bool IsNumeric(TextBox box)
    {
        return box.Classes.Contains("bo_numeric") || NumericNames.Contains(box.Name ?? string.Empty);
    }

    public static bool IsNumericKey(string? key)
    {
        return string.IsNullOrWhiteSpace(key) == false && NumericNames.Contains(key);
    }

    private static readonly HashSet<string> NumericNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "FiscalNumber", "Discount", "ZipCode", "Phone", "MobilePhone", "Fax",
        "NextNumber", "NumberRangeBegin", "NumberRangeEnd", "Value", "Quantity", "Price",
        "Commission", "ExchangeRate", "Day", "Month", "Year", "CharactersPerLine",
        "BaudRate", "DataBits", "EnumValue", "PrintCopies", "CustomerFiscal", "EditorFiscal",
        "EditorDiscount", "EditorZip", "EditorPhone", "EditPrice", "EditQuantity",
        "EditDiscount", "ToPostal", "FromPostal", "SessionAmount", "VoltaQuantity"
    };

    private static void Collect(object? node, List<TextBox> boxes, List<AutoCompleteBox> searches)
    {
        switch (node)
        {
            case AutoCompleteBox search:
                searches.Add(search);
                break;
            case TextBox box:
                boxes.Add(box);
                break;
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    Collect(child, boxes, searches);
                }

                break;
            case Decorator decorator:
                Collect(decorator.Child, boxes, searches);
                break;
            case ContentControl content:
                Collect(content.Content, boxes, searches);
                break;
        }
    }
}
