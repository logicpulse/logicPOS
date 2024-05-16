namespace LogicPOS.Settings
{
    public static class ColorSettings
    {
        public static string ValidTextBoxColor => GeneralSettings.Settings["colorEntryValidationValidFont"];
        public static string InvalidTextBoxColor => GeneralSettings.Settings["colorEntryValidationInvalidFont"];
        public static string ValidTextBoxBackgroundColor => GeneralSettings.Settings["colorEntryValidationValidBackground"];
        public static string InvalidTextBoxBackgroundColor => GeneralSettings.Settings["colorEntryValidationInvalidBackground"];
    }
}
