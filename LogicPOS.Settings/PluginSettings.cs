using LogicPOS.Plugin.Abstractions;
using LogicPOS.Plugin.Utils;
using System;
using System.Reflection;

namespace LogicPOS.Settings
{
    public static class PluginSettings
    {
        public static string AppSoftwareName { get; set; }
        public static string AppCompanyName { get; set; }
        public static string AppCompanyPhone { get; set; }
        public static string AppCompanyEmail { get; set; }
        public static string AppCompanyWeb { get; set; }
        public static string AppSoftwareVersionFormat { get; set; }

        public static PluginContainer PluginContainer { get; set; }
        public static ISoftwareVendor PluginSoftwareVendor { get; set; }
        public static ISoftwareVendor SoftwareVendor { get; set; }
        public static ILicenseManager LicenceManager { get; set; }

        public static string GetSoftwareVendorValueAsString(string property)
        {
            return Convert.ToString(GetPluginSoftwareVendorValue(property));
        }

        public static int GetSoftwareVendorValueAsInt(string property)
        {
            return Convert.ToInt16(GetPluginSoftwareVendorValue(property));
        }

        public static bool GetSoftwareVendorValueAsBool(string property)
        {
            return Convert.ToBoolean(GetPluginSoftwareVendorValue(property));
        }

        public static object GetPluginSoftwareVendorValue(string property)
        {
            object resultObject = null;

            if (SoftwareVendor != null)
            {
                Type thisType = SoftwareVendor.GetType();
                string methodName = $"Get{property}";
                MethodInfo methodInfo = thisType.GetMethod(methodName);
                object[] methodParameters = null;
                resultObject = methodInfo.Invoke(SoftwareVendor, methodParameters);
            }
            if (resultObject != null)
            {
                return resultObject;
            }
            else
            {
                return null;
            }
        }

        public static bool HasPlugin => SoftwareVendor != null;

        public static void InitSoftwareVendorPluginSettings()
        {
            AppSoftwareName = GetSoftwareVendorValueAsString(nameof(AppSoftwareName));
            AppCompanyName = GetSoftwareVendorValueAsString(nameof(AppCompanyName));
            AppCompanyPhone = GetSoftwareVendorValueAsString(nameof(AppCompanyPhone));
            AppCompanyEmail = GetSoftwareVendorValueAsString(nameof(AppCompanyEmail));
            AppCompanyWeb = GetSoftwareVendorValueAsString(nameof(AppCompanyWeb));
            AppSoftwareVersionFormat = GetSoftwareVendorValueAsString(nameof(AppSoftwareVersionFormat));

            CultureSettings.FileFormatDateTime = GetSoftwareVendorValueAsString(nameof(CultureSettings.FileFormatDateTime));
            CultureSettings.FileFormatSaftPT = GetSoftwareVendorValueAsString(nameof(CultureSettings.FileFormatSaftPT));
            CultureSettings.FileFormatSaftAO = GetSoftwareVendorValueAsString(nameof(CultureSettings.FileFormatSaftAO));

            SaftSettings.DocumentsPadLength = GetSoftwareVendorValueAsInt(nameof(SaftSettings.DocumentsPadLength));
            CultureSettings.DateTimeFormatDocumentDate = GetSoftwareVendorValueAsString(nameof(CultureSettings.DateTimeFormatDocumentDate));
            CultureSettings.DateTimeFormatCombinedDateTime = GetSoftwareVendorValueAsString(nameof(CultureSettings.DateTimeFormatCombinedDateTime));
            SaftSettings.FinanceFinalConsumerFiscalNumber = GetSoftwareVendorValueAsString(nameof(SaftSettings.FinanceFinalConsumerFiscalNumber));
            SaftSettings.FinanceFinalConsumerFiscalNumberDisplay = GetSoftwareVendorValueAsString(nameof(SaftSettings.FinanceFinalConsumerFiscalNumberDisplay));
            CultureSettings.DecimalFormatSAFTPT = GetSoftwareVendorValueAsString(nameof(CultureSettings.DecimalFormatSAFTPT));
            CultureSettings.DecimalFormatSAFTAO = GetSoftwareVendorValueAsString(nameof(CultureSettings.DecimalFormatSAFTAO));
            CultureSettings.DecimalFormatGrossTotalSAFTPT = GetSoftwareVendorValueAsString(nameof(CultureSettings.DecimalFormatGrossTotalSAFTPT));
            CultureSettings.DecimalRoundTo = GetSoftwareVendorValueAsInt(nameof(CultureSettings.DecimalRoundTo));
            SaftSettings.SaftProductCompanyTaxID = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftProductCompanyTaxID));
            SaftSettings.SaftSoftwareCertificateNumber = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftSoftwareCertificateNumber));
            SaftSettings.SaftSoftwareCertificateNumberAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftSoftwareCertificateNumberAO));
            SaftSettings.SaftVersionPrefix = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionPrefix));
            SaftSettings.SaftVersionPrefixAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionPrefixAO));
            SaftSettings.SaftVersion = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersion));
            SaftSettings.SaftVersionAO = GetSoftwareVendorValueAsString(nameof(SaftSettings.SaftVersionAO));
            DocumentSettings.HashControl = GetSoftwareVendorValueAsInt(nameof(DocumentSettings.HashControl));
            SaftSettings.TaxAccountingBasis = GetSoftwareVendorValueAsString(nameof(SaftSettings.TaxAccountingBasis));
            CultureSettings.SaftCurrencyCode = GetSoftwareVendorValueAsString(nameof(CultureSettings.SaftCurrencyCode));
            CultureSettings.SaftCurrencyCodeAO = GetSoftwareVendorValueAsString(nameof(CultureSettings.SaftCurrencyCodeAO));

            DocumentSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix = GetSoftwareVendorValueAsBool(nameof(DocumentSettings.DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix));
            DocumentSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat = GetSoftwareVendorValueAsString(nameof(DocumentSettings.DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat));
        }
    }
}
