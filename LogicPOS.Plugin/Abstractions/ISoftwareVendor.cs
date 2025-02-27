using System.Security.Cryptography.X509Certificates;

namespace LogicPOS.Plugin.Abstractions
{
    public interface ISoftwareVendor : IPlugin
    {
        string GetAppSoftwareName();
        string GetAppCompanyName();
        string GetAppCompanyPhone();
        string GetAppCompanyEmail();
        string GetAppCompanyWeb();
        string GetAppSoftwareVersionFormat();
        string GetAppSoftwareATWSProdModeCertificatePassword();
       
        string GetFileFormatDateTime();
        string GetFileFormatSaftPT();


        int GetDocumentsPadLength();

        string GetDateTimeFormatDocumentDate();
        string GetDateTimeFormatCombinedDateTime();
        string GetFinanceFinalConsumerFiscalNumber();
        string GetFinanceFinalConsumerFiscalNumberDisplay();

        string GetDecimalFormatSAFTPT();
        string GetDecimalFormatGrossTotalSAFTPT();

        int GetDecimalRoundTo();

        string GetSaftProductID();
        string GetSaftProductCompanyTaxID();
        string GetSaftSoftwareCertificateNumber();
        string GetSaftVersionPrefix();
        string GetSaftVersion();         

        int GetHashControl();
      
        string GetTaxAccountingBasis();
        string GetSaftCurrencyCode();

       
        int GetFinanceRuleSimplifiedInvoiceMaxTotal();
     
        int GetFinanceRuleSimplifiedInvoiceMaxTotalServices();
   
        int GetFinanceRuleRequiredCustomerDetailsAboveValue();

      
        int GetDocumentsPadLengthAO();

        string GetDateTimeFormatDocumentDateAO();
        string GetDateTimeFormatCombinedDateTimeAO();
        string GetFinanceFinalConsumerFiscalNumberAO();
        string GetFinanceFinalConsumerFiscalNumberDisplayAO();

        string GetDecimalFormatSAFTAO();
        string GetDecimalFormatGrossTotalSAFTAO();
       
        string GetSaftProductIDAO();
        string GetSaftProductCompanyTaxIDAO();
        string GetSaftSoftwareCertificateNumberAO();
        string GetSaftVersionPrefixAO();
        string GetSaftVersionAO();
 
        int GetHashControlAO();
       
        string GetTaxAccountingBasisAO();
        string GetSaftCurrencyCodeAO();


        int GetFinanceRuleSimplifiedInvoiceMaxTotalAO();

        int GetFinanceRuleSimplifiedInvoiceMaxTotalServicesAO();

        int GetFinanceRuleRequiredCustomerDetailsAboveValueAO();


        bool GetDocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix();
        string GetDocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat();

      
        bool IsValidSecretKey(string secretKey);

        string SignDataToSHA1Base64(string secretKey, string encryptData, bool debug = false);

   
        string Encrypt(string toEncrypt);
        string Decrypt(string cipherString);


        X509Certificate2 ImportCertificate(bool testModeEnabled, string pathCertificate);
    }
}
