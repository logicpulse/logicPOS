using System.Drawing;

namespace LogicPOS.UI.Settings
{
    public partial class AppSettings
    {
        public static class Colors
        {
            public static Color ValidTextBoxColor => Instance.ColorEntryValidationValidFont;
            public static Color InvalidTextBoxColor => Instance.ColorEntryValidationInvalidFont;
            public static Color ValidTextBoxBackgroundColor => Instance.ColorEntryValidationValidBackground;
            public static Color InvalidTextBoxBackgroundColor => Instance.ColorEntryValidationInvalidBackground;
        }
    }
}
