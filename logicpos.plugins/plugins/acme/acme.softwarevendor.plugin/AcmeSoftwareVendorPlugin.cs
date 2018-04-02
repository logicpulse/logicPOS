using logicpos.plugin.contracts;
using logicpos.shared.Classes.Utils;
using acme.softwarevendor.plugin.App;
using System;
using System.Collections.Generic;
using System.IO;

namespace acme.softwarevendor.plugin
{
    public class AcmeSoftwareVendorPlugin : ISoftwareVendor
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            return SettingsApp.AppSoftwareName;
        }

        public string GetAppCompanyName()
        {
            return SettingsApp.AppCompanyName;
        }

        public string GetAppCompanyPhone()
        {
            return SettingsApp.AppCompanyPhone;
        }

        public string GetAppCompanyEmail()
        {
            return SettingsApp.AppCompanyEmail;
        }

        public string GetAppCompanyWeb()
        {
            return SettingsApp.AppCompanyWeb;
        }

        public string GetAppSoftwareVersionFormat()
        {
            return SettingsApp.AppSoftwareVersionFormat;
        }

        public string GetFileFormatDateTime()
        {
            return SettingsApp.FileFormatDateTime;
        }

        public string GetFileFormatSaftPT()
        {
            return SettingsApp.FileFormatSaftPT;
        }

        public int GetDocumentsPadLength()
        {
            return SettingsApp.DocumentsPadLength;
        }

        public string GetDateTimeFormatDocumentDate()
        {
            return SettingsApp.DateTimeFormatDocumentDate;
        }

        public string GetDateTimeFormatCombinedDateTime()
        {
            return SettingsApp.DateTimeFormatCombinedDateTime;
        }

        public string GetFinanceFinalConsumerFiscalNumber()
        {
            return SettingsApp.FinanceFinalConsumerFiscalNumber;
        }

        public string GetFinanceFinalConsumerFiscalNumberDisplay()
        {
            return SettingsApp.FinanceFinalConsumerFiscalNumberDisplay;
        }

        public string GetDecimalFormatSAFTPT()
        {
            return SettingsApp.DecimalFormatSAFTPT;
        }

        public string GetDecimalFormatGrossTotalSAFTPT()
        {
            return SettingsApp.DecimalFormatGrossTotalSAFTPT;
        }

        public int GetDecimalRoundTo()
        {
            return SettingsApp.DecimalRoundTo;
        }

        public string GetSaftProductID()
        {
            return SettingsApp.SaftProductID;
        }

        public string GetSaftProductCompanyTaxID()
        {
            return SettingsApp.SaftProductCompanyTaxID;
        }

        public string GetSaftSoftwareCertificateNumber()
        {
            return SettingsApp.SaftSoftwareCertificateNumber;
        }

        public string GetSaftVersionPrefix()
        {
            return SettingsApp.SaftVersionPrefix;
        }

        public string GetSaftVersion()
        {
            return SettingsApp.SaftVersion;
        }

        public int GetHashControl()
        {
            return SettingsApp.HashControl;
        }

        public string GetTaxAccountingBasis()
        {
            return SettingsApp.TaxAccountingBasis;
        }

        public string GetSaftCurrencyCode()
        {
            return SettingsApp.SaftCurrencyCode;
        }

        public int GetFinanceRuleSimplifiedInvoiceMaxTotal()
        {
            return SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal;
        }

        public int GetFinanceRuleSimplifiedInvoiceMaxTotalServices()
        {
            return SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices;
        }

        public int GetFinanceRuleRequiredCustomerDetailsAboveValue()
        {
            return SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue;
        }

        public bool GetDocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix()
        {
            return SettingsApp.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix;
        }

        public string GetDocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat()
        {
            return SettingsApp.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Protected Methods

        public bool IsValidSecretKey(string secretKey)
        {
            return (secretKey.Equals(SettingsApp.SecretKey));
        }

        public string SignDataToSHA1Base64(string secretKey, string encryptData, bool debug = false)
        {
            if (IsValidSecretKey(secretKey))
            {
                return FrameworkUtils.SignDataToSHA1Base64(SettingsApp.RsaPrivateKey, encryptData, debug);
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
                    result = Utils.ZipPack(files, fileDestination, SettingsApp.BackupPassword);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
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
                    result = Utils.ZipUnPack(fileName, destinationPath, SettingsApp.BackupPassword, flattenFoldersOnExtract);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
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
                string templateContent = FrameworkUtils.StreamToString(stream);
                // Get ReportContent
                stream = GetType().Module.Assembly.GetManifestResourceStream(resourceReportLocation);
                string reportContent = FrameworkUtils.StreamToString(stream);

                string randomPrefix = Utils.GenerateRandomString(8);
                string targetTemplateFileName = $"{randomPrefix}.{templateBase}";
                string targetTemplateFilePath = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["temp"], targetTemplateFileName));
                string targetReportFileName = $"{randomPrefix}.{reportName}";
                string targetReportFilePath = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["temp"], targetReportFileName));

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
                    _log.Debug(string.Format("GetReportFileName templateContent: [{0}]", templateContent));
                    _log.Debug(string.Format("GetReportFileName reportContent: [{0}]", reportContent));
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

        public void ValidateEmbbededResources()
        {
             bool debug = false;
            string resourcePathLocation = "Resources/Reports/UserReports/";
            string resourceBaseLocation = "acme.softwarevendor.plugin.Resources.Reports.UserReports.{0}";
            string resourceLocation = string.Empty;
            string[] files = Directory.GetFiles(resourcePathLocation, "*.frx");
            List<string> emmbededFilesMissing = new List<string>();
            string fileName = string.Empty;
            var resources = GlobalFramework.PluginSoftwareVendor.GetType().Assembly.GetManifestResourceNames();
            Stream stream = null;

            foreach (var item in files)
            {
                fileName = item.Replace(resourcePathLocation, string.Empty);
                resourceLocation = string.Format(resourceBaseLocation, fileName);
                if (debug) _log.Debug(string.Format("fileName: [{0}], resourceLocation: [{1}]", fileName, resourceLocation));

                try
                {
                    // Try Get Resource
                    stream = GetType().Module.Assembly.GetManifestResourceStream(resourceLocation);
                    if (stream == null)
                    {
                        emmbededFilesMissing.Add(resourceLocation);
                    }
                }
                catch (Exception ex)
                {
                    if (debug) _log.Error(ex.Message, ex);
                }
            }

            if (emmbededFilesMissing.Count > 0)
            {
                throw new Exception(string.Format("Error! Detected files not Embbeded in Resources! [{0}]", String.Join(", ", emmbededFilesMissing)));
            }
        }
    }
}
