using System;
using System.IO;

namespace logicpos.financial.service.App
{
    public class SettingsApp : financial.library.App.SettingsApp
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer

#if (DEBUG)
        public static string DatabaseName = "logicposdb";
        //public static string AppHardwareId = "92A4-3CA3-0CFD-4FF4-2962-5379";
#else
        public static string DatabaseName = "logicposdb";
        //public static string AppHardwareId = string.Empty;
#endif

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Application

        public static string AppName = "Framework Console Service Project";
        //Required to load files in Service Mode else uses C:\WINDOWS\system32
        public static string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Formats

        //Dont change This, Required to use . in many things like SAF-T etc
        public static string CultureNumberFormat = "en-US";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Add On Parameters to Console Service Project

        //private static string _pathCertificates = FrameworkUtils.OSSlash(GlobalFramework.Settings["pathCertificates"]);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Service Configuration

        public static bool ServiceTimerEnabled = Convert.ToBoolean(GlobalFramework.Settings["serviceTimerEnabled"]);
        public static double ServiceTimerInterval = Convert.ToDouble(GlobalFramework.Settings["serviceTimerInterval"]);
        public static DateTime ServiceTimer = DateTime.ParseExact((GlobalFramework.Settings["serviceTimer"]), "H:mm", null, System.Globalization.DateTimeStyles.None); 

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services

        //Todo: CleanUp

        // Enable/Disable TestMode
        public static bool ServiceATEnableTestMode { get { return GetServiceATEnableTestMode(); } }
        public static bool ServiceATSendDocuments { get { return GetServiceATSendDocuments(); } }
        public static bool ServiceATSendDocumentsWayBill { get { return GetServiceATSendDocumentsWayBill(); } }
        public static bool ServiceATWBAgriculturalMode { get { return GetServiceATWBAgriculturalMode(); } }

        //Uris
        public static Uri ServicesATUriDocuments { get { return GetServicesATDCUri(ServiceATEnableTestMode); } }
        public static Uri ServicesATUriDocumentsSOAPAction = new Uri("http://servicos.portaldasfinancas.gov.pt/faturas/RegisterInvoice");

        //From Shared Parameters
        public static string ServicesATFilePublicKey { get { return GetServicesATFilePublicKey(ServiceATEnableTestMode); } }
        public static string ServicesATFileCertificate { get { return GetServicesATFileCertificate(ServiceATEnableTestMode); } }
        public static string ServicesATCertificatePassword { get { return GetServicesATCertificatePassword(ServiceATEnableTestMode); } }
        public static string ServicesATTaxRegistrationNumber { get { return GetServicesATTaxRegistrationNumber(ServiceATEnableTestMode); } }
        //User of "portal das finanças"
        public static string ServicesATAccountFiscalNumber { get { return GetServicesATAccountFiscalNumber(ServiceATEnableTestMode); } }
        public static string ServicesATAccountPassword { get { return GetServicesATAccountPassword(ServiceATEnableTestMode); } }

        //DocumentsWayBill(Agricultural)
        public static Uri ServicesATUriDocumentsWayBill { get { return GetServicesATWBUri(ServiceATEnableTestMode, ServiceATWBAgriculturalMode); } }
        public static Uri ServicesATUriDocumentsWayBillSOAPAction = new Uri("https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/");

        //From Shared Parameters
        public static int ServicesATRequestTimeout = Convert.ToInt16(GlobalFramework.Settings["servicesATRequestTimeout"]);
        public static string ServicesATWBFilePublicKey { get { return GetServicesATFilePublicKey(ServiceATEnableTestMode); } }
        public static string ServicesATWBFileCertificate { get { return GetServicesATFileCertificate(ServiceATEnableTestMode); } }
        public static string ServicesATWBCertificatePassword { get { return GetServicesATCertificatePassword(ServiceATEnableTestMode); } }
        public static string ServicesATWBTaxRegistrationNumber { get { return GetServicesATTaxRegistrationNumber(ServiceATEnableTestMode); } }
        //User of "portal das finanças"
        public static string ServicesATWBAccountFiscalNumber { get { return GetServicesATAccountFiscalNumber(ServiceATEnableTestMode); } }
        //Pass of "portal das finanças"
        public static string ServicesATWBAccountPassword { get { return GetServicesATAccountPassword(ServiceATEnableTestMode); } }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services : SendDocuments and SendDocumentsWayBill

