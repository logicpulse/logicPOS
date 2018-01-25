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
        public static bool ServiceATSendDocuments = Convert.ToBoolean(GlobalFramework.Settings["serviceATSendDocuments"]);
        public static bool ServiceATSendDocumentsWayBill = Convert.ToBoolean(GlobalFramework.Settings["serviceATSendDocumentsWayBill"]);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // AT Web Services

        // Enable/Disable TestMode
        public static bool ServicesATEnableTestMode = Convert.ToBoolean(GlobalFramework.Settings["servicesATEnableTestMode"]);
        public static bool ServicesATWBAgriculturalMode = Convert.ToBoolean(GlobalFramework.Settings["servicesATWBAgriculturalMode"]);

        //Uris
        public static Uri ServicesATUriDocuments { get { return GetServicesATDCUri(ServicesATEnableTestMode); } }
        public static Uri ServicesATUriDocumentsSOAPAction = new Uri("http://servicos.portaldasfinancas.gov.pt/faturas/RegisterInvoice");

        //From Shared Parameters
        public static string ServicesATFilePublicKey { get { return GetServicesATFilePublicKey(ServicesATEnableTestMode); } }
        public static string ServicesATFileCertificate { get { return GetServicesATFileCertificate(ServicesATEnableTestMode); } }
        public static string ServicesATCertificatePassword { get { return GetServicesATCertificatePassword(ServicesATEnableTestMode); } }
        public static string ServicesATTaxRegistrationNumber { get { return GetServicesATTaxRegistrationNumber(ServicesATEnableTestMode); } }
        //User of "portal das finanças"
        public static string ServicesATAccountFiscalNumber { get { return GetServicesATAccountFiscalNumber(ServicesATEnableTestMode); } }
        public static string ServicesATAccountPassword { get { return GetServicesATAccountPassword(ServicesATEnableTestMode); } }

        //DocumentsWayBill(Agricultural)
        public static Uri ServicesATUriDocumentsWayBill { get { return GetServicesATWBUri(ServicesATEnableTestMode, ServicesATWBAgriculturalMode); } }
        public static Uri ServicesATUriDocumentsWayBillSOAPAction = new Uri("https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/");

        //From Shared Parameters
        public static int ServicesATRequestTimeout = Convert.ToInt16(GlobalFramework.Settings["servicesATRequestTimeout"]);
        public static string ServicesATWBFilePublicKey { get { return GetServicesATFilePublicKey(ServicesATEnableTestMode); } }
        public static string ServicesATWBFileCertificate { get { return GetServicesATFileCertificate(ServicesATEnableTestMode); } }
        public static string ServicesATWBCertificatePassword { get { return GetServicesATCertificatePassword(ServicesATEnableTestMode); } }
        public static string ServicesATWBTaxRegistrationNumber { get { return GetServicesATTaxRegistrationNumber(ServicesATEnableTestMode); } }
        //User of "portal das finanças"
        public static string ServicesATWBAccountFiscalNumber { get { return GetServicesATAccountFiscalNumber(ServicesATEnableTestMode); } }
        //Pass of "portal das finanças"
        public static string ServicesATWBAccountPassword { get { return GetServicesATAccountPassword(ServicesATEnableTestMode); } }

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
            string companyFiscalNumber = GlobalFramework.PreferenceParameters["COMPANY_FISCALNUMBER"];

            return (pTestMode) ? "599999993" : companyFiscalNumber;//"508278155"
        }

        private static string GetServicesATAccountFiscalNumber(bool pTestMode)
        {
            return (pTestMode)
                ? "599999993/0037"
                : GlobalFramework.Settings["servicesATProdModeAccountFiscalNumber"];//" "508278155/2";
        }

        private static string GetServicesATAccountPassword(bool pTestMode)
        {
            return (pTestMode)
                ? "testes1234"
                : GlobalFramework.Settings["servicesATProdModeAccountPassword"];//"logicpulse#2015X";
        }

        private static string GetServicesATCertificatePassword(bool pTestMode)
        {
            return (pTestMode)
                ? "TESTEwebservice"
                : GlobalFramework.Settings["servicesATProdModeCertificatePassword"];//" "logicpulse#2015X";
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
