using Avalonia;
using Avalonia.Controls;

namespace LogicPOS.App;

/// <summary>
/// Eye toggle for masked PIN/password TextBoxes (same glyphs as login).
/// Call after <see cref="TouchFields.Attach"/> so the button sits over the field
/// beside the touch-keyboard row.
/// </summary>
internal static class PasswordReveal
{
    private const char Mask = '•';
    private const string InvisibleIcon = "avares://LogicPOS.App/Assets/Images/Login/botao_password_invisivel.svg";
    private const string VisibleIcon = "avares://LogicPOS.App/Assets/Images/Login/botao_password_visivel.svg";

    public static void Attach(params TextBox[] boxes)
    {
        foreach (var box in boxes)
        {
            Attach(box);
        }
    }

    public static void Attach(TextBox box)
    {
        if (box.Classes.Contains("bo_password_reveal_ready"))
        {
            return;
        }

        box.PasswordChar = Mask;
        box.Padding = new Thickness(8, 4, 36, 4);
        box.Classes.Add("bo_password_reveal_ready");

        var hidden = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "login_pin_eye" },
            Path = InvisibleIcon
        };
        var shown = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
        {
            Classes = { "login_pin_eye" },
            Path = VisibleIcon,
            IsVisible = false
        };
        var button = new Button
        {
            Classes = { "login_pin_reveal" },
            Content = new Panel { Children = { hidden, shown } }
        };
        ToolTip.SetTip(button, "Mostrar");
        button.Click += (_, _) =>
        {
            var reveal = box.PasswordChar != '\0';
            box.PasswordChar = reveal ? '\0' : Mask;
            hidden.IsVisible = !reveal;
            shown.IsVisible = reveal;
            ToolTip.SetTip(button, reveal ? "Ocultar" : "Mostrar");
        };

        var host = new Panel();
        if (box.Parent is DockPanel row && row.Classes.Contains("bo_touch_row"))
        {
            var index = row.Children.IndexOf(box);
            if (index < 0)
            {
                return;
            }

            row.Children.RemoveAt(index);
            host.Children.Add(box);
            host.Children.Add(button);
            row.Children.Insert(index, host);
            return;
        }

        if (box.Parent is Panel panel)
        {
            var index = panel.Children.IndexOf(box);
            if (index < 0)
            {
                return;
            }

            panel.Children.RemoveAt(index);
            host.Children.Add(box);
            host.Children.Add(button);
            panel.Children.Insert(index, host);
        }
    }
}
