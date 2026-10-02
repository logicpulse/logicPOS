using Avalonia.Controls;
using Avalonia.Input.TextInput;
using LogicPOS.App.Views;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace LogicPOS.App;

internal static class TouchFields
{
    public static void Attach(Control root)
    {
        var boxes = new List<TextBox>();
        Collect(root, boxes);
        foreach (var box in boxes)
        {
            if (box.IsReadOnly
                || box.IsVisible == false
                || box.Classes.Contains("bo_touch_ready")
                || box.Classes.Contains("login_pin_entry"))
            {
                continue;
            }

            if (box.Parent is DockPanel existing && existing.Classes.Contains("bo_touch_row"))
            {
                continue;
            }

            var numeric = IsNumeric(box);
            if (numeric)
            {
                TextInputOptions.SetContentType(box, TextInputContentType.Number);
            }

            var button = new Button
            {
                Classes = { "bo_touch_key" },
                Content = new Avalonia.Svg.Skia.Svg(new Uri("avares://logicpos/"))
                {
                    Classes = { "bo_touch_key_icon" },
                    Path = "avares://logicpos/Assets/Images/Listing/botao_teclado.svg"
                }
            };
            button.Click += (_, _) =>
            {
                box.Focus();
                TouchKeyboard.Show(IsNumeric(box));
            };
            box.AddHandler(InputElement.PointerPressedEvent, (_, args) =>
            {
                if (args.Pointer.Type == PointerType.Touch)
                {
                    box.Focus();
                    TouchKeyboard.Show(IsNumeric(box));
                }
            }, RoutingStrategies.Tunnel);

            var row = new DockPanel { Classes = { "bo_touch_row" } };
            DockPanel.SetDock(button, Dock.Right);
            row.Children.Add(button);
            box.Classes.Add("bo_touch_ready");

            if (box.Parent is Panel panel)
            {
                var index = panel.Children.IndexOf(box);
                if (index < 0)
                {
                    continue;
                }

                panel.Children.RemoveAt(index);
                row.Children.Add(box);
                panel.Children.Insert(index, row);
                continue;
            }

            if (box.Parent is Decorator decorator && ReferenceEquals(decorator.Child, box))
            {
                decorator.Child = null;
                row.Children.Add(box);
                decorator.Child = row;
                continue;
            }

            if (box.Parent is ContentControl content && ReferenceEquals(content.Content, box))
            {
                content.Content = null;
                row.Children.Add(box);
                content.Content = row;
            }
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

    private static void Collect(object? node, List<TextBox> boxes)
    {
        switch (node)
        {
            case TextBox box:
                boxes.Add(box);
                break;
            case Panel panel:
                foreach (var child in panel.Children)
                {
                    Collect(child, boxes);
                }

                break;
            case Decorator decorator:
                Collect(decorator.Child, boxes);
                break;
            case ContentControl content:
                Collect(content.Content, boxes);
                break;
        }
    }
}
