using LogicPOS.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public static class EntitySelectionModalColors
    {
        public static Color TitleBackground = AppSettings.Instance.colorBaseDialogTitleBackground;
        public static Color WindowBackground = AppSettings.Instance.colorBaseDialogWindowBackground;
        public static Color WindowBackgroundBorder = AppSettings.Instance.colorBaseDialogWindowBackgroundBorder;
        public static Color DefaultButtonFont = AppSettings.Instance.colorBaseDialogDefaultButtonFont;
        public static Color DefaultButtonBackground = AppSettings.Instance.colorBaseDialogDefaultButtonBackground;
        public static Color ActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
        public static Color ActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
    }
}
