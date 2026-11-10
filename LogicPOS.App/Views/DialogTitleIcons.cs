using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace LogicPOS.App.Views;

/// <summary>
/// Title-bar icons for BO/FO popups so create/edit dialogs match the action chrome.
/// </summary>
internal static class DialogTitleIcons
{
    public const string Create = "avares://LogicPOS.App/Assets/Images/Documents/icon_doc_new.png";
    public const string Edit = "avares://LogicPOS.App/Assets/Images/Documents/icon_doc_edit_w.png";
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

        try
        {
            icon.Source = new Bitmap(AssetLoader.Open(new Uri(avaresPath)));
        }
        catch
        {
            // Missing asset must not block the dialog.
        }
    }

    public static void ApplyCreate(Image? icon) => Apply(icon, Create);

    public static void ApplyEdit(Image? icon) => Apply(icon, Edit);

    public static void ApplyInfo(Image? icon) => Apply(icon, Info);
}