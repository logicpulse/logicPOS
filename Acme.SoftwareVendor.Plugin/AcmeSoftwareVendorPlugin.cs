using acme.softwarevendor.plugin.App;
using logicpos;
using LogicPOS.Plugin.Abstractions;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace acme.softwarevendor.plugin
{
    public class AcmeSoftwareVendorPlugin : ISoftwareVendor
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Name { get => "AcmeSoftwareVendorPlugin"; }

        public Type BaseType { get => typeof(IPlugin); }

        public Type Interface { get => typeof(ISoftwareVendor); }

        public AcmeSoftwareVendorPlugin()
        {
        }

        public void Do()
        {
            throw new NotImplementedException();
        }

        public string GetAppSoftwareName()
        {
            return AcmeSettings.AppSoftwareName;
        }

        public string GetAppCompanyName()
        {
            return AcmeSettings.AppCompanyName;
        }

        public string GetAppCompanyPhone()
        {
            return AcmeSettings.AppCompanyPhone;
        }

        public string GetAppCompanyEmail()
        {
            return AcmeSettings.AppCompanyEmail;
        }

        public string GetAppCompanyWeb()
        {
            return AcmeSettings.AppCompanyWeb;
        }

        public string GetAppSoftwareVersionFormat()
        {
            return AcmeSettings.AppSoftwareVersionFormat;
        }

        public string GetAppSoftwareATWSProdModeCertificatePassword()
        {
            return AcmeSettings.AppSoftwareATWSProdModeCertificatePassword;
        }

        public string GetFileFormatDateTime()
        {
            return AcmeSettings.FileFormatDateTime;
        }

        public string GetFileFormatSaftPT()
        {
            return AcmeSettings.FileFormatSaftPT;
        }

        public int GetDocumentsPadLength()
        {
            return AcmeSettings.DocumentsPadLength;
        }

        public string GetDateTimeFormatDocumentDate()
        {
            return AcmeSettings.DateTimeFormatDocumentDate;
        }

        public string GetDateTimeFormatCombinedDateTime()
        {
            return AcmeSettings.DateTimeFormatCombinedDateTime;
        }

        public string GetFinanceFinalConsumerFiscalNumber()
        {
            return AcmeSettings.FinanceFinalConsumerFiscalNumber;
        }

        public string GetFinanceFinalConsumerFiscalNumberDisplay()
        {
            return AcmeSettings.FinanceFinalConsumerFiscalNumberDisplay;
        }

        public string GetDecimalFormatSAFTPT()
        {
            return AcmeSettings.DecimalFormatSAFTPT;
        }

        public string GetDecimalFormatGrossTotalSAFTPT()
        {
            return AcmeSettings.DecimalFormatGrossTotalSAFTPT;
        }

        public int GetDecimalRoundTo()
        {
            return AcmeSettings.DecimalRoundTo;
        }

        public string GetSaftProductID()
        {
            return AcmeSettings.SaftProductID;
        }

        public string GetSaftProductCompanyTaxID()
        {
            return AcmeSettings.SaftProductCompanyTaxID;
        }

        public string GetSaftSoftwareCertificateNumber()
        {
            return AcmeSettings.SaftSoftwareCertificateNumber;
        }

        public string GetSaftVersionPrefix()
        {
            return AcmeSettings.SaftVersionPrefix;
        }

        public string GetSaftVersion()
        {
            return AcmeSettings.SaftVersion;
        }

        public int GetHashControl()
        {
            return AcmeSettings.HashControl;
        }

        public string GetTaxAccountingBasis()
        {
            return AcmeSettings.TaxAccountingBasis;
        }

        public string GetSaftCurrencyCode()
        {
            return AcmeSettings.SaftCurrencyCode;
        }

        public int GetSimplifiedInvoiceMaxItems()
        {
            return AcmeSettings.FinanceRuleSimplifiedInvoiceMaxTotal;
        }

        public int GetSimplifiedInvoiceMaxServices()
        {
            return AcmeSettings.FinanceRuleSimplifiedInvoiceMaxTotalServices;
        }

        public int GetRequiredCustomerDetailsAboveValue()
        {
            return AcmeSettings.FinanceRuleRequiredCustomerDetailsAboveValue;
        }

        public bool GetDocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix()
        {
            return AcmeSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix;
        }

        public string GetDocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat()
        {
            return AcmeSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Protected Methods

        public bool IsValidSecretKey(string secretKey)
        {
            return (secretKey.Equals(AcmeSettings.SecretKey));
        }

        public string SignDataToSHA1Base64(string secretKey, string encryptData, bool debug = false)
        {
            if (IsValidSecretKey(secretKey))
            {
                throw new NotImplementedException();
                //return FrameworkUtils.SignDataToSHA1Base64(SettingsApp.RsaPrivateKey, encryptData, debug); -> tchial0
            }
            else
            {
                return "Invalid SecretKey";
            }
        }

        public bool BackupDatabase(string secretKey, string[] files, string fileDestination)
        {
            bool result = false;

            if (IsValidSecretKey(secretKey))
            {
                try
                {
                    result = LogicPOS.Utility.Compression.ZipPack(files, fileDestination, AcmeSettings.BackupPassword);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }

            return result;
        }

        public bool RestoreBackup(string secretKey, string fileName, string destinationPath, bool flattenFoldersOnExtract = false)
        {
            bool result = false;

            if (IsValidSecretKey(secretKey))
            {
                try
                {
                    result = LogicPOS.Utility.Compression.ZipUnPack(fileName, destinationPath, AcmeSettings.BackupPassword, flattenFoldersOnExtract);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }

            return result;
        }

        public List<string> GetReportFileName(string secretKey, string filePath, string templateBase)
        {
            List<string> result = new List<string>();

            if (!IsValidSecretKey(secretKey)) return result;

            bool debug = false;
            string removeLocationPrefix = "Resources/Reports/UserReports/";
            string reportName = filePath.Replace(removeLocationPrefix, string.Empty);
            string resourceBaseLocation = "logicpulse.softwarevendor.plugin.Resources.Reports.UserReports.{0}";
            string resourceTemplateLocation = string.Format(resourceBaseLocation, templateBase);
            string resourceReportLocation = string.Format(resourceBaseLocation, reportName);

            try
            {
                // Get TemplateContent
                Stream stream = GetType().Module.Assembly.GetManifestResourceStream(resourceTemplateLocation);
                string templateContent = StringUtils.StreamToString(stream);
                // Get ReportContent
                stream = GetType().Module.Assembly.GetManifestResourceStream(resourceReportLocation);
                string reportContent = StringUtils.StreamToString(stream);

                string randomPrefix = LogicPOS.Utility.StringUtils.GenerateRandomString(8);
                string targetTemplateFileName = $"{randomPrefix}.{templateBase}";
                string targetTemplateFilePath = $"{PathsSettings.TempFolderLocation}{targetTemplateFileName}";
                string targetReportFileName = $"{randomPrefix}.{reportName}";
                string targetReportFilePath = $"{PathsSettings.TempFolderLocation}{targetReportFileName}";

                // Replace templateBase (TemplateBase.frx) with targetTemplateFileName, WE MUST Change Template Name in Template Childs Sub Reports
                if (reportContent.Contains(templateBase))
                {
                    reportContent = reportContent.Replace(templateBase, targetTemplateFileName);
                }
                else
                {
                    // Throw exception to Caller if we dont replace the Template 
                    throw new Exception(string.Format("Error! Cant find TemplateBase:[{0}] in ChildTemplate", templateBase));
                }

                if (debug)
                {
                    _logger.Debug(string.Format("GetReportFileName templateContent: [{0}]", templateContent));
                    _logger.Debug(string.Format("GetReportFileName reportContent: [{0}]", reportContent));
                }

                // Save Temporary Template/Report
                File.WriteAllText(targetTemplateFilePath, templateContent);
                File.WriteAllText(targetReportFilePath, reportContent);

                // Add Files to result, MainReport File used in Load() is always index 0
                result.Add(targetReportFilePath);
                result.Add(targetTemplateFilePath);
            }
            catch (Exception ex)
            {
                // Throw exception to Caller
                throw ex;
            }

            return result;
        }

        public void ValidateEmbeddedResources()
        {
            bool debug = false;
            string resourcePathLocation = "Resources/Reports/UserReports/";
            string resourceBaseLocation = "acme.softwarevendor.plugin.Resources.Reports.UserReports.{0}";
            string[] files = Directory.GetFiles(resourcePathLocation, "*.frx");
            List<string> emmbededFilesMissing = new List<string>();
            var resources = LogicPOS.Settings.PluginSettings.SoftwareVendor.GetType().Assembly.GetManifestResourceNames();
            foreach (var item in files)
            {
                string fileName = item.Replace(resourcePathLocation, string.Empty);
                string resourceLocation = string.Format(resourceBaseLocation, fileName);
                if (debug) _logger.Debug(string.Format("fileName: [{0}], resourceLocation: [{1}]", fileName, resourceLocation));

                try
                {
                    // Try Get Resource
                    Stream stream = GetType().Module.Assembly.GetManifestResourceStream(resourceLocation);
                    if (stream == null)
                    {
                        emmbededFilesMissing.Add(resourceLocation);
                    }
                }
                catch (Exception ex)
                {
                    if (debug) _logger.Error(ex.Message, ex);
                }
            }

            if (emmbededFilesMissing.Count > 0)
            {
                throw new Exception(string.Format("Error! Detected files not Embedded in Resources! [{0}]", string.Join(", ", emmbededFilesMissing)));
            }
        }

        public string Encrypt(string toEncrypt)
        {
            string result = toEncrypt;

            try
            {
                result = CryptographyUtils.Encrypt(toEncrypt, true, AcmeSettings.SecretKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public string Decrypt(string cipherString)
        {
            string result = cipherString;

            try
            {
                result = CryptographyUtils.Decrypt(cipherString, true, AcmeSettings.SecretKey);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Import certificate Inside Plugin and Return X509Certificate2 to be used Outside, 
        /// this is a requirenmente to protect the ATWSProdModeCertificatePassword
        /// </summary>
        /// <param name="pathCertificate"></param>
        /// <returns></returns>
        public X509Certificate2 ImportCertificate(bool testModeEnabled, string pathCertificate)
        {
            X509Certificate2 cert = new X509Certificate2();
            //From user installed Certificates
            //cert.Import(_pathCertificate, _passwordCertificate, X509KeyStorageFlags.DefaultKeySet);
            //From FileSystem "Resources\Certificates"
            string password = (testModeEnabled)
                ? AcmeSettings.AppSoftwareATWSProdModeCertificatePassword
                : AcmeSettings.AppSoftwareATWSTestModeCertificatePassword;
            //Import Certificate
            cert.Import(pathCertificate, password, X509KeyStorageFlags.Exportable);

            return cert;
        }

        public int GetDocumentsPadLengthAO()
        {
            throw new NotImplementedException();
        }

        public string GetDateTimeFormatDocumentDateAO()
        {
            throw new NotImplementedException();
        }

        public string GetDateTimeFormatCombinedDateTimeAO()
        {
            throw new NotImplementedException();
        }

        public string GetFinanceFinalConsumerFiscalNumberAO()
        {
            throw new NotImplementedException();
        }

        public string GetFinanceFinalConsumerFiscalNumberDisplayAO()
        {
            throw new NotImplementedException();
        }

        public string GetDecimalFormatSAFTAO()
        {
            throw new NotImplementedException();
        }

        public string GetDecimalFormatGrossTotalSAFTAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftProductIDAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftProductCompanyTaxIDAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftSoftwareCertificateNumberAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftVersionPrefixAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftVersionAO()
        {
            throw new NotImplementedException();
        }

        public int GetHashControlAO()
        {
            throw new NotImplementedException();
        }

        public string GetTaxAccountingBasisAO()
        {
            throw new NotImplementedException();
        }

        public string GetSaftCurrencyCodeAO()
        {
            throw new NotImplementedException();
        }

        public int GetFinanceRuleSimplifiedInvoiceMaxTotalAO()
        {
            throw new NotImplementedException();
        }

        public int GetFinanceRuleSimplifiedInvoiceMaxTotalServicesAO()
        {
            throw new NotImplementedException();
        }

        public int GetFinanceRuleRequiredCustomerDetailsAboveValueAO()
        {
            throw new NotImplementedException();
        }
    }
}