        private static bool GetServiceATEnableTestMode()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(GlobalFramework.PreferenceParameters["SERVICE_AT_PRODUCTION_MODE_ENABLED"]))
            {
                result = !Convert.ToBoolean(GlobalFramework.PreferenceParameters["SERVICE_AT_PRODUCTION_MODE_ENABLED"]);
            }

            return result;
        }

        private static bool GetServiceATSendDocuments()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(GlobalFramework.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS"]))
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS"]);
            }

            return result;
        }

        private static bool GetServiceATSendDocumentsWayBill()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(GlobalFramework.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS_WAYBILL"]))
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["SERVICE_AT_SEND_DOCUMENTS_WAYBILL"]);
            }

            return result;
        }

        private static bool GetServiceATWBAgriculturalMode()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(GlobalFramework.PreferenceParameters["SERVICE_AT_WAYBILL_AGRICULTURAL_MODE_ENABLED"]))
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["SERVICE_AT_WAYBILL_AGRICULTURAL_MODE_ENABLED"]);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services : Shared for Documents|WayBill(Agricultural)

        private static string GetServicesATFilePublicKey(bool pTestMode)
        {
            return (pTestMode)
                ? string.Format(@"{0}{1}", GlobalFramework.Path["certificates"], GlobalFramework.Settings["servicesATTestModeFilePublicKey"])
                : string.Format(@"{0}{1}", GlobalFramework.Path["certificates"], GlobalFramework.Settings["servicesATProdModeFilePublicKey"])
            ;
        }

        private static string GetServicesATFileCertificate(bool pTestMode)
        {
            return (pTestMode)
                ? string.Format(@"{0}{1}", GlobalFramework.Path["certificates"], GlobalFramework.Settings["servicesATTestModeFileCertificate"])
                : string.Format(@"{0}{1}", GlobalFramework.Path["certificates"], GlobalFramework.Settings["servicesATProdModeFileCertificate"])
            ;
        }

        private static string GetServicesATTaxRegistrationNumber(bool pTestMode)
        {
            return (pTestMode)
                ? "599999993"
                : GlobalFramework.PreferenceParameters["COMPANY_FISCALNUMBER"];
        }

        private static string GetServicesATAccountFiscalNumber(bool pTestMode)
        {
            //GlobalFramework.Settings["servicesATProdModeAccountFiscalNumber"];
            return (pTestMode)
                ? "599999993/0037"
                : GlobalFramework.PreferenceParameters["SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER"];
        }

        private static string GetServicesATAccountPassword(bool pTestMode)
        {
            //GlobalFramework.Settings["servicesATProdModeAccountPassword"];
            return (pTestMode)
                ? "testes1234"
                : GlobalFramework.PreferenceParameters["SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD"];
        }

        private static string GetServicesATCertificatePassword(bool pTestMode)
        {
            //GlobalFramework.Settings["servicesATProdModeCertificatePassword"];
            return (pTestMode)
                ? "TESTEwebservice"
                : GlobalFramework.PluginSoftwareVendor.GetAppSoftwareATWSProdModeCertificatePassword();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services : Documents

        private static Uri GetServicesATDCUri(bool pTestMode)
        {
            return (pTestMode)
                ? new Uri("https://servicos.portaldasfinancas.gov.pt:700/fews/faturas")
                : new Uri("https://servicos.portaldasfinancas.gov.pt:400/fews/faturas")
            ;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services : DocumentsWayBill

        private static Uri GetServicesATWBUri(bool pTestMode, bool pAgricultural)
        {
            if (!pAgricultural)
            {
                //Normal Mode : Documentos de transporte:
                return (pTestMode)
                    ? new Uri("https://servicos.portaldasfinancas.gov.pt:701/sgdtws/documentosTransporte")
                    : new Uri("https://servicos.portaldasfinancas.gov.pt:401/sgdtws/documentosTransporte")
                ;
            }
            else
            {
                //Agricultural Mode : Guias de aquisição de produtos de produtores agrícolas:
                return (pTestMode)
                    ? new Uri("https://servicos.portaldasfinancas.gov.pt:702/sgdtws/GuiasAquisicaoProdAgricola")
                    : new Uri("https://servicos.portaldasfinancas.gov.pt:402/sgdtws/GuiasAquisicaoProdAgricola")
                ;
            }
        }
    }
}
