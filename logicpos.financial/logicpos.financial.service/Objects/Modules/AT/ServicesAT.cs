using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.shared.App;
using LogicPOS.DTOs.Common;
using LogicPOS.Settings;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace logicpos.financial.service.Objects.Modules.AT
{
    public class ServicesAT
    {
        //Log4Net
        protected log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool ValidCerificates { get; set; } = false;
        private readonly bool _wayBillMode;
        private readonly Uri _urlWebService;
        private readonly Uri _urlSoapAction;
        //Change this to test Mode
        private readonly bool testMode = false;
        private readonly string _appPath = string.Empty;
        private string _postData;
        private readonly string _pathSaveSoap;
        private readonly string _pathSaveSoapResult;
        private readonly string _pathSaveSoapResultError;
        private readonly string _pathPublicKey;
        private readonly string _pathCertificate;
        private readonly string _atPasswordCertificate;
        private readonly string _atTaxRegistrationNumber;
        private readonly string _atAccountFiscalNumber;
        private readonly string _atAccountPassword;
        //Generated on buildCredentials()
        private string _accountPasswordEncrypted = string.Empty;
        private string _symetricKeyEncrypted = string.Empty;
        private string _dateOfCriationEncrypted = string.Empty;
        //Sample Document Details
        private readonly bool _useMockSampleData = false;
        //IncreaseDocumentNumber
        private readonly bool _increaseDocumentNumber = false;
        //Override Document Date, used in WayBills MovementStartTime to be equal/greater than SystemTime else -100
        private DateTime _movementStartTime;
        //Sample Document Details : Documents
        private readonly string _sampleDCInvoiceNo = "FS 10001/1033";
        private readonly string _sampleDCInvoiceDate = "2016-07-14";
        private readonly string _sampleDCInvoiceType = "FS";
        //Sample Document Details : DocumentWayBill
        private readonly string _sampleWBDocumentNumber = "GT 2013/1";
        private readonly string _sampleWBMovementDate = "2016-07-09";
        private readonly string _sampleWBMovementType = "GT";
        //Sample Document Details : Shared for Documents and DocumentWayBill
        private readonly string _sampleXXCustomerTaxID = "508278155";//299999998 | 111111111

        
        //XPObjects
        private readonly fin_documentfinancemaster _documentMaster;

        //Response
        private ServicesATSoapResult _soapResult = null;
        public ServicesATSoapResult SoapResult
        {
            get { return (ServicesATSoapResult)_soapResult; }
            set { _soapResult = value; }
        }

        private static string GetServicesATFilePublicKey(bool pTestMode)
        {
            return (pTestMode)
                ? string.Format(@"{0}{1}", GeneralSettings.Path["certificates"], GeneralSettings.Settings["servicesATTestModeFilePublicKey"])
                : string.Format(@"{0}{1}", GeneralSettings.Path["certificates"], GeneralSettings.Settings["servicesATProdModeFilePublicKey"])
            ;
        }

        private static string GetServicesATFileCertificate(bool pTestMode)
        {
            return (pTestMode)
                ? string.Format(@"{0}{1}", GeneralSettings.Path["certificates"], GeneralSettings.Settings["servicesATTestModeFileCertificate"])
                : string.Format(@"{0}{1}", GeneralSettings.Path["certificates"], GeneralSettings.Settings["servicesATProdModeFileCertificate"])
            ;
        }

        private static string GetServicesATTaxRegistrationNumber(bool pTestMode)
        {
            return (pTestMode)
                ? "599999993"
                : GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
        }

        private static string GetServicesATAccountFiscalNumber(bool pTestMode)
        {
            //LogicPOS.Settings.GeneralSettings.Settings["servicesATProdModeAccountFiscalNumber"];
            return (pTestMode)
                ? "599999993/0037"
                : GeneralSettings.PreferenceParameters["SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER"];
        }

        private static string GetServicesATAccountPassword(bool pTestMode)
        {
            //LogicPOS.Settings.GeneralSettings.Settings["servicesATProdModeAccountPassword"];
            return (pTestMode)
                ? "testes1234"
                : GeneralSettings.PreferenceParameters["SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD"];
        }

        private static string GetServicesATCertificatePassword(bool pTestMode)
        {
            //LogicPOS.Settings.GeneralSettings.Settings["servicesATProdModeCertificatePassword"];
            return (pTestMode)
                ? "TESTEwebservice"
                : PluginSettings.PluginSoftwareVendor.GetAppSoftwareATWSProdModeCertificatePassword();
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
        //DocumentsWayBill(Agricultural)
        public static Uri ServicesATUriDocumentsWayBill { get { return GetServicesATWBUri(false, false); } }
        public static Uri ServicesATUriDocumentsWayBillSOAPAction = new Uri("https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/");

        public static Uri ServicesATUriDocuments { get { return GetServicesATDCUri(false); } }
        public static Uri ServicesATUriDocumentsSOAPAction = new Uri("http://servicos.portaldasfinancas.gov.pt/faturas/RegisterInvoice");

        //Constructor
        public ServicesAT(fin_documentfinancemaster pFinanceMaster)
        {
            //Init Settings Main Config Settings
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
            
            //Parameters
            _documentMaster = pFinanceMaster;
            //Prepare Local Vars : Must be WayBill and Not a InvoiceWayBill, InvoiceWayBill always are processed like normal Documents
            //Se a fatura for utilizada como documento de transporte e acompanhar os bens, terei que efetuar a comunicação à AT?  
            //https://www.portugal-a-programar.pt/forums/topic/57734-utilizar-webservices-da-at/?do=findComment&comment=598210
            //https://info.portaldasfinancas.gov.pt/infofaqs/listafaqs.aspx?subarea=263
            //Caso a fatura seja emitida por via eletrónica, (...) e contenha os elementos referidos no art 36º do CIVA, assim como todos os elementos que devam constar do documento de transporte, fica o remetente (proprietário dos bens) dispensado de comunicação à AT.
            _wayBillMode = (_documentMaster.DocumentType.WayBill);

            //Init WebService parameters and Files
            _pathSaveSoap = string.Format(@"{0}{1}", GeneralSettings.Path["temp"], "soapsend.xml");
            _pathSaveSoapResult = string.Format(@"{0}{1}", GeneralSettings.Path["temp"], "soapresult.xml");
            _pathSaveSoapResultError = string.Format(@"{0}{1}", GeneralSettings.Path["temp"], "soapresult_error.xml");
            _urlWebService = (!_wayBillMode) ? GetServicesATDCUri(false) : GetServicesATWBUri(testMode, false);
            _urlSoapAction = (!_wayBillMode) ? ServicesATUriDocumentsSOAPAction : ServicesATUriDocumentsWayBillSOAPAction;
            _pathPublicKey = GetServicesATFilePublicKey(testMode);
            _pathCertificate = GetServicesATFileCertificate(testMode);
            // Get TestMode/Production Configuration
            _atAccountFiscalNumber = GetServicesATAccountFiscalNumber(testMode);
            _atAccountPassword = GetServicesATAccountPassword(testMode);
            // Values from PrefsDB and VendorPlugin
            _atTaxRegistrationNumber = GetServicesATTaxRegistrationNumber(testMode);
            _atPasswordCertificate = GetServicesATCertificatePassword(testMode);

            //Override Default Paths: If Works in Service mode need FullPath to Files, and a user running service, to bypass windows service user
            if (!Environment.UserInteractive)
            {
                var appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                _pathPublicKey = string.Format(@"{0}\{1}", appPath, _pathPublicKey).Replace('/', '\\');
                _pathCertificate = string.Format(@"{0}\{1}", appPath, _pathCertificate).Replace('/', '\\'); ;
            }
            // Show Logs if in Console/Interactive Mode (Not in Service Mode)
            else
            {
                // Log Paths
                _logger.Debug(string.Format("Using pathPublicKey: [{0}]", _pathPublicKey));
                Utils.Log(string.Format("Using pathCertificate: [{0}]", _pathCertificate));
                // Log Parameters
                Utils.Log(string.Format("TaxRegistrationNumber :[{0}], AccountFiscalNumber: [{1}], AccountPassword: [{2}]", _atTaxRegistrationNumber, _atAccountFiscalNumber, _atAccountPassword));
            }

            if (File.Exists(_pathPublicKey) && File.Exists(_pathCertificate))
            {
                ValidCerificates = true;
            }
            else
            {
                // Commented, leave it for Next Error Message
                //if (!File.Exists(_pathPublicKey)) Utils.Log(String.Format("Error cant find file pathPublicKey: [{0}]!", _pathPublicKey));
                //if (!File.Exists(_pathCertificate)) Utils.Log(String.Format("Error cant find file pathCertificate: [{0}]!", _pathCertificate));
            }
        }

        //Sample : Documents
        private string GenerateXmlStringDCSample()
        {
            //Init StringBuilder
            StringBuilder sb = new StringBuilder();
            //Soap Header
            sb.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            sb.Append("  <S:Header>");
            sb.Append("    <wss:Security xmlns:wss=\"http://schemas.xmlsoap.org/ws/2002/12/secext/\">");
            sb.Append("    <wss:UsernameToken>");
            sb.Append("      <wss:Username>" + _atAccountFiscalNumber + "</wss:Username>");
            sb.Append("      <wss:Password>" + _accountPasswordEncrypted + "</wss:Password>");
            sb.Append("      <wss:Nonce>" + _symetricKeyEncrypted + "</wss:Nonce>");
            sb.Append("      <wss:Created>" + _dateOfCriationEncrypted + "</wss:Created>");
            sb.Append("    </wss:UsernameToken>");
            sb.Append("  </wss:Security>");
            sb.Append("</S:Header>");
            //Soap Body
            sb.Append("  <S:Body>");
            sb.Append("    <ns2:RegisterInvoiceElem xmlns:ns2=\"http://servicos.portaldasfinancas.gov.pt/faturas/\">");
            sb.Append("    <TaxRegistrationNumber>" + _atTaxRegistrationNumber + "</TaxRegistrationNumber>");
            sb.Append("    <ns2:InvoiceNo>" + _sampleDCInvoiceNo + "</ns2:InvoiceNo>");
            sb.Append("    <ns2:InvoiceDate>" + _sampleDCInvoiceDate + "</ns2:InvoiceDate>");
            sb.Append("    <ns2:InvoiceType>" + _sampleDCInvoiceType + "</ns2:InvoiceType>");
            sb.Append("    <ns2:InvoiceStatus>N</ns2:InvoiceStatus>");
            sb.Append("    <CustomerTaxID>" + _sampleXXCustomerTaxID + "</CustomerTaxID>");
            sb.Append("    <Line>");
            sb.Append("      <ns2:DebitAmount>100</ns2:DebitAmount>");
            sb.Append("      <ns2:Tax>");
            sb.Append("        <ns2:TaxType>IVA</ns2:TaxType>");
            sb.Append("        <ns2:TaxCountryRegion>PT</ns2:TaxCountryRegion>");
            sb.Append("        <ns2:TaxPercentage>23</ns2:TaxPercentage>");
            sb.Append("      </ns2:Tax>");
            sb.Append("    </Line>");
            sb.Append("      <DocumentTotals>");
            sb.Append("        <ns2:TaxPayable>23</ns2:TaxPayable>");
            sb.Append("        <ns2:NetTotal>100</ns2:NetTotal>");
            sb.Append("        <ns2:GrossTotal>123</ns2:GrossTotal>");
            sb.Append("      </DocumentTotals>");
            sb.Append("    </ns2:RegisterInvoiceElem>");
            sb.Append("  </S:Body>");
            sb.Append("</S:Envelope>");

            XmlDocument xc = new XmlDocument();
            TextReader textReader = new StringReader(sb.ToString());
            XDocument xmlDocument = XDocument.Load(textReader);

            //Save Soap
            xmlDocument.Save(_pathSaveSoap);

            return xmlDocument.ToString();
        }

        //Sample : WayBill
        private string GenerateXmlStringWBSample()
        {
            StringBuilder sb = new StringBuilder();

            //Soap Envelope
            sb.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            //Soap Header
            sb.Append("  <S:Header>");
            sb.Append("    <wss:Security xmlns:wss=\"http://schemas.xmlsoap.org/ws/2002/12/secext/\">");
            sb.Append("      <wss:UsernameToken>");
            sb.Append("      <wss:Username>" + _atAccountFiscalNumber + "</wss:Username>");
            sb.Append("      <wss:Password>" + _accountPasswordEncrypted + "</wss:Password>");
            sb.Append("      <wss:Nonce>" + _symetricKeyEncrypted + "</wss:Nonce>");
            sb.Append("      <wss:Created>" + _dateOfCriationEncrypted + "</wss:Created>");
            sb.Append("      </wss:UsernameToken>");
            sb.Append("    </wss:Security>");
            sb.Append("  </S:Header>");
            //Soap Body
            sb.Append("  <S:Body>");

            //BODY#01 Sample
            sb.Append("		<ns2:envioDocumentoTransporteRequestElem xmlns:ns2=\"https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/\">");
            sb.Append("       <TaxRegistrationNumber>" + _atTaxRegistrationNumber + "</TaxRegistrationNumber>");
            sb.Append("       <CompanyName>Carlos Mendes</CompanyName>");
            sb.Append("       <CompanyAddress>");
            //Change "AddressDetail" to "Addressdetail" else error: complex-type 2.4: in element CompanyAddress of type
            sb.Append("         <Addressdetail>Rua Alexandre Herculano 60</Addressdetail>");
            sb.Append("         <City>Soure</City>");
            sb.Append("         <PostalCode>3130-219</PostalCode>");
            sb.Append("         <Country>PT</Country>");
            sb.Append("       </CompanyAddress>");
            sb.Append("       <DocumentNumber>" + _sampleWBDocumentNumber + "</DocumentNumber>");
            sb.Append("       <MovementStatus>N</MovementStatus>");
            sb.Append("       <MovementDate>" + _sampleWBMovementDate + "</MovementDate>");
            sb.Append("       <MovementType>" + _sampleWBMovementType + "</MovementType>");
            sb.Append("       <CustomerTaxID>" + _sampleXXCustomerTaxID + "</CustomerTaxID>");
            sb.Append("       <CustomerName>Servidor</CustomerName>");
            sb.Append("       <CustomerAddress>");
            sb.Append("         <Addressdetail>Rua Alexandre Herculano 60</Addressdetail>");
            sb.Append("         <City>Soure</City>");
            sb.Append("         <PostalCode>3130-219</PostalCode>");
            sb.Append("         <Country>PT</Country>");
            sb.Append("       </CustomerAddress>");
            sb.Append("       <AddressTo>");
            sb.Append("         <Addressdetail>Rua Alexandre Herculano 60</Addressdetail>");
            sb.Append("         <City>Soure</City>");
            sb.Append("         <PostalCode>3130-219</PostalCode>");
            sb.Append("         <Country>PT</Country>");
            sb.Append("       </AddressTo>");
            sb.Append("       <AddressFrom>");
            sb.Append("         <Addressdetail>Rua Alexandre Herculano 60</Addressdetail>");
            sb.Append("         <City>Soure</City>");
            sb.Append("         <PostalCode>3130-219</PostalCode>");
            sb.Append("         <Country>PT</Country>");
            sb.Append("       </AddressFrom>");
            sb.Append("       <MovementEndTime>2013-04-09T23:26:59</MovementEndTime>");
            sb.Append("       <MovementStartTime>2013-04-09T23:25:59</MovementStartTime>");
            sb.Append("       <VehicleID>80-15-NF</VehicleID>");
            sb.Append("       <Line>");
            sb.Append("         <ProductDescription>Artigo para entregar</ProductDescription>");
            sb.Append("         <Quantity>1.00</Quantity>");
            sb.Append("         <UnitOfMeasure>UNI</UnitOfMeasure>");
            sb.Append("         <UnitPrice>0.00</UnitPrice>");
            sb.Append("       </Line>");
            sb.Append("		</ns2:envioDocumentoTransporteRequestElem>");
            //EO BODY#01 Sample

            //BO BODY#02 Sample
            //sb.Append("		<ns2:envioDocumentoTransporteRequestElem xmlns:ns2=\"https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/\">");
            //sb.Append("		<TaxRegistrationNumber>504512153</TaxRegistrationNumber>");
            //sb.Append("		<CompanyName>Elabora Software, Lda.</CompanyName>");
            //sb.Append("		<CompanyAddress>");
            ////Change "AddressDetail" to "Addressdetail" else error: complex-type 2.4: in element CompanyAddress of type
            //sb.Append("		    <Addressdetail>Rua dos Mourões, 1486</Addressdetail>");
            //sb.Append("		    <City>S. Félix Da Marinha</City>");
            //sb.Append("		    <PostalCode>4410-137</PostalCode>");
            //sb.Append("		    <Country>PT</Country>");
            //sb.Append("		</CompanyAddress>");
            //sb.Append("		<DocumentNumber>22 2013/1</DocumentNumber>");
            //sb.Append("		<MovementStatus>N</MovementStatus>");
            //sb.Append("		<MovementDate>2012-12-31</MovementDate>");
            //sb.Append("		<MovementType>GT</MovementType>");
            //sb.Append("		<CustomerTaxID>504512153</CustomerTaxID>");
            //sb.Append("		<CustomerName>Elabora Software, Lda.</CustomerName>");
            //sb.Append("		<CustomerAddress>");
            //sb.Append("		    <Addressdetail>Rua da Igreja Velha, 5</Addressdetail>");
            //sb.Append("		    <City>S. Félix Da Marinha</City>");
            //sb.Append("		    <PostalCode>4410-000</PostalCode>");
            //sb.Append("		    <Country>PT</Country>");
            //sb.Append("		</CustomerAddress>");
            //sb.Append("		<AddressTo>");
            //sb.Append("		    <Addressdetail>Rua da Igreja Velha, 5</Addressdetail>");
            //sb.Append("		    <City>S. Félix Da Marinha</City>");
            //sb.Append("		    <PostalCode>4410-137</PostalCode>");
            //sb.Append("		    <Country>PT</Country>");
            //sb.Append("		</AddressTo>");
            //sb.Append("		<AddressFrom>");
            //sb.Append("		    <Addressdetail>Rua dos Mourões, 1486</Addressdetail>");
            //sb.Append("		    <City>S. Félix Da Marinha</City>");
            //sb.Append("		    <PostalCode>4410-137</PostalCode>");
            //sb.Append("		    <Country>PT</Country>");
            //sb.Append("		</AddressFrom>");
            //sb.Append("		<MovementEndTime>2012-12-31T23:25:59</MovementEndTime>");
            //sb.Append("		<MovementStartTime>2012-12-31T23:25:59</MovementStartTime>");
            //sb.Append("		<VehicleID>80-15-NF</VehicleID>");
            //sb.Append("		<Line>");
            //sb.Append("		    <OrderReferences>20 2013/1</OrderReferences>");
            //sb.Append("		    <ProductDescription>Artigo para entregar</ProductDescription>");
            //sb.Append("		    <Quantity>1.00</Quantity>");
            //sb.Append("		    <UnitOfMeasure>UNI</UnitOfMeasure>");
            //sb.Append("		    <UnitPrice>0.00</UnitPrice>");
            //sb.Append("		</Line>");
            //sb.Append("		</ns2:envioDocumentoTransporteRequestElem>");
            //EO BODY#02 Sample

            sb.Append("  </S:Body>");
            sb.Append("</S:Envelope>");

            XmlDocument xc = new XmlDocument();
            TextReader textReader = new StringReader(sb.ToString());
            XDocument xmlDocument = XDocument.Load(textReader);

            //Save Soap
            xmlDocument.Save(_pathSaveSoap);

            return xmlDocument.ToString();
        }

        private string GenerateXmlStringDC()
        {
            _logger.Debug($"string ServiceAT.GenerateXmlStringDC() :: {_documentMaster.DocumentNumber}");

            /* IN009150 (IN009075) */
            string entityFiscalNumber = "";
            if (!string.IsNullOrEmpty(_documentMaster.EntityFiscalNumber)) { entityFiscalNumber = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityFiscalNumber); }
            /* IN009150 - end */

            //Init Local Vars
            string customerTaxID = FiscalNumber.ExtractFiscalNumber(entityFiscalNumber);

            //Test mode, increase DocumentNumber only when Develop
            if (_increaseDocumentNumber) _documentMaster.IncreaseDocumentNumber();

            string sbContentCustomerTax;
            //Diferent sbContentCustomerTax if OutSide Portugal
            if (_documentMaster.EntityCountryOid.Equals(SharedSettings.XpoOidConfigurationCountryPortugal))
            {
                sbContentCustomerTax = string.Format("    <CustomerTaxID>{0}</CustomerTaxID>", customerTaxID);
            }
            else
            {
                sbContentCustomerTax = string.Format(@"    <ns2:InternationalCustomerTaxID>
              <TaxIDNumber>{0}</TaxIDNumber>
              <TaxIDCountry>{1}</TaxIDCountry>
            </ns2:InternationalCustomerTaxID>"
                    , customerTaxID
                    , _documentMaster.EntityCountry
                );
            }

            //Get Lines Content
            string sbContentLinesAndDocumentTotals = Utils.GetDocumentContentLinesAndDocumentTotals(_documentMaster);

            //Init StringBuilder
            StringBuilder sb = new StringBuilder();
            //Soap Header
            sb.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            sb.Append("  <S:Header>");
            sb.Append("    <wss:Security xmlns:wss=\"http://schemas.xmlsoap.org/ws/2002/12/secext/\">");
            sb.Append("    <wss:UsernameToken>");
            sb.Append("      <wss:Username>" + _atAccountFiscalNumber + "</wss:Username>");
            sb.Append("      <wss:Password>" + _accountPasswordEncrypted + "</wss:Password>");
            sb.Append("      <wss:Nonce>" + _symetricKeyEncrypted + "</wss:Nonce>");
            sb.Append("      <wss:Created>" + _dateOfCriationEncrypted + "</wss:Created>");
            sb.Append("    </wss:UsernameToken>");
            sb.Append("  </wss:Security>");
            sb.Append("</S:Header>");
            //Soap Body
            sb.Append("  <S:Body>");
            sb.Append("    <ns2:RegisterInvoiceElem xmlns:ns2=\"http://servicos.portaldasfinancas.gov.pt/faturas/\">");
            sb.Append("    <TaxRegistrationNumber>" + _atTaxRegistrationNumber + "</TaxRegistrationNumber>");
            sb.Append("    <ns2:InvoiceNo>" + _documentMaster.DocumentNumber + "</ns2:InvoiceNo>");
            sb.Append("    <ns2:InvoiceDate>" + _documentMaster.DocumentDate + "</ns2:InvoiceDate>");
            sb.Append("    <ns2:InvoiceType>" + _documentMaster.DocumentType.Acronym + "</ns2:InvoiceType>");
            sb.Append("    <ns2:InvoiceStatus>" + _documentMaster.DocumentStatusStatus + "</ns2:InvoiceStatus>");
            //Generated Content
            sb.Append(sbContentCustomerTax);
            sb.Append(sbContentLinesAndDocumentTotals);
            sb.Append("    </ns2:RegisterInvoiceElem>");
            sb.Append("  </S:Body>");
            sb.Append("</S:Envelope>");

            TextReader textReader = new StringReader(sb.ToString());
            XDocument xmlDocument = XDocument.Load(textReader);

            //Save Soap
            xmlDocument.Save(_pathSaveSoap);

            return xmlDocument.ToString();
        }

        private string GenerateXmlStringWB()
        {
            /* IN007016 - escaping the following list of chars using "System.Security.SecurityElement.Escape()" method:
             * 
             *      "   &quot;
             *      '   &apos;
             *      <   &lt;
             *      >   &gt;
             *      &   &amp;
             */
            _logger.Debug($"string ServicesAT.GenerateXmlStringWB() :: {_documentMaster.DocumentNumber}");
			//IN009347 Documentos PT - Alteração do Layout dos dados do Cliente #Lindote 2020
            /* IN009150 (IN009075) - Decrypt phase */
            string entityName           = "";
            string entityAddress        = "";
            string entityZipCode        = "";
            string entityCity           = "";
            // string entityCountry        = "";
            string entityFiscalNumber   = "";

            if (!string.IsNullOrEmpty(_documentMaster.EntityName))          { entityName = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityName); }
            if (!string.IsNullOrEmpty(_documentMaster.EntityAddress))       { entityAddress = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityAddress); }
            if (!string.IsNullOrEmpty(_documentMaster.EntityZipCode))       { entityZipCode = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityZipCode); }
            if (!string.IsNullOrEmpty(_documentMaster.EntityCity))          { entityCity = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityCity); }
            if (!string.IsNullOrEmpty(_documentMaster.EntityLocality))      { entityCity = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityLocality); }
            // if (!string.IsNullOrEmpty(_documentMaster.EntityCountry))       { entityCountry = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityCountry); }
            if (!string.IsNullOrEmpty(_documentMaster.EntityFiscalNumber))  { entityFiscalNumber = PluginSettings.PluginSoftwareVendor.Decrypt(_documentMaster.EntityFiscalNumber); }
            /* IN009150 - end */

            //Init Local Vars
            string customerTaxID = FiscalNumber.ExtractFiscalNumber(entityFiscalNumber);
            //MovementStartTime equal/greater system DateTime to prevent -100 error (Override _documentMaster.MovementStartTime)
            _movementStartTime = DateTime.Now;

            //Test mode, increase DocumentNumber only when Develop
            if (_increaseDocumentNumber) _documentMaster.IncreaseDocumentNumber();

            //Get Lines Content
            string sbContentLines = Utils.GetDocumentWayBillContentLines(_documentMaster);

            StringBuilder sb = new StringBuilder();
            //Soap Envelope
            sb.Append("<S:Envelope xmlns:S=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            //Soap Header
            sb.Append("  <S:Header>");
            sb.Append("    <wss:Security xmlns:wss=\"http://schemas.xmlsoap.org/ws/2002/12/secext/\">");
            sb.Append("      <wss:UsernameToken>");
            sb.Append("      <wss:Username>" + _atAccountFiscalNumber + "</wss:Username>");
            sb.Append("      <wss:Password>" + _accountPasswordEncrypted + "</wss:Password>");
            sb.Append("      <wss:Nonce>" + _symetricKeyEncrypted + "</wss:Nonce>");
            sb.Append("      <wss:Created>" + _dateOfCriationEncrypted + "</wss:Created>");
            sb.Append("      </wss:UsernameToken>");
            sb.Append("    </wss:Security>");
            sb.Append("  </S:Header>");
            //Soap Body
            sb.Append("  <S:Body>");
            sb.Append("		<ns2:envioDocumentoTransporteRequestElem xmlns:ns2=\"https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/\">");
            sb.Append("       <TaxRegistrationNumber>" + _atTaxRegistrationNumber + "</TaxRegistrationNumber>");
            sb.Append("       <CompanyName>" + SecurityElement.Escape(entityName) + "</CompanyName>");
            sb.Append("       <CompanyAddress>");
            //Change "AddressDetail" to "Addressdetail" else error: complex-type 2.4: in element CompanyAddress of type
            //Only sent is not empty Addressdetail,City,PostalCode,Country
            sb.Append("         <Addressdetail>" + SecurityElement.Escape(entityAddress) + "</Addressdetail>");
            sb.Append("         <City>" + entityCity + "</City>");
            sb.Append("         <PostalCode>" + entityZipCode + "</PostalCode>");
            sb.Append("         <Country>" + _documentMaster.EntityCountry + "</Country>");
            sb.Append("       </CompanyAddress>");
            sb.Append("       <DocumentNumber>" + _documentMaster.DocumentNumber + "</DocumentNumber>");
            //WIP: CancellWayBills : Find "//WIP: CancellWayBills : " to continue in Uncomment in PosDocumentFinanceSelectRecordDialog.cs
            //Used to Cancel Documents, ex After we have it in DB, resulted from a Sent WB
            //Currently disabled Problems :"Não pode ser alterado um Documento de Transporte quando a Data de Início já decorreu."
            if (!string.IsNullOrEmpty(_documentMaster.ATDocCodeID))
            {
                /* IN009083 - only Transportation Documents  have "ATDocCodeID" when successfully sent to AT */
                sb.Append("       <ATDocCodeID>" + _documentMaster.ATDocCodeID + "</ATDocCodeID>");
            }
            sb.Append("       <MovementStatus>" + _documentMaster.DocumentStatusStatus + "</MovementStatus>");
            sb.Append("       <MovementDate>" + _documentMaster.DocumentDate + "</MovementDate>");
            sb.Append("       <MovementType>" + _documentMaster.DocumentType.Acronym + "</MovementType>");
            sb.Append("       <CustomerTaxID>" + customerTaxID + "</CustomerTaxID>");
            sb.Append("       <CustomerAddress>");
            //Only sent is not empty Addressdetail,City,PostalCode,Country
            sb.Append("         <Addressdetail>" + SecurityElement.Escape(entityAddress) + "</Addressdetail>");
            sb.Append("         <City>" + entityCity + "</City>");
            sb.Append("         <PostalCode>" + entityZipCode + "</PostalCode>");
            sb.Append("         <Country>" + _documentMaster.EntityCountry + "</Country>");
            sb.Append("       </CustomerAddress>");
            //Required to be after <CustomerAddress>
            sb.Append("       <CustomerName>" + SecurityElement.Escape(entityName) + "</CustomerName>");
            sb.Append("       <AddressTo>");
            sb.Append("         <Addressdetail>" + SecurityElement.Escape(_documentMaster.ShipToAddressDetail) + "</Addressdetail>");
            sb.Append("         <City>" + _documentMaster.ShipToCity + "</City>");
            sb.Append("         <PostalCode>" + _documentMaster.ShipToPostalCode + "</PostalCode>");
            sb.Append("         <Country>" + _documentMaster.ShipToCountry + "</Country>");
            sb.Append("       </AddressTo>");
            sb.Append("       <AddressFrom>");
            sb.Append("         <Addressdetail>" + SecurityElement.Escape(_documentMaster.ShipFromAddressDetail) + "</Addressdetail>");
            sb.Append("         <City>" + _documentMaster.ShipFromCity + "</City>");
            sb.Append("         <PostalCode>" + _documentMaster.ShipFromPostalCode + "</PostalCode>");
            sb.Append("         <Country>" + _documentMaster.ShipFromCountry + "</Country>");
            sb.Append("       </AddressFrom>");
            //Dont Sent MovementEndTime, not required
            //sb.Append("       <MovementEndTime>" + _documentMaster.MovementEndTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime) + "</MovementEndTime>");
            sb.Append("       <MovementStartTime>" + _movementStartTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime) + "</MovementStartTime>");
            //VehicleID
            if (!string.IsNullOrEmpty(_documentMaster.ShipFromDeliveryID)) sb.Append("       <VehicleID>" + _documentMaster.ShipFromDeliveryID + "</VehicleID>");
            //Line
            //Generated Content
            sb.Append(sbContentLines);
            sb.Append("		</ns2:envioDocumentoTransporteRequestElem>");
            sb.Append("  </S:Body>");
            sb.Append("</S:Envelope>");

            TextReader textReader = new StringReader(sb.ToString());
            XDocument xmlDocument = XDocument.Load(textReader);

            //Save Soap
            xmlDocument.Save(_pathSaveSoap);

            return xmlDocument.ToString();
        }

        private void buildCredentials()
        {
            //Informamos que na cifra da password deverá continuar a utilizar o Certificado Digital já enviado para testes "ChavePublicaAT.cer"
            X509Certificate2 certCP = new X509Certificate2();
            //certCP.Import(CaminhoCertificado, SenhaCertificado, X509KeyStorageFlags.DefaultKeySet);
            certCP.Import(_pathPublicKey);

            string publicKey = certCP.PublicKey.Key.ToXmlString(false);

            string stCreationDate = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.ff") + "Z";

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.GenerateKey();
            rijndaelCipher.Mode = CipherMode.ECB;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            string simetricKey = rijndaelCipher.Key.ToString();
            byte[] simetricKeyByte = rijndaelCipher.Key;
            SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
            rijn.Key = rijndaelCipher.IV;
            rijn.IV = rijndaelCipher.IV;
            rijn.Mode = CipherMode.ECB;
            MemoryStream msPasswordAT = new MemoryStream();
            CryptoStream csPasswordAT = new CryptoStream(msPasswordAT, rijn.CreateEncryptor(rijn.Key, rijn.IV), CryptoStreamMode.Write);
            using (StreamWriter swPasswordAT = new StreamWriter(csPasswordAT))
            {
                swPasswordAT.Write(_atAccountPassword);
            }
            MemoryStream msCreationDate = new MemoryStream();
            CryptoStream csCreationDate = new CryptoStream(msCreationDate, rijn.CreateEncryptor(rijn.Key, rijn.IV), CryptoStreamMode.Write);
            using (StreamWriter swCreationDate = new StreamWriter(csCreationDate))
            {
                swCreationDate.Write(stCreationDate);
            }
            _accountPasswordEncrypted = Convert.ToBase64String(msPasswordAT.ToArray());
            _dateOfCriationEncrypted = Convert.ToBase64String(msCreationDate.ToArray());
            RSACryptoServiceProvider AlgRSA = new RSACryptoServiceProvider();
            AlgRSA.FromXmlString(publicKey);
            byte[] Chave = AlgRSA.Encrypt(rijn.Key, false);
            _symetricKeyEncrypted = Convert.ToBase64String(Chave);
        }

        public string Send()
        {
            Utils.Log(string.Format("urlWebService: {0}", _urlWebService));
            Utils.Log(string.Format("urlSoapAction: {0}", _urlSoapAction));
            Utils.Log(string.Format("Send Document DocumentNumber: [{0}]/WayBillMode: [{1}]", _documentMaster.DocumentNumber, _wayBillMode));

            //Check Certificates
            if (!ValidCerificates)
            {
                string msg = string.Format("Invalid Certificates: [{0}], [{1}] in current Directory [{2}]", _pathPublicKey, _pathCertificate, Directory.GetCurrentDirectory());
                Utils.Log(msg);
                return string.Format(msg);
            }

            //Fix for Error 99
            //Neste momento o erro internal error ainda está a ocorrer? Fiquei sem perceber se o erro deriva de alguma atualização do windows ou problema da AT.
            //Pode ser resolvido, em c#, com:
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            buildCredentials();

            try
            {
                //Prepare Request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlWebService);
                request.Headers.Add("SOAPAction", _urlSoapAction.ToString());
                if (Convert.ToInt32(ConfigurationManager.AppSettings["servicesATRequestTimeout"]) > 0) request.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["servicesATRequestTimeout"]);

                // Old Method : without Using VendorPlugin
                //X509Certificate2 cert = new X509Certificate2();
                //From user installed Certificates
                //cert.Import(_pathCertificate, _passwordCertificate, X509KeyStorageFlags.DefaultKeySet);
                //From FileSystem "Resources\Certificates"
                //cert.Import(_pathCertificate, _atPasswordCertificate, X509KeyStorageFlags.Exportable);

                // New Method : Import Certificate From VendorPlugin
                X509Certificate2 cert = PluginSettings.PluginSoftwareVendor.ImportCertificate(testMode, _pathCertificate);

                // Output Certificate 
                Utils.Log(string.Format("Cert Subject: [{0}], NotBefore: [{1}], NotAfter: [{2}]", cert.Subject, cert.NotBefore, cert.NotAfter));

                request.ClientCertificates.Add(cert);

                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                request.Accept = "text/xml";
                string oRequest = (!_wayBillMode)
                    ? (_useMockSampleData) ? GenerateXmlStringDCSample() : GenerateXmlStringDC()
                    : (_useMockSampleData) ? GenerateXmlStringWBSample() : GenerateXmlStringWB()
                ;

                _postData = oRequest;
                byte[] byteArray = Encoding.UTF8.GetBytes(_postData);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                //Save Result to File
                File.WriteAllText(_pathSaveSoapResult, responseFromServer);

                //GetSoapResult
                _soapResult = (!_wayBillMode) ? GetSoapResultDC() : GetSoapResultWB();

                //Log
                Utils.Log(string.Format("Send Document ReturnCode: [{0}]", _soapResult.ReturnCode));
                Utils.Log(string.Format("Send Document ReturnMessage: [{0}]", _soapResult.ReturnMessage), true);

                //Persist Result in Database
                PersistResult(_soapResult);

                return responseFromServer;
            }
            catch (WebException ex)
            {
                _logger.Error(ex.Message, ex);

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse response = ex.Response;
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());
                    //Store Result to use 2 times, else second time is string.Empty
                    string streamResult = streamReader.ReadToEnd();
                    //Save Result Error to File
                    File.WriteAllText(_pathSaveSoapResultError, streamResult);
                    //Log
                    Utils.Log(string.Format("Send ProtocolError StreamResult: [{0}]", streamResult), true);
                    return streamResult;
                }
                else
                {
                    //Log
                    Utils.Log(string.Format("Exception: [{0}]", ex.Message), true);
                    return ex.Message;
                }
            }
        }

        //Private, outside sendDocument.SoapResult Member
        private ServicesATSoapResult GetSoapResultDC()
        {
            ServicesATSoapResult result = new ServicesATSoapResult();

            if (File.Exists(_pathSaveSoapResult))
            {
                result.ReturnRaw = File.ReadAllText(_pathSaveSoapResult);

                XElement doc = XElement.Load(_pathSaveSoapResult);
                //Envelop Namespace S
                XNamespace xnS = "http://schemas.xmlsoap.org/soap/envelope/";
                //RegisterInvoiceResponseElem Namespace
                XNamespace xnRI = "http://servicos.portaldasfinancas.gov.pt/faturas/";

                foreach (var itm in doc.Descendants(xnS + "Body").Descendants(xnRI + "RegisterInvoiceResponseElem"))
                {
                    //<ns2:RegisterInvoiceResponseElem>
                    result.ReturnCode = itm.Element("ReturnCode").Value;
                    result.ReturnMessage = itm.Element("ReturnMessage").Value;
                    //</ns2:RegisterInvoiceResponseElem>
                }
            }

            return result;
        }

        //Private, outside sendDocument.SoapResult Member
        private ServicesATSoapResult GetSoapResultWB()
        {
            ServicesATSoapResult result = new ServicesATSoapResult();

            if (File.Exists(_pathSaveSoapResult))
            {
                result.ReturnRaw = File.ReadAllText(_pathSaveSoapResult);

                XElement doc = XElement.Load(_pathSaveSoapResult);
                //Envelop Namespace S
                XNamespace xnS = "http://schemas.xmlsoap.org/soap/envelope/";
                //RegisterInvoiceResponseElem Namespace
                XNamespace xnRI = "https://servicos.portaldasfinancas.gov.pt/sgdtws/documentosTransporte/";

                foreach (var itm in doc.Descendants(xnS + "Body").Descendants(xnRI + "envioDocumentoTransporteResponseElem"))
                {
                    //<ns0:envioDocumentoTransporteResponseElem
                    //<ResponseStatus>
                    result.ReturnCode = itm.Element("ResponseStatus").Element("ReturnCode").Value;
                    result.ReturnMessage = itm.Element("ResponseStatus").Element("ReturnMessage").Value;
                    //<//ResponseStatus>
                    if (result.ReturnCode == "0")
                    {
                        result.DocumentNumber = itm.Element("DocumentNumber").Value;
                        result.ATDocCodeID = itm.Element("ATDocCodeID").Value;
                    }
                    //</ns0:envioDocumentoTransporteResponseElem
                }
            }

            return result;
        }

        /// <summary>
        /// Persist SOAP Result in DataBase
        /// </summary>
        /// <param name="SoapResult"></param>
        public bool PersistResult(ServicesATSoapResult pSoapResult)
        {
            bool result = false;

            try
            {
                fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), _documentMaster.Oid);
                //Always Add to sys_systemauditat Log
                SystemAuditATWSType systemAuditATWSType = (!_wayBillMode) ? SystemAuditATWSType.Document : SystemAuditATWSType.DocumentWayBill;
                var systemAuditATWS = new sys_systemauditat(XPOSettings.Session)
                {
                    Date = DateTime.Now,
                    Type = systemAuditATWSType,
                    PostData = _postData,
                    ReturnCode = Convert.ToInt16(pSoapResult.ReturnCode),
                    ReturnMessage = pSoapResult.ReturnMessage,
                    ReturnRaw = pSoapResult.ReturnRaw,
                    DocumentNumber = documentMaster.DocumentNumber,
                    ATDocCodeID = pSoapResult.ATDocCodeID,
                    DocumentMaster = documentMaster
                };
                systemAuditATWS.Save();

                //Always Add to fin_documentfinancemaster ATAudit
                documentMaster.ATAudit.Add(systemAuditATWS);
                documentMaster.Save();

                //Assign OK Result
                if (pSoapResult.ReturnCode == "0")
                {
                    //Assign to ATValidResult to Document Master
                    documentMaster.ATValidAuditResult = systemAuditATWS;
                    //Assign ATDocCodeID
                    if (_wayBillMode) documentMaster.ATDocCodeID = pSoapResult.ATDocCodeID;
                    //Disable Resend if Enabled (Used when document is Cancelled)
                    if (documentMaster.ATResendDocument) documentMaster.ATResendDocument = false;
                    //Override Default Document Date
                    documentMaster.MovementStartTime = _movementStartTime;
                    documentMaster.Save();

                    //Assign True to Result 
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
