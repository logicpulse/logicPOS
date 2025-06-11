namespace LogicPOS.Plugin.Data
{
    public class SoftwareVendorPluginData
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //App
        
        public static string SecretKey { get; set; } = ")p[r#HW'gOg|KNI1L3k]H&~D!DKy`Y[fx2/t&s7{:!S<xDl,l#5)[YHcVf'3UUc";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Vendor

        public static string AppSoftwareName = "LogicPos";
        public static string AppCompanyName = "LogicPulse Technologies";
        public static string AppCompanyPhone = "+351 233 042 347 / +351 910 287 029 / +351 800 180 500";
        public static string AppCompanyEmail = "comercial@logicpulse.com";
        public static string AppCompanyWeb = "http://www.logicpulse.com";
        public static string AppSoftwareVersionFormat = string.Format("Powered by {0}© Vers. {{0}}", AppCompanyName);
        public static string AppSoftwareATWSTestModeCertificatePassword = "TESTEwebservice";
        public static string AppSoftwareATWSProdModeCertificatePassword = "XXXXXXX";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Files/File Formats

        //FileFormat Used in SAF-T(PT)/Backups Etc
        public static string FileFormatDateTime = "yyyyMMdd_HHmmss";
        //filename_countrycode_version_date.extension
        public static string FileFormatSaftPT = "saf-t_{0}_{1}_{2}.xml";
        public static string FileFormatSaftAO = "saf-t_{0}_{1}_{2}.xml";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataBase Backup System

        public static string BackupPassword = "w)9SC1=a^M$]4#yL#<?z8!-w[r2rW)ub";

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
  <Modulus>x30nmR4KQl7kttdSmqU90xCIVwiosiMSQrrk1v8ghFPIyd4+BEEgys6dZYjl4jmJU4M9Zm8HyHa8/OmL2tcGbON1osGR1fP3F4H5XrGZmVeYGchCObx8EejjEbdL6bbWDJUwIUUe6XRRv9LDwv0jBDmF8aG25w/Wjj7LIUuyM40=</Modulus>
  <Exponent>AQAB</Exponent>
  <P>+jQmbPbvonuB+GBuaG7AtcHC5VHfgj4lWntCvFXXz5243yqR1xWk4KhOAr0ArXoaKaetOKxsOMoF3ZaKAgcAuQ==</P>
  <Q>zBw8pZxDvc8cNVwwMWAX/iJ8Te0wbDqkZECNh6h6nxCyGs1d8NW36UqMMpCN9dCGlnG1/B+fsYUNaZF+x0BXdQ==</Q>
  <DP>R4xkNKLE0i0JdLJ2wCxztUYsapFoHgGRgvdelSbjKP7MYBz3SY9p78iRTl0n9XPGSYUmlt9Pr0muNbiXzI6eGQ==</DP>
  <DQ>bhq5WHkQTd7gj6RYVvpIVw4RUhQmL+v+bBoqfsiSxSXDfhPUnisb15wgPtKd0cGYKKXUjtocUi29C3qyEhgSVQ==</DQ>
  <InverseQ>FoIjxq0R/pOXWH8E5dSBJeCxqy5OB7hETqKDyxj3LXp6RIpMBfHKCJb6rUjZi2BZGA/IBSCLu8OlwVBJfox8Kg==</InverseQ>
  <D>Vr4MBt9yFJQQnZSZXZc2f91ze2zPdc4cNZnwwa/kIsYPy/9wNgdfy+/1rt3NIAZmuUKa4zyqRLeky9B9uKXBK7hbpwpfvK+70nIXjGuMBSHppcUfwADNmoaB1Ure1UZRvUqyL4rCbIr0gqFqca8TkYWN94xSvwHdnFGrwI+NCWE=</D>
</RSAKeyValue>";

        //SAFT-T XML Export Header
        public static string SaftProductID = string.Format("LogicPos/{0}", AppCompanyName);
        public static string SaftProductCompanyTaxID = "508278155";
        public static string SaftSoftwareCertificateNumber = "2543";
        public static string SaftVersionPrefix = "PT";
        public static string SaftVersion = "1.04_01";
        //Versão da Chave Privada utilizada na criação da Assinatura 
        public static int HashControl = 1;
        //C — Contabilidade;
        //E — Faturação emitida por terceiros;
        //F — Faturação;
        //I — Contabilidade integrada com a faturação;
        //P — Faturação parcial;
        //R — Recibos (a);
        //S — Autofaturação;
        //T — Documentos de transporte (a).	
        public static string TaxAccountingBasis = "F";
        //Currency Code
        public static string SaftCurrencyCode = "EUR";

        //SAFT(PT) : Country Rules
        //Retalhistas e vendedores ambulantes é permitida a emissão de faturas simplificadas a não sujeitos passivos, 
        //Até limite de 1000,00€ e a todas as outras atividades é apenas permitida a emissão de faturas até aos 100,00€
        public static int FinanceRuleSimplifiedInvoiceMaxTotal = 1000;
        //Services
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServices = 100;
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        public static int FinanceRuleRequiredCustomerDetailsAboveValue = 1000;




        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SAF-T AO

        //SAF-T(AO) : Formats 
        public static int DocumentsPadLengthAO = 6;
        //SAF-T(AO) : DateTime Formats 
        public static string DateTimeFormatDocumentDateAO = "yyyy-MM-dd";
        public static string DateTimeFormatCombinedDateTimeAO = "yyyy-MM-ddTHH:mm:ss";
        public static string FinanceFinalConsumerFiscalNumberAO = "999999999";
        public static string FinanceFinalConsumerFiscalNumberDisplayAO = "---------";
        //SAF-T(AO) : Decimal Format
        public static string DecimalFormatSAFTAO = "0.00000000";
        public static string DecimalFormatGrossTotalSAFTAO = "0.00";
      
        //RSA Private Key :Sign Finance Documents used in SHA1SignMessage()
        //01/11/2019
        public static string RsaPrivateKeyAO = @"XXXX";
        //SAFT-T XML Export Header
        public static string SaftProductIDAO = string.Format("LogicPos/{0}", AppCompanyName);
        public static string SaftProductCompanyTaxIDAO = "5171164380";
        public static string SaftSoftwareCertificateNumberAO = "221";
        public static string SaftVersionPrefixAO = "AO";
        public static string SaftVersionAO = "1.01_01";
        //Versão da Chave Privada utilizada na criação da Assinatura 
        public static int HashControlAO = 1;
        //C — Contabilidade;
        //E — Faturação emitida por terceiros;
        //F — Faturação;
        //I — Contabilidade integrada com a faturação;
        //P — Faturação parcial;
        //R — Recibos (a);
        //S — Autofaturação;
        //T — Documentos de transporte (a).	
        public static string TaxAccountingBasisAO = "F";
        //Currency Code
        public static string SaftCurrencyCodeAO = "AOA";

        //SAFT(AO) : Country Rules
        //Retalhistas e vendedores ambulantes é permitida a emissão de faturas simplificadas a não sujeitos passivos, 
        //Até limite de 1000,00€ e a todas as outras atividades é apenas permitida a emissão de faturas até aos 100,00€
        public static int FinanceRuleSimplifiedInvoiceMaxTotalAO = 1000;
        //Services
        public static int FinanceRuleSimplifiedInvoiceMaxTotalServicesAO = 100;
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        public static int FinanceRuleRequiredCustomerDetailsAboveValueAO = 1000;



        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Series

        //Use Alphanumeric Random|Or Numeric Sequential 
        public static bool DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = false;
        public static string DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = "0";//000
    }
}
