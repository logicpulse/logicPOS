using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace logicpos.plugin.contracts
{
    public interface ISoftwareVendor : IPlugin
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Vendor

        string GetAppSoftwareName();
        string GetAppCompanyName();
        string GetAppCompanyPhone();
        string GetAppCompanyEmail();
        string GetAppCompanyWeb();
        string GetAppSoftwareVersionFormat();
        string GetAppSoftwareATWSProdModeCertificatePassword();
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Files/File Formats

        //FileFormat Used in SAF-T(PT)/Backups Etc
        string GetFileFormatDateTime();
        //filename_countrycode_version_date.extension
        string GetFileFormatSaftPT();

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SAF-T PT

        //SAF-T(PT) : Formats 
        int GetDocumentsPadLength();
        //SAF-T(PT) : DateTime Formats 
        string GetDateTimeFormatDocumentDate();
        string GetDateTimeFormatCombinedDateTime();
        string GetFinanceFinalConsumerFiscalNumber();
        string GetFinanceFinalConsumerFiscalNumberDisplay();
        //SAF-T(PT) : Decimal Format
        string GetDecimalFormatSAFTPT();
        string GetDecimalFormatGrossTotalSAFTPT();
        //Used to Compare, Round first, Compare After
        int GetDecimalRoundTo();
        //RSA Private Key :Sign Finance Documents used in SHA1SignMessage() : Keep This Hidden
        // string GetRsaPrivateKey();
        //SAFT-T XML Export Header
        string GetSaftProductID();
        string GetSaftProductCompanyTaxID();
        string GetSaftSoftwareCertificateNumber();
        string GetSaftVersionPrefix();
        string GetSaftVersion();         
        //Versão da Chave Privada utilizada na criação da Assinatura 
        int GetHashControl();
        //C — Contabilidade();
        //E — Faturação emitida por terceiros();
        //F — Faturação();
        //I — Contabilidade integrada com a faturação();
        //P — Faturação parcial();
        //R — Recibos (a)();
        //S — Autofaturação();
        //T — Documentos de transporte (a).	
        string GetTaxAccountingBasis();
        //Currency Code
        string GetSaftCurrencyCode();

        //SAFT(PT) : Country Rules
        //Retalhistas e vendedores ambulantes é permitida a emissão de faturas simplificadas a não sujeitos passivos, 
        //Até limite de 1000,00€ e a todas as outras atividades é apenas permitida a emissão de faturas até aos 100,00€
        int GetFinanceRuleSimplifiedInvoiceMaxTotal();
        //Services
        int GetFinanceRuleSimplifiedInvoiceMaxTotalServices();
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        int GetFinanceRuleRequiredCustomerDetailsAboveValue();

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SAF-T AO

        //SAF-T(AO) : Formats 
        int GetDocumentsPadLengthAO();
        //SAF-T(AO) : DateTime Formats 
        string GetDateTimeFormatDocumentDateAO();
        string GetDateTimeFormatCombinedDateTimeAO();
        string GetFinanceFinalConsumerFiscalNumberAO();
        string GetFinanceFinalConsumerFiscalNumberDisplayAO();
        //SAF-T(PT) : Decimal Format
        string GetDecimalFormatSAFTAO();
        string GetDecimalFormatGrossTotalSAFTAO();
        //Used to Compare, Round first, Compare After
        //int GetDecimalRoundToAO();
        //RSA Private Key :Sign Finance Documents used in SHA1SignMessage() : Keep This Hidden
        // string GetRsaPrivateKey();
        //SAFT-T XML Export Header
        string GetSaftProductIDAO();
        string GetSaftProductCompanyTaxIDAO();
        string GetSaftSoftwareCertificateNumberAO();
        string GetSaftVersionPrefixAO();
        string GetSaftVersionAO();
        //Versão da Chave Privada utilizada na criação da Assinatura 
        int GetHashControlAO();
        //C — Contabilidade();
        //E — Faturação emitida por terceiros();
        //F — Faturação();
        //I — Contabilidade integrada com a faturação();
        //P — Faturação parcial();
        //R — Recibos (a)();
        //S — Autofaturação();
        //T — Documentos de transporte (a).	
        string GetTaxAccountingBasisAO();
        //Currency Code
        string GetSaftCurrencyCodeAO();

        //SAFT(PT) : Country Rules
        //Retalhistas e vendedores ambulantes é permitida a emissão de faturas simplificadas a não sujeitos passivos, 
        //Até limite de 1000,00€ e a todas as outras atividades é apenas permitida a emissão de faturas até aos 100,00€
        int GetFinanceRuleSimplifiedInvoiceMaxTotalAO();
        //Services
        int GetFinanceRuleSimplifiedInvoiceMaxTotalServicesAO();
        //This rule is to force fill Customer details if total document value is Greater or Equal to
        int GetFinanceRuleRequiredCustomerDetailsAboveValueAO();


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Series

        //Use Alphanumeric Random|Or Numeric Sequential 
        bool GetDocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix();
        string GetDocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat();

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Protected Methods, Protected by SecretKey

        bool IsValidSecretKey(string secretKey);

        string SignDataToSHA1Base64(string secretKey, string encryptData, bool debug = false);

        //Override ZipPack
        //public static bool ZipPack(string[] pFiles, string pFileDestination, string pPassword)
        bool BackupDatabase(string secretKey, string[] files, string fileDestination);

        //Override ZipUnPack
        //public static bool ZipUnPack(string pFileName, string pDestinationPath, string pPassword, bool pFlattenFoldersOnExtract = false)
        bool RestoreBackup(string secretKey, string fileName, string destinationPath, bool flattenFoldersOnExtract = false);

        List<string> GetReportFileName(string secretKey, string filePath, string templateBase);

        void ValidateEmbeddedResources();

        // Methods to Encrypt and Decrypt
        string Encrypt(string toEncrypt);
        string Decrypt(string cipherString);

        // AT WebService
        X509Certificate2 ImportCertificate(bool testModeEnabled, string pathCertificate);
    }
}
