using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Dialogs
{
    public class BaseDialogColorSettings
    {
        public Color TitleBackground = AppSettings.Instance.ColorBaseDialogTitleBackground;
        public Color WindowBackground = AppSettings.Instance.ColorBaseDialogWindowBackground;
        public Color WindowBackgroundBorder = AppSettings.Instance.ColorBaseDialogWindowBackgroundBorder;
        public Color DefaultButtonFont = AppSettings.Instance.ColorBaseDialogDefaultButtonFont;
        public Color DefaultButtonBackground = AppSettings.Instance.ColorBaseDialogDefaultButtonBackground;
        public Color ActionAreaButtonFont = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont;
        public Color ActionAreaButtonBackground = AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground;
    }
}
