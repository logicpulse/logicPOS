using LogicPOS.UI.Settings;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalIconsSettings
    {
        public string WindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_default.png";
        public readonly string CloseWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_window_close.png";
        public readonly string MinimizeWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_window_minimize.png";
        public string ActionDefault = AppSettings.Paths.Images + @"Icons\icon_pos_default.png";
        public string ActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        public string ActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
        public string DemoData = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_demo.png";
        public string ActionMore = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_nav_new.png";

        public static ModalIconsSettings Default { get; } = new ModalIconsSettings();
    }
}
