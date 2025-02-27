using System.Drawing;

namespace LogicPOS.Settings
{
    public static class ColorSettings
    {
        public static Color ValidTextBoxColor => AppSettings.Instance.colorEntryValidationValidFont;
        public static Color InvalidTextBoxColor => AppSettings.Instance.colorEntryValidationInvalidFont;
        public static Color ValidTextBoxBackgroundColor => AppSettings.Instance.colorEntryValidationValidBackground;
        public static Color InvalidTextBoxBackgroundColor => AppSettings.Instance.colorEntryValidationInvalidBackground;
    }
}
