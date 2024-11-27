using LogicPOS.Settings;
using System.Drawing;

namespace LogicPOS.UI.Dialogs
{
    public class BaseDialogColorSettings
    {
        public Color TitleBackground = AppSettings.Instance.colorBaseDialogTitleBackground;
        public Color WindowBackground = AppSettings.Instance.colorBaseDialogWindowBackground;
        public Color WindowBackgroundBorder = AppSettings.Instance.colorBaseDialogWindowBackgroundBorder;
        public Color DefaultButtonFont = AppSettings.Instance.colorBaseDialogDefaultButtonFont;
        public Color DefaultButtonBackground = AppSettings.Instance.colorBaseDialogDefaultButtonBackground;
        public Color ActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;
        public Color ActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
    }
}
