namespace acme.softwarevendor.plugin.App
{
    public static class AcmeSettings
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //App
        
        public static string SecretKey = ")p[r#HW'gOg|KNI1L3k]H&~D!DKy`Y[fx2/t&s7{:!S<xDl,l#5)[YHcVf'3UUc";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Vendor

        public static string AppSoftwareName = "Acme";
        public static string AppCompanyName = "Acme";
        public static string AppCompanyPhone = "+351 000 000 000";
        public static string AppCompanyEmail = "sales@acme.com";
        public static string AppCompanyWeb = "http://www.acme.com";
        public static string AppSoftwareVersion = string.Format("Powered by {0}© Vers. {{{0}}}", AppCompanyName);
        public static string AppSoftwareVersionFormat = string.Format("Powered by {0}© Vers. {{0}}", AppCompanyName);
        public static string AppSoftwareATWSTestModeCertificatePassword = "TESTEwebservice";
        public static string AppSoftwareATWSProdModeCertificatePassword = "YOUR_PASSWORD_HERE";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Files/File Formats

        //FileFormat Used in SAF-T(PT)/Backups Etc
        public static string FileFormatDateTime = "yyyyMMdd_HHmmss";
        //filename_countrycode_version_date.extension
        public static string FileFormatSaftPT = "saf-t_{0}_{1}_{2}.xml";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataBase Backup System

        public static string BackupPassword = "PASSWORD";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SAF-T PT

        //SAF-T(PT) : Formats 
        public static int DocumentsPadLength = 6;
        //SAF-T(PT) : DateTime Formats 
        public static string DateTimeFormatDocumentDate = "yyyy-MM-dd";
        public static string DateTimeFormatCombinedDateTime = "yyyy-MM-ddTHH:mm:ss";
        public static string FinanceFinalConsumerFiscalNumber = "999999990";
        public static string FinanceFinalConsumerFiscalNumberDisplay = "---------";
        //SAF-T(PT) : Decimal Format
        public static string DecimalFormatSAFTPT = "0.00000000";
        public static string DecimalFormatGrossTotalSAFTPT = "0.00";
        //Used to Compare, Round first, Compare After
        public static int DecimalRoundTo = 2;
        //RSA Private Key :Sign Finance Documents used in SHA1SignMessage()
        public static string RsaPrivateKey = @"<RSAKeyValue>
  <Modulus></Modulus>
  <Exponent></Exponent>
  <P></P>
  <Q></Q>
  <DP></DP>
  <DQ></DQ>
  <InverseQ></InverseQ>
  <D></D>
</RSAKeyValue>";
        //SAFT-T XML Export Header
        public static string SaftProductID = string.Format("AcmePos/{0}", AppCompanyName);
        public static string SaftProductCompanyTaxID = "000000000";
        public static string SaftSoftwareCertificateNumber = "0000";
        public static string SaftVersionPrefix = "PT";
        public static string SaftVersion = "1.03_01";
        public static int HashControl = 1;
        public static string TaxAccountingBasis = "F";
        public static string SaftCurrencyCode = "EUR";
        public static int FinanceRuleSimplifiedInvoiceMaxTotal = 1000;
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServices = 100;
        public static int FinanceRuleRequiredCustomerDetailsAboveValue = 1000;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Series

        //Use Alphanumeric Random|Or Numeric Sequential 
        public static bool DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = false;
        public static string DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = "000";
    }
}
