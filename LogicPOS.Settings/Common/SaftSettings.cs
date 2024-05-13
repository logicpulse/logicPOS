namespace LogicPOS.Settings
{
    public static class SaftSettings
    {
        private static string GetSaftProductID()
        {
            return $"{PluginSettings.AppSoftwareName}/{PluginSettings.AppCompanyName}";
        }

        private static string GetSaftProductID_AO()
        {
            return $"{PluginSettings.AppSoftwareName}";
        }

        public static int DocumentsPadLength { get; set; }
        public static string FinanceFinalConsumerFiscalNumber { get; set; }
        public static string FinanceFinalConsumerFiscalNumberDisplay { get; set; }
        public static string SaftProductID { get { return GetSaftProductID(); } }
        public static string SaftProductCompanyTaxID { get; set; }
        public static string SaftSoftwareCertificateNumber { get; set; }
        public static string SaftVersionPrefix { get; set; }
        public static string SaftVersion { get; set; }
        public static string SaftProductIDAO { get { return GetSaftProductID_AO(); } }
        public static string SaftSoftwareCertificateNumberAO { get; set; }
        public static string SaftVersionPrefixAO { get; set; }
        public static string SaftVersionAO { get; set; }
        public static string TaxAccountingBasis { get; set; }
    }
}
