namespace LogicPOS.Settings
{
    public static partial class PrintingSettings
    {
        public static class ThermalPrinter
        {
            public static bool UsingThermalPrinter { get; set; }
            public static string Encoding { get; set; } = "PC860";
            public static string CompanyLogoLocation { get; set; } = "Images/Tickets/company_loggero_thermal.bmp";
            public static int MaxCharsPerLineNormal { get; set; } = 48;
            public static int MaxCharsPerLineNormalBold { get; set; } = 44;
            public static int MaxCharsPerLineSmall { get; set; } = 64;
            public static string CutCommand { get; set; } = "0x42,0x00";
            public static int OpenDrawerValueM { get; set; } = 0;
            public static int OpenDrawerValueT1 { get; set; } = 3;
            public static int OpenDrawerValueT2 { get; set; } = 49;
        }
    }
}
