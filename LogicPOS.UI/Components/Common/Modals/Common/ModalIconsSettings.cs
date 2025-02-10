using LogicPOS.Settings;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalIconsSettings
    {
        public string WindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_default.png";
        public readonly string CloseWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_window_close.png";
        public readonly string MinimizeWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_window_minimize.png";
        public string ActionDefault = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_default.png";
        public string ActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        public string ActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
        public string DemoData = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_demo.png";
        public string ActionMore = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_nav_new.png";

        public static ModalIconsSettings Default { get; } = new ModalIconsSettings();
    }
}
