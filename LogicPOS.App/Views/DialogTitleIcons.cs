using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace LogicPOS.App.Views;

/// <summary>
/// Title-bar icons for BO/FO popups so create/edit dialogs match the action chrome.
/// Create uses the same green “novo” glyph as listing action buttons (not the circle-plus).
/// </summary>
internal static class DialogTitleIcons
{
    public const string Create = "avares://LogicPOS.App/Assets/Images/Listing/botao_novo_w.svg";
    public const string Edit = "avares://LogicPOS.App/Assets/Images/Documents/botao_editar_w.svg";
    public const string Info = "avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_info_window.png";
    public const string Question = "avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_question_window.png";
    public const string Warning = "avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_warning_window.png";
    public const string Error = "avares://LogicPOS.App/Assets/Images/Dialogs/icon_pos_dialog_error_window.png";

    public static void Apply(Image? icon, string avaresPath)
    {
        if (icon is null || string.IsNullOrWhiteSpace(avaresPath))
        {
            return;
        }

        ApplyFlexible(icon, avaresPath);
    }

    public static void ApplyCreate(Image? icon) => ApplyFlexible(icon, Create);

    public static void ApplyEdit(Image? icon) => ApplyFlexible(icon, Edit);

    public static void ApplyInfo(Image? icon) => ApplyFlexible(icon, Info);

    private static void ApplyFlexible(Image? icon, string avaresPath)
    {
        if (icon is null || string.IsNullOrWhiteSpace(avaresPath))
        {
            return;
        }

        ClearDynamicSvg(icon);

        if (avaresPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            icon.IsVisible = false;
            if (icon.Parent is not Panel parent)
            {
                return;
            }

            var index = parent.Children.IndexOf(icon);
            var svg = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "login_alert_title_icon", "login_alert_title_icon_dynamic" },
                Path = avaresPath
            };
            parent.Children.Insert(index >= 0 ? index : 0, svg);
            return;
        }

        icon.IsVisible = true;
        try
        {
            icon.Source = new Bitmap(AssetLoader.Open(new Uri(avaresPath)));
        }
        catch
        {
            // Missing asset must not block the dialog.
        }
    }

    private static void ClearDynamicSvg(Image icon)
    {
        if (icon.Parent is not Panel parent)
        {
            return;
        }

        for (var i = parent.Children.Count - 1; i >= 0; i--)
        {
            if (parent.Children[i] is Avalonia.Svg.Skia.Svg svg &&
                svg.Classes.Contains("login_alert_title_icon_dynamic"))
            {
                parent.Children.RemoveAt(i);
            }
        }
    }
}
