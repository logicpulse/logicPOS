using DevExpress.Xpo.DB;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;
using System.Text;
using System.Xml;

//Notes Tests in DocumentFinanceDialogPage7

namespace LogicPOS.Finance.Saft
{
    //Notes: SourceID and CustomerID required CodeInternal (30 Chars Key), View Customer and UserDetail XPOObject/CodeInternal Notes
    //Notes: IN SAF-T PT the TotalFinal correspond to GrossTotal (Total do documento com impostos (GrossTotal))

    public class SaftPt
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private static XmlWriter _xmlWriter;
        private static DateTime _currentDate;

        //Filter DocumentFinanceMaster
        private static DateTime _documentDateStart;
        private static DateTime _documentDateEnd;

        //Settings
        private static readonly string _dateTimeFormatDocumentDate = CultureSettings.DateTimeFormatDocumentDate;
        //Custom Number Format
        private static readonly string _decimalTaxFormat = CultureSettings.DecimalFormatGrossTotalSAFTPT;
        private static readonly string _decimalFormat = CultureSettings.DecimalFormatSAFTPT;
        private static readonly string _decimalFormatTotals = CultureSettings.DecimalFormatGrossTotalSAFTPT;
        //Default Customer
        private static readonly erp_customer _defaultCustomer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), InvoiceSettings.FinalConsumerId);
        //Default Currency
        private static readonly cfg_configurationcurrency _defaultCurrency = XPOSettings.ConfigurationSystemCurrency;

        public static string ExportSaftPt()
        {
            //DEVELOPER : Assign pastMonths=0 to Work in Curent Month Range, Else it Works in Past Month by Default (-1)
            int pastMonths = 0;

            //TODO: Move to Filter Date Dialog
            DateTime workingDate = XPOUtility.CurrentDateTimeAtomic().AddMonths(-pastMonths);
            DateTime firstDayOfMonth = new DateTime(workingDate.Year, workingDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime dateTimeStart = firstDayOfMonth;
            DateTime dateTimeEnd = lastDayOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59);

            return ExportSaftPt(dateTimeStart, dateTimeEnd);
        }

        public static string ExportSaftPt(DateTime pDateTimeStart, DateTime pDateTimeEnd)
        {
            //Parameters
            _documentDateStart = pDateTimeStart;
            _documentDateEnd = pDateTimeEnd;

            _currentDate = XPOUtility.CurrentDateTimeAtomic();

            //Settings
            string fileSaftPT = CultureSettings.FileFormatSaftPT;
            string dateTimeFileFormat = CultureSettings.FileFormatDateTime;
            string dateTime = XPOUtility.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
            string fileName = PathsSettings.Paths["saftpt"] + string.Format(fileSaftPT, SaftSettings.SaftVersionPrefix, SaftSettings.SaftVersion, dateTime).ToLower();
            if (PathsUtils.HasWritePermissionOnPath(PathsSettings.Paths["saftpt"].ToString()))
            {
                fileName = string.Format("\\temp\\" + fileSaftPT, SaftSettings.SaftVersionPrefix, SaftSettings.SaftVersion, dateTime).ToLower();
            }

            //XmlWriterSettings
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            //Required SAF-T Encoding Windows-1252
            xmlWriterSettings.Encoding = Encoding.GetEncoding(1252);
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.IndentChars = "  ";// \t
            xmlWriterSettings.NewLineChars = "\r\n";

            try
            {
                using (_xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
                {
                    //<Document>
                    _xmlWriter.WriteStartDocument();

                    //<AuditFile>
                    string standardAuditFileTax = string.Format("{0}_{1}", SaftSettings.SaftVersionPrefix, SaftSettings.SaftVersion);
                    _xmlWriter.WriteStartElement("AuditFile", string.Format("urn:OECD:StandardAuditFile-Tax:{0}", standardAuditFileTax));
                    _xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    _xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

                    Header();
                    MasterFiles();
                    //GeneralLedgerEntries();
                    SourceDocuments();

                    //</AuditFile>
                    _xmlWriter.WriteEndElement();

                    //</Document>
                    _xmlWriter.WriteEndDocument();
                }

                //Audit
                XPOUtility.Audit("EXPORT_SAF-T", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_export_saft"), fileName, _documentDateStart.ToString(CultureSettings.DateFormat), _documentDateEnd.ToString(CultureSettings.DateFormat)));

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw new Exception("ERROR_EXPORTING_SAFT", ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<Header>

        private static void Header()
        {
            try
            {
                //Comment
                //_xmlWriter.WriteComment(string.Format("Period: {0} to {1}", _documentDateStart.ToShortDateString(), _documentDateEnd.ToShortDateString()));

                //<Header>
                _xmlWriter.WriteStartElement("Header");
                WriteElement("AuditFileVersion", SaftSettings.SaftVersion);
                //Deprecated now We use NIF
                //string companyID = string.Format("{0} {1}"
                //    ,LogicPOS.Settings.AppSettings.PreferenceParameters["COMPANY_CIVIL_REGISTRATION"].Replace(' ', '_')
                //    ,LogicPOS.Settings.AppSettings.PreferenceParameters["COMPANY_CIVIL_REGISTRATION_ID"].Replace(' ', '_')
                //);
                string companyID = GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"];
                WriteElement("CompanyID", companyID);
                WriteElement("TaxRegistrationNumber", GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"]);
                WriteElement("TaxAccountingBasis", SaftSettings.TaxAccountingBasis);
                WriteElement("CompanyName", GeneralSettings.PreferenceParameters["COMPANY_NAME"]);
                WriteElement("BusinessName", GeneralSettings.PreferenceParameters["COMPANY_BUSINESS_NAME"]);

                //<CompanyAddress>
                _xmlWriter.WriteStartElement("CompanyAddress");
                WriteElement("AddressDetail", GeneralSettings.PreferenceParameters["COMPANY_ADDRESS"]);
                WriteElement("City", GeneralSettings.PreferenceParameters["COMPANY_CITY"]);
                WriteElement("PostalCode", GeneralSettings.PreferenceParameters["COMPANY_POSTALCODE"]);
                WriteElement("Region", GeneralSettings.PreferenceParameters["COMPANY_REGION"]);
                WriteElement("Country", GeneralSettings.PreferenceParameters["COMPANY_COUNTRY_CODE2"]);
                _xmlWriter.WriteEndElement();
                //</CompanyAddress>

                WriteElement("FiscalYear", _currentDate.Year);
                WriteElement("StartDate", _documentDateStart.ToString(_dateTimeFormatDocumentDate));
                WriteElement("EndDate", _documentDateEnd.ToString(_dateTimeFormatDocumentDate));
                WriteElement("CurrencyCode", CultureSettings.SaftCurrencyCode);
                WriteElement("DateCreated", _currentDate.ToString(_dateTimeFormatDocumentDate));
                WriteElement("TaxEntity", GeneralSettings.PreferenceParameters["COMPANY_TAX_ENTITY"]);
                WriteElement("ProductCompanyTaxID", SaftSettings.SaftProductCompanyTaxID);
                WriteElement("SoftwareCertificateNumber", SaftSettings.SaftSoftwareCertificateNumber);
                WriteElement("ProductID", SaftSettings.SaftProductID);
                WriteElement("ProductVersion", GeneralSettings.ProductVersion);
                WriteElement("Telephone", GeneralSettings.PreferenceParameters["COMPANY_TELEPHONE"]);
                WriteElement("Fax", GeneralSettings.PreferenceParameters["COMPANY_FAX"]);
                WriteElement("Email", GeneralSettings.PreferenceParameters["COMPANY_EMAIL"]);
                WriteElement("Website", GeneralSettings.PreferenceParameters["COMPANY_WEBSITE"]);

                //</Header>
                _xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles>

        private static void MasterFiles()
        {
            try
            {
                //<MasterFiles>
                _xmlWriter.WriteStartElement("MasterFiles");
                //MasterFiles_GeneralLedger();
                MasterFiles_Customer();
                //MasterFiles_Supplier();
                MasterFiles_Product();
                MasterFiles_TaxTable();
                //</MasterFiles>
                _xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments>

        private static void SourceDocuments()
        {
            try
            {
                //<SourceDocuments>
                _xmlWriter.WriteStartElement("SourceDocuments");
                SourceDocuments_SalesInvoices();
                SourceDocuments_MovementOfGoods();
                SourceDocuments_WorkingDocuments();
                SourceDocuments_Payments();
                _xmlWriter.WriteEndElement();
                //</SourceDocuments>
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><Customer>

        private static void MasterFiles_Customer()
        {
            //<Customer>
            //  <CustomerID>CB1</CustomerID>
            //  <AccountID>21110001</AccountID>
            //  <CustomerTaxID>508650704</CustomerTaxID>
            //  <CompanyName>Gomes &amp; Gomes, Lda.</CompanyName>
            //  <Contact>Sra. Maria Arminda Soares</Contact>
            //  <BillingAddress>
            //    <AddressDetail>Av. Liberdade, 435, R/C</AddressDetail>
            //    <City>Expo 98</City>
            //    <PostalCode>1050-007</PostalCode>
            //    <Country>PT</Country>
            //  </BillingAddress>
            //  <ShipToAddress>
            //    <AddressDetail>Rua Férra</AddressDetail>
            //    <City>Azambuja</City>
            //    <PostalCode>2050-010</PostalCode>
            //    <Country>PT</Country>
            //  </ShipToAddress>
            //  <Telephone>229725566</Telephone>
            //  <Fax>229725568</Fax>
            //  <Email>gomes_gomes@sapo.pt</Email>
            //  <Website>www.GomesAndGomes.pt</Website>
            //  <SelfBillingIndicator>0</SelfBillingIndicator>
            //</Customer>

            try
            {
                string sql = string.Format(@"
                    SELECT 
                        cu.CodeInternal AS CustomerID,
			            '' AS AccountID,
			            cu.FiscalNumber AS CustomerTaxID,
			            cu.Name AS CompanyName,
			            cu.Address AS AddressDetail,
			            cu.City AS City,
			            cu.ZipCode AS PostalCode,
			            cc.Code2 AS Country,
			            cu.Phone AS Telephone,
			            cu.Fax AS Fax,
			            cu.Email AS Email,
			            cu.Website AS Website
                    FROM 
                        fin_documentfinancepayment as fp
	                    left join erp_customer cu ON (fp.EntityOid = cu.Oid)
	                    left join cfg_configurationcountry cc ON (cu.Country = cc.Oid)
                    WHERE 
                        cu.Oid IS NOT NULL 
                        AND (fp.DocumentDate >= '{0}' AND fp.DocumentDate <= '{1}')

                    UNION

                    SELECT 
                        cu.CodeInternal AS CustomerID,
			            '' AS AccountID,
			            cu.FiscalNumber AS CustomerTaxID,
			            cu.Name AS CompanyName,
			            cu.Address AS AddressDetail,
			            cu.City AS City,
			            cu.ZipCode AS PostalCode,
			            cc.Code2 AS Country,
			            cu.Phone AS Telephone,
			            cu.Fax AS Fax,
			            cu.Email AS Email,
			            cu.Website AS Website
                    FROM 
                        fin_documentfinancemaster as fm
	                    left join erp_customer cu ON (fm.EntityOid = cu.Oid)
	                    left join cfg_configurationcountry cc ON (cu.Country = cc.Oid)
                    WHERE 
                        cu.Oid IS NOT NULL 
                        AND (fm.Date >= '{0}' AND fm.Date <= '{1}')
                        
                    GROUP BY 
	                    cu.CodeInternal, cu.FiscalNumber, cu.Name, cu.Address, cu.City, cu.ZipCode, cc.Code2, cu.Phone, cu.Fax, cu.Email, cu.Website
   
                    ;"
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                );
                //_logger.Debug(string.Format("sql: [{0}]", sql));

                //Used to Add Default Customer if not in Query, Required to Always have a Default Customer for ex to Documents that Donta Have a Customer (NULL), like Conference Documents, etc
                MasterFiles_Customer_DefaultCustomer();

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    //<Customer>
                    _xmlWriter.WriteStartElement("Customer");
                    if (row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")] != null)
                    {
                        WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")]);
                    }
                    else
                    {
                        WriteElement("CustomerID", _defaultCustomer.CodeInternal);
                    }
                    WriteElement("AccountID", row.Values[xPSelectData.GetFieldIndexFromName("AccountID")], CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    WriteElement("CustomerTaxID", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("CustomerTaxID")]), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    WriteElement("CompanyName", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("CompanyName")]), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    //<BillingAddress>
                    _xmlWriter.WriteStartElement("BillingAddress");
                    WriteElement("AddressDetail", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("AddressDetail")]), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    WriteElement("City", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("City")]), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    WriteElement("PostalCode", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("PostalCode")]), CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    WriteElement("Country", row.Values[xPSelectData.GetFieldIndexFromName("Country")], CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
                    _xmlWriter.WriteEndElement();
                    //</BillingAddress>
                    WriteElement("Telephone", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("Telephone")]));
                    WriteElement("Fax", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("Fax")]));
                    WriteElement("Email", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("Email")]));
                    WriteElement("Website", Entity.DecryptIfNeeded(row.Values[xPSelectData.GetFieldIndexFromName("Website")]));
                    WriteElement("SelfBillingIndicator", 0);
                    //</Customer>
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Used to Add Default Customer if is Missed
        private static void MasterFiles_Customer_DefaultCustomer()
        {
            string sqlCheckDefaultCustomer = string.Format(@"
                SELECT 
                    Count(*) AS Count 
                FROM 
                    fin_documentfinancemaster 
                WHERE 
                    EntityOid = '{2}' 
                    AND (Date >= '{0}' AND Date <= '{1}')
                ;"
                , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                , InvoiceSettings.FinalConsumerId
            );
            //_logger.Debug(string.Format("sqlCheckDefaultCustomer: [{0}]", sqlCheckDefaultCustomer));

            //<NumberOfEntries>
            object customerCount = XPOSettings.Session.ExecuteScalar(sqlCheckDefaultCustomer);

            //RETURN if have Default Customer
            if (Convert.ToInt16(customerCount) > 0) return;

            //Else Add Dafault Customer
            //<Customer>
            _xmlWriter.WriteStartElement("Customer");
            WriteElement("CustomerID", _defaultCustomer.CodeInternal);
            WriteElement("AccountID", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            WriteElement("CustomerTaxID", _defaultCustomer.FiscalNumber);
            WriteElement("CompanyName", _defaultCustomer.Name, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            //<BillingAddress>
            _xmlWriter.WriteStartElement("BillingAddress");
            WriteElement("AddressDetail", _defaultCustomer.Address, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            WriteElement("City", _defaultCustomer.City, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            WriteElement("PostalCode", _defaultCustomer.ZipCode, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            WriteElement("Country", _defaultCustomer.Country.Code2, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "saft_value_unknown"));
            _xmlWriter.WriteEndElement();
            //</BillingAddress>
            WriteElement("SelfBillingIndicator", 0);
            _xmlWriter.WriteEndElement();
            //</Customer>
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><Product>

        private static void MasterFiles_Product()
        {
            //<Product>
            //  <ProductType>I</ProductType>
            //  <ProductCode>IEC</ProductCode>
            //  <ProductDescription>Imposto Especial Consumo</ProductDescription>
            //  <ProductNumberCode>IEC</ProductNumberCode>
            //</Product>

            try
            {
                //right join fin_documentfinancedetail fd ON (fd.DocumentMaster = fm.Oid)
                string sql = string.Format(@"
                    SELECT 
	                    ac.Acronym AS ProductType, 
                        ar.Oid AS ProductCode, 
	                    af.Designation AS ProductGroup,
	                    ar.Designation AS ProductDescription,
	                    ar.BarCode AS ProductNumberCode
                    FROM 
	                    fin_documentfinancemaster as fm
                        left join fin_documentfinancedetail fd ON (fm.Oid = fd.DocumentMaster)
	                    left join fin_article ar ON (ar.Oid = fd.Article)
	                    left join fin_articlefamily af ON (af.Oid = ar.Family)
	                    left join fin_articleclass ac ON (ac.Oid = ar.Class)
                    WHERE 
	                    (fm.Date >= '{0}' AND fm.Date <= '{1}')
                    GROUP BY 
	                    ac.Acronym, ar.Oid, ar.Code, af.Designation, ar.Designation, ar.BarCode
                    ORDER BY
	                    af.Designation, ar.Designation
                    ;"
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                );
                //_logger.Debug(string.Format("sql: [{0}]", sql));

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    if (row.Values[1] != null)
                    {
                        //<Product>
                        _xmlWriter.WriteStartElement("Product");
                        WriteElement("ProductType", row.Values[xPSelectData.GetFieldIndexFromName("ProductType")]);
                        WriteElement("ProductCode", row.Values[xPSelectData.GetFieldIndexFromName("ProductCode")].ToString());
                        //Utilizado o descritivo da tabela “Familias”.
                        WriteElement("ProductGroup", row.Values[xPSelectData.GetFieldIndexFromName("ProductGroup")]);
                        WriteElement("ProductDescription", row.Values[xPSelectData.GetFieldIndexFromName("ProductDescription")]);
                        //Código EAN. Deve ser utilizado o código EAN (código de barras) do produto. Quando este não existir, preencher com o valor do campo “Identificador do Produto” 
                        string productNumberCodeField = (row.Values[xPSelectData.GetFieldIndexFromName("ProductNumberCode")] != null) ? "ProductNumberCode" : "ProductCode";
                        WriteElement("ProductNumberCode", row.Values[xPSelectData.GetFieldIndexFromName(productNumberCodeField)]);
                        //</Product>
                        _xmlWriter.WriteEndElement();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><TaxTable>
        //Informação das taxas de IVA definidas no sistema

        private static void MasterFiles_TaxTable()
        {
            //<TaxTableEntry>
            //  <TaxType>IVA</TaxType>
            //  <TaxCountryRegion>PT</TaxCountryRegion>
            //  <TaxCode>RED</TaxCode>
            //  <Description>Continente</Description>
            //  <TaxExpirationDate>2010-06-30</TaxExpirationDate>
            //  <TaxPercentage>5</TaxPercentage>
            //</TaxTableEntry>

            try
            {
                //<TaxTable>
                _xmlWriter.WriteStartElement("TaxTable");

                string sql = string.Format(@"
                    SELECT 
                        TaxType,
                        TaxCode,
                        TaxCountryRegion,
                        TaxDescription AS Description,
                        Value AS TaxPercentage
                    FROM 
                        fin_configurationvatrate 
                    WHERE 
                        Oid <> '{0}'
                    ORDER BY 
                        Ord
                ;"
                , XPOSettings.XpoOidUndefinedRecord
                );
                //_logger.Debug(string.Format("sql: [{0}]", sql));

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    //<TaxTableEntry>
                    _xmlWriter.WriteStartElement("TaxTableEntry");
                    WriteElement("TaxType", row.Values[xPSelectData.GetFieldIndexFromName("TaxType")]);
                    WriteElement("TaxCountryRegion", row.Values[xPSelectData.GetFieldIndexFromName("TaxCountryRegion")]);
                    WriteElement("TaxCode", row.Values[xPSelectData.GetFieldIndexFromName("TaxCode")]);
                    WriteElement("Description", row.Values[xPSelectData.GetFieldIndexFromName("Description")]);
                    WriteElement("TaxPercentage", DataConversionUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPercentage")]), _decimalFormat));
                    _xmlWriter.WriteEndElement();
                    //</TaxTableEntry>
                }

                //</TaxTable>
                _xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices>
        private static void SourceDocuments_SalesInvoices()
        {
            try
            {
                SourceDocuments_DocumentType(SaftDocumentType.SalesInvoices);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><MovementOfGoods>

        private static void SourceDocuments_MovementOfGoods()
        {
            try
            {
                SourceDocuments_DocumentType(SaftDocumentType.MovementOfGoods);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><WorkingDocuments>

        private static void SourceDocuments_WorkingDocuments()
        {
            try
            {
                SourceDocuments_DocumentType(SaftDocumentType.WorkingDocuments);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SourceDocuments : SalesInvoices|MovementOfGoods|WorkingDocuments
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices|MovementOfGoods|WorkingDocuments>

        private static void SourceDocuments_DocumentType(SaftDocumentType pSaftDocumentType)
        {
            //Shared for SaftDocumentType
            string documentNodeName = string.Empty;
            string documentNodeNameChild = string.Empty;
            string documentNodeNameChildNo = string.Empty;
            string documentNodeKeyWord = string.Empty;
            string documentNodeFilter = string.Empty;
            string documentNodeFilterTotalControl = string.Empty;

            switch (pSaftDocumentType)
            {
                case SaftDocumentType.SalesInvoices:
                    documentNodeName = "SalesInvoices";
                    documentNodeKeyWord = "Invoice";
                    documentNodeNameChild = "Invoice";
                    documentNodeNameChildNo = "InvoiceNo";
                    documentNodeFilter = "(ft.SaftDocumentType = 1 AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'F' OR fm.DocumentStatusStatus = 'A'))";
                    //Excluded : DocumentStatusStatus=A|F : AT: Os valores da tabela SalesInvoices (somas de controle), excluem os documentos com status A e F;
                    documentNodeFilterTotalControl = "(ft.SaftDocumentType = 1 AND (fm.DocumentStatusStatus = 'N'))";
                    break;
                case SaftDocumentType.MovementOfGoods:
                    documentNodeName = "MovementOfGoods";
                    documentNodeKeyWord = "Movement";
                    documentNodeNameChild = "StockMovement";
                    documentNodeNameChildNo = "DocumentNumber";
                    documentNodeFilter = "(ft.SaftDocumentType = 2 AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'T' OR fm.DocumentStatusStatus = 'F' OR fm.DocumentStatusStatus = 'R' OR fm.DocumentStatusStatus = 'A'))";
                    //Excluded : DocumentStatusStatus=A
                    documentNodeFilterTotalControl = "(ft.SaftDocumentType = 2 AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'T' OR fm.DocumentStatusStatus = 'F' OR fm.DocumentStatusStatus = 'R'))";
                    break;
                case SaftDocumentType.WorkingDocuments:
                    documentNodeName = "WorkingDocuments";
                    documentNodeKeyWord = "Work";
                    documentNodeNameChild = "WorkDocument";
                    documentNodeNameChildNo = "DocumentNumber";
                    documentNodeFilter = "(ft.SaftDocumentType = 3 AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'F' OR fm.DocumentStatusStatus = 'A'))";
                    //Excluded : DocumentStatusStatus=A
                    documentNodeFilterTotalControl = "(ft.SaftDocumentType = 3 AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'F'))";
                    break;
                default:
                    break;
            }

            try
            {
                //<NumberOfEntries>18</NumberOfEntries>
                //<TotalDebit>2201.7838</TotalDebit>
                //<TotalCredit>14085.3790</TotalCredit>

                string sqlNumberOfEntries = string.Format(@"
                    select 
                        COUNT(*) as NumberOfEntries
                    FROM
                        fin_documentfinancemaster fm
                        left join fin_documentfinancetype ft ON (fm.DocumentType = ft.Oid)
                    WHERE 
                        ft.SaftAuditFile = 1 
                        AND (fm.Date >= '{0}' AND fm.Date <= '{1}')
                        AND {2}
                    ;"
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                    , documentNodeFilter
                );
                //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlNumberOfEntries: [{1}]", pSaftDocumentType, sqlNumberOfEntries));

                //<NumberOfEntries>
                object numberOfEntries = XPOSettings.Session.ExecuteScalar(sqlNumberOfEntries);

                //RETURN if we dont have any Documents/NumberOfEntries
                if (Convert.ToInt16(numberOfEntries) <= 0) return;

                //<SalesInvoices|MovementOfGoods|WorkingDocuments>
                _xmlWriter.WriteStartElement(documentNodeName);

                switch (pSaftDocumentType)
                {
                    case SaftDocumentType.SalesInvoices:
                    case SaftDocumentType.WorkingDocuments:

                        WriteElement("NumberOfEntries", numberOfEntries);

                        //alteração de fdTotalGross -> fdTotalNet para contabilização dos descontos na soma do total de créditos
                        string sqlTotalDebitTotalCredit = string.Format(@"
                            SELECT 
                                SUM(fdTotalNet) AS Total
                            FROM 
                                view_documentfinance
                            WHERE 
                                ftSaftAuditFile = 1 AND ftCredit = {3} 
                                AND (fmDate >= '{0}' AND fmDate <= '{1}')
                                AND {2} 
                            "
                            , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                            , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                            //, documentNodeFilter.Replace("ft.", "ft").Replace("fm.", "fm")
                            , documentNodeFilterTotalControl.Replace("ft.", "ft").Replace("fm.", "fm")
                            , "{0}"
                         );

                        //<TotalDebit>
                        string sqlTotalDebit = string.Format(sqlTotalDebitTotalCredit, 0);
                        //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalDebit: [{1}]", pSaftDocumentType, sqlTotalDebit));

                        object totalDebit = XPOSettings.Session.ExecuteScalar(sqlTotalDebit);
                        if (totalDebit == null) totalDebit = 0;
                        WriteElement("TotalDebit", DataConversionUtils.DecimalToString(Convert.ToDecimal(totalDebit), _decimalFormat));

                        //<TotalCredit>
                        string sqlTotalCredit = string.Format(sqlTotalDebitTotalCredit, 1);
                        //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalCredit: [{1}]", pSaftDocumentType, sqlTotalCredit));
                        object totalCredit = XPOSettings.Session.ExecuteScalar(sqlTotalCredit);
                        if (totalCredit == null) totalCredit = 0;
                        WriteElement("TotalCredit", DataConversionUtils.DecimalToString(Convert.ToDecimal(totalCredit), _decimalFormat));

                        //<Invoice|StockMovement|WorkDocument>
                        SourceDocuments_DocumentType_Childs(pSaftDocumentType, documentNodeKeyWord, documentNodeNameChild, documentNodeNameChildNo, documentNodeFilter);
                        //</Invoice|StockMovement|WorkDocument>

                        _xmlWriter.WriteEndElement();
                        break;

                    case SaftDocumentType.MovementOfGoods:

                        string sqlTotalQuantityIssued = string.Format(@"
                            SELECT 
                                SUM(fdQuantity) AS Total
                            FROM 
                                view_documentfinance
                            WHERE 
                                ftSaftAuditFile = 1  
                                AND (fmDate >= '{0}' AND fmDate <= '{1}')
                                AND {2} 
                            "
                            , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                            , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                            , documentNodeFilterTotalControl.Replace("ft.", "ft").Replace("fm.", "fm")
                            , "{0}"
                         );

                        //<TotalDebit>
                        string sqlTotalQuantity = string.Format(sqlTotalQuantityIssued, 0);
                        //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalQuantityIssued: [{1}]", pSaftDocumentType, sqlTotalQuantityIssued));

                        object totalQuantity = XPOSettings.Session.ExecuteScalar(sqlTotalQuantity);
                        if (totalQuantity == null)
                        {
                            totalDebit = 0;
                            totalQuantity = 0;
                        }

                        WriteElement("NumberOfMovementLines", numberOfEntries);
                        WriteElement("TotalQuantityIssued", totalQuantity.ToString().Replace(',', '.')); //2015-01-20 apmuga verificar se é soma de kg+lt+m

                        //<Invoice|StockMovement|WorkDocument>
                        SourceDocuments_DocumentType_Childs(pSaftDocumentType, documentNodeKeyWord, documentNodeNameChild, documentNodeNameChildNo, documentNodeFilter);
                        //</Invoice|StockMovement|WorkDocument>

                        _xmlWriter.WriteEndElement();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices|MovementOfGoods|WorkingDocuments><Invoice|StockMovement|WorkDocument>

        private static void SourceDocuments_DocumentType_Childs(SaftDocumentType pSaftDocumentType, string pDocumentNodeKeyWord, string pDocumentNodeNameChild, string pDocumentNodeNameChildNo, string pdocumentNodeFilter)
        {
            //Sample for Invoices

            //<Invoice>
            //  <InvoiceNo>1T 1/6</InvoiceNo>
            //  <!-- Added for 1.04_01-->
            //  <ATCUD>0</ATCUD>
            //  <DocumentStatus>
            //    <InvoiceStatus>N</InvoiceStatus>
            //    <InvoiceStatusDate>2008-09-16T15:58:10</InvoiceStatusDate>
            //    <SourceID>INF</SourceID>
            //    <SourceBilling>P</SourceBilling>
            //  </DocumentStatus>
            //  <Hash>WAm0zPiWIXACFP0W2aZANXe6+fs25NoIJaFUVxhZgDVaAn3H8QYJUfFOSLRBeH/a7gmwxGDWY5Oq0J1QJ81jEsBfTtR4nMey+vMxxrCEokeUMU8A9o49ve2bc43yM+F9AHLLZ4N0K3AK6GOgkw36+O/Xc4TIfkXelsg4Y5XaZPw=</Hash>
            //  <HashControl>1</HashControl>
            //  <Period>10</Period>
            //  <InvoiceDate>2008-10-21</InvoiceDate>
            //  <InvoiceType>FT</InvoiceType>
            //  <SpecialRegimes>
            //    <SelfBillingIndicator>0</SelfBillingIndicator>
            //    <CashVATSchemeIndicator>0</CashVATSchemeIndicator>
            //    <ThirdPartiesBillingIndicator>0</ThirdPartiesBillingIndicator>
            //  </SpecialRegimes>
            //  <SourceID>INF</SourceID>
            //  <SystemEntryDate>2008-10-21T11:32:09</SystemEntryDate>
            //  <TransactionID>2008-10-21 VND 10</TransactionID>
            //  <CustomerID>CA3</CustomerID>
            //  <Line>
            //    <LineNumber>1</LineNumber>
            //    <ProductCode>FAT03</ProductCode>
            //    <ProductDescription>Fato de algodão (Produção própria)</ProductDescription>
            //    <Quantity>1</Quantity>
            //    <UnitOfMeasure>UN</UnitOfMeasure>
            //    <UnitPrice>5000</UnitPrice>
            //    <TaxPointDate>2008-10-21</TaxPointDate>
            //    <Description>Fato de algodão (Produção própria)</Description>
            //    <CreditAmount>5000</CreditAmount>
            //    <Tax>
            //      <TaxType>IVA</TaxType>
            //      <TaxCountryRegion>PT</TaxCountryRegion>
            //      <TaxCode>NOR</TaxCode>
            //      <TaxPercentage>20</TaxPercentage>
            //    </Tax>
            //    <TaxExemptionReason>Al... do n.º... do DL nº...</TaxExemptionReason>
            //    <!-- Added for 1.04_01-->
            //    <TaxExemptionCode>M05</TaxExemptionCode>
            //    <SettlementAmount>234.568</SettlementAmount>
            //  </Line>
            //  <DocumentTotals>
            //    <TaxPayable>1000</TaxPayable>
            //    <NetTotal>6000</NetTotal>
            //    <GrossTotal>7000.00</GrossTotal>
            //    <Currency>
            //      <CurrencyCode>USD</CurrencyCode>
            //      <CurrencyAmount>860.27</CurrencyAmount>
            //      <ExchangeRate>0.79</ExchangeRate>
            //    </Currency>
            //  </DocumentTotals>
            //</Invoice>

            try
            {
                string sql = string.Format(@"
                    SELECT
                        fm.Oid AS Oid,
                        fm.DocumentNumber AS DocumentNo,
                        fm.DocumentStatusStatus AS DocumentStatusStatus,
                        fm.DocumentStatusDate AS DocumentStatusDate,
                        fm.DocumentStatusReason AS DocumentStatusReason,
                        fm.DocumentStatusUser AS SourceIDStatus,
                        fm.SourceBilling AS SourceBilling,
                        fm.Hash AS Hash,
                        fm.HashControl AS HashControl,
                        fm.DocumentDate AS DocumentDate,
                        ft.Acronym AS DocumentType,
                        fm.SelfBillingIndicator,
                        fm.CashVatSchemeIndicator,
                        fm.ThirdPartiesBillingIndicator,
                        fm.DocumentCreatorUser AS SourceIDCreator,
                        fm.EACCode AS EACCode,
                        fm.SystemEntryDate AS SystemEntryDate,
                        fm.TransactionID AS TransactionID,
                        fm.EntityInternalCode AS CustomerID,
                        pm.Acronym AS PaymentMechanism,
                        (fm.TotalDelivery - fm.TotalChange) AS PaymentAmount,
                        ft.WayBill AS WayBill,
                        fm.ShipToDeliveryID ,
                        fm.ShipToDeliveryDate,
                        fm.ShipToWarehouseID,
                        fm.ShipToLocationID,
                        fm.ShipToBuildingNumber,
                        fm.ShipToStreetName,
                        fm.ShipToAddressDetail,
                        fm.ShipToCity,
                        fm.ShipToPostalCode,
                        fm.ShipToRegion,
                        fm.ShipToCountry,
                        fm.ShipFromDeliveryID,
                        fm.ShipFromDeliveryDate,
                        fm.ShipFromWarehouseID,
                        fm.ShipFromLocationID,
                        fm.ShipFromBuildingNumber,
                        fm.ShipFromStreetName,
                        fm.ShipFromAddressDetail,
                        fm.ShipFromCity,
                        fm.ShipFromPostalCode,
                        fm.ShipFromRegion,
                        fm.ShipFromCountry,
                        fm.MovementEndTime,
                        fm.MovementStartTime,
                        fm.ATDocCodeID,
                        fm.ExchangeRate,
                        cc.Acronym AS CurrencyCode, 
                        0 AS ATCUD,
                        fm.TotalNet as TotalNet,
                        fm.TotalDiscount as TotalDiscount,
                        fm.TotalFinal as TotalFinal
                    FROM
                        fin_documentfinancemaster fm
                        left join fin_documentfinancetype ft ON (fm.DocumentType = ft.Oid)
                        left join fin_configurationpaymentmethod pm ON (fm.PaymentMethod = pm.Oid)
                        left join cfg_configurationcurrency cc ON (fm.Currency = cc.Oid)
                    WHERE 
                        ft.SaftAuditFile = 1 
                        AND (fm.Date >= '{0}' AND fm.Date <= '{1}')
                        AND {2}
                    ORDER BY 
                        Date
                    ;
                    "
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                    , pdocumentNodeFilter
                );
                //_logger.Debug(string.Format("SaftDocumentType:[{0}] sql: [{1}]", pSaftDocumentType, sql));

                Guid guidDocumentMaster;
                //Declare Vars for Currency/ExchangeRate
                decimal documentExchangeRate = 0.0m;
                decimal currencyCurrencyAmount = 0.0m;
                decimal currencyExchangeRate = 0.0m;
                //Declare Var to Store Acronym Type for Replace OR and FP with DC (Acronym)
                string documentType = string.Empty;
                bool wayBill = false;

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    //Protected Documents with total amount and discount zero 
                    if (Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TotalDiscount")].ToString()) == 0 && Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TotalNet")].ToString()) == 0)
                    {
                        continue;
                    }
                    //<Invoice|StockMovement|WorkDocument>
                    _xmlWriter.WriteStartElement(pDocumentNodeNameChild);
                    WriteElement(pDocumentNodeNameChildNo, row.Values[xPSelectData.GetFieldIndexFromName("DocumentNo")]);
                    WriteElement("ATCUD", row.Values[xPSelectData.GetFieldIndexFromName("ATCUD")]);
                    //<DocumentStatus>
                    _xmlWriter.WriteStartElement("DocumentStatus");
                    WriteElement(string.Format("{0}Status", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndexFromName("DocumentStatusStatus")]);
                    WriteElement(string.Format("{0}StatusDate", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndexFromName("DocumentStatusDate")]);
                    WriteElement("Reason", row.Values[xPSelectData.GetFieldIndexFromName("DocumentStatusReason")]);
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceIDStatus")].ToString());
                    WriteElement("SourceBilling", row.Values[xPSelectData.GetFieldIndexFromName("SourceBilling")]);
                    _xmlWriter.WriteEndElement();
                    //</DocumentStatus>
                    WriteElement("Hash", row.Values[xPSelectData.GetFieldIndexFromName("Hash")]);
                    WriteElement("HashControl", row.Values[xPSelectData.GetFieldIndexFromName("HashControl")]);
                    WriteElement("Period", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]).Month);
                    WriteElement(string.Format("{0}Date", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndexFromName("DocumentDate")]);
                    //Required to Replace WorkType OR and FP with DC
                    documentType = row.Values[xPSelectData.GetFieldIndexFromName("DocumentType")].ToString();
                    //Replace DocumentType if Detect a ConferenceDocument (OR|FP|DC)
                    if (documentType == "OR" || documentType == "FP") documentType = "DC";
                    //Detect if DocumentType is WayBill
                    wayBill = Convert.ToBoolean(row.Values[xPSelectData.GetFieldIndexFromName("WayBill")]);

                    //Write Element
                    WriteElement(string.Format("{0}Type", pDocumentNodeKeyWord), documentType);

                    switch (pSaftDocumentType)
                    {
                        case SaftDocumentType.SalesInvoices:
                            try
                            {
                                //<SpecialRegimes>
                                _xmlWriter.WriteStartElement("SpecialRegimes");
                                WriteElement("SelfBillingIndicator", row.Values[xPSelectData.GetFieldIndexFromName("SelfBillingIndicator")]);
                                WriteElement("CashVATSchemeIndicator", row.Values[xPSelectData.GetFieldIndexFromName("CashVatSchemeIndicator")]);
                                WriteElement("ThirdPartiesBillingIndicator", row.Values[xPSelectData.GetFieldIndexFromName("ThirdPartiesBillingIndicator")]);
                                _xmlWriter.WriteEndElement();
                                //</SpecialRegimes> 

                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndexFromName("EACCode")]);
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]);
                                //WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndex("TransactionID")]);
                                if (row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")] != null)
                                {
                                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")]);
                                }
                                else
                                {
                                    WriteElement("CustomerID", _defaultCustomer.CodeInternal);
                                }

                                //Check if is WayBill Document and Call ShipDetails Helper to Output ShipTo|ShipFrom Details
                                if (wayBill) SourceDocuments_DocumentType_Childs_ShipDetails(xPSelectData, row);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message, ex);
                            };
                            break;

                        case SaftDocumentType.MovementOfGoods:
                            try
                            {
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]);
                                WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndexFromName("TransactionID")]);
                                WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")].ToString());
                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndexFromName("EACCode")]);

                                //Allways Call ShipDetails Helper to Output ShipTo|ShipFrom Details
                                SourceDocuments_DocumentType_Childs_ShipDetails(xPSelectData, row);

                                //4.2.3.19: Used only in MovementOfGoods: Código de identificação atribuído pela AT ao documento, nos termos do Decreto - Lei n.º 147/2003, de 11 de julho.
                                WriteElement("ATDocCodeID", row.Values[xPSelectData.GetFieldIndexFromName("ATDocCodeID")]);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message, ex);
                            }
                            break;

                        case SaftDocumentType.WorkingDocuments:
                            try
                            {
                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndexFromName("EACCode")]);
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]);
                                if (row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")] != null)
                                {
                                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")]);
                                }
                                else
                                {
                                    WriteElement("CustomerID", _defaultCustomer.CodeInternal);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message, ex);
                            }
                            break;
                    }

                    //<Line>
                    guidDocumentMaster = new Guid(row.Values[xPSelectData.GetFieldIndexFromName("Oid")].ToString());
                    TotalLinesResult totalLineResult = SourceDocuments_Lines(pSaftDocumentType, guidDocumentMaster);
                    //</Line>

                    //<DocumentTotals>
                    //  <TaxPayable>1.59</TaxPayable>
                    //  <NetTotal>6.91</NetTotal>
                    //  <GrossTotal>8.50</GrossTotal>
                    //  <Payment>
                    //    <PaymentMechanism>NU</PaymentMechanism>
                    //    <PaymentAmount>8.50</PaymentAmount>
                    //    <PaymentDate>2014-07-16</PaymentDate>
                    //  </Payment>
                    //</DocumentTotals>

                    //<DocumentTotals>
                    _xmlWriter.WriteStartElement("DocumentTotals");
                    WriteElement("TaxPayable", DataConversionUtils.DecimalToString(totalLineResult.TaxPayable, _decimalFormatTotals));
                    WriteElement("NetTotal", DataConversionUtils.DecimalToString(totalLineResult.NetTotal, _decimalFormatTotals));
                    WriteElement("GrossTotal", DataConversionUtils.DecimalToString(totalLineResult.GrossTotal, _decimalFormatTotals));

                    //Currency
                    if (_defaultCurrency.Acronym != row.Values[xPSelectData.GetFieldIndexFromName("CurrencyCode")].ToString())
                    {
                        //Calculate Totals
                        documentExchangeRate = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("ExchangeRate")]);
                        currencyCurrencyAmount = totalLineResult.GrossTotal * documentExchangeRate;
                        currencyExchangeRate = totalLineResult.GrossTotal / currencyCurrencyAmount;
                        //<Currency>
                        _xmlWriter.WriteStartElement("Currency");
                        WriteElement("CurrencyCode", row.Values[xPSelectData.GetFieldIndexFromName("CurrencyCode")].ToString());
                        WriteElement("CurrencyAmount", DataConversionUtils.DecimalToString(currencyCurrencyAmount, _decimalFormat));
                        //In SAT-F Example we have 2 examples one with decimals 0.00 and other with 0.00000000000 opted to use divide value without conversion
                        WriteElement("ExchangeRate", DataConversionUtils.DecimalToString(currencyExchangeRate, _decimalFormat));
                        //WriteElement("ExchangeRate", currencyExchangeRate);
                        _xmlWriter.WriteEndElement();
                        //</Currency>
                    }

                    //</SalesInvoices|MovementOfGoods|WorkingDocuments>
                    switch (pSaftDocumentType)
                    {
                        case SaftDocumentType.SalesInvoices:
                            //Get decimal to check if Greater than Zero
                            decimal paymentAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("PaymentAmount")]);
                            //Only Export if Greater Than Zero (SAF-T Recomendation)
                            if (paymentAmount > 0.0m)
                            {
                                //<Payment>
                                _xmlWriter.WriteStartElement("Payment");
                                //Default : OU : OtherPayments /Outros Pagamentos
                                WriteElement("PaymentMechanism", row.Values[xPSelectData.GetFieldIndexFromName("PaymentMechanism")], "OU");
                                WriteElement("PaymentAmount", DataConversionUtils.DecimalToString(paymentAmount, _decimalFormat));
                                WriteElement("PaymentDate", row.Values[xPSelectData.GetFieldIndexFromName("DocumentDate")]);
                                _xmlWriter.WriteEndElement();
                                //</Payment>
                            }
                            break;
                    }

                    _xmlWriter.WriteEndElement();
                    //</DocumentTotals>

                    //</Invoice|StockMovement|WorkDocument>
                    _xmlWriter.WriteEndElement();

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Helper to Output ShiptTo and ShipFrom Details, Shared for Invoice|StockMovement SaftDocumentType
        private static void SourceDocuments_DocumentType_Childs_ShipDetails(SQLSelectResultData pXPSelectData, SelectStatementResultRow pRow)
        {
            //<ShipTo>
            _xmlWriter.WriteStartElement("ShipTo");
            WriteElement("DeliveryID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToDeliveryID")]);
            WriteElement("DeliveryDate", XPOUtility.DateToString(pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToDeliveryDate")]));
            WriteElement("WarehouseID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToWarehouseID")]);
            WriteElement("LocationID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToLocationID")]);
            //<Address>
            _xmlWriter.WriteStartElement("Address");
            WriteElement("BuildingNumber", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToBuildingNumber")]);
            WriteElement("StreetName", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToStreetName")]);
            WriteElement("AddressDetail", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToAddressDetail")]);
            WriteElement("City", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToCity")]);
            WriteElement("PostalCode", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToPostalCode")]);
            WriteElement("Region", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToRegion")]);
            WriteElement("Country", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipToCountry")]);
            _xmlWriter.WriteEndElement();
            //</Address>
            _xmlWriter.WriteEndElement();
            //<ShipTo>

            //<ShipFrom>
            _xmlWriter.WriteStartElement("ShipFrom");
            WriteElement("DeliveryID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromDeliveryID")]);
            WriteElement("DeliveryDate", XPOUtility.DateToString(pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromDeliveryDate")]));
            WriteElement("WarehouseID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromWarehouseID")]);
            WriteElement("LocationID", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromLocationID")]);
            //<Address>
            _xmlWriter.WriteStartElement("Address");
            WriteElement("BuildingNumber", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromBuildingNumber")]);
            WriteElement("StreetName", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromStreetName")]);
            WriteElement("AddressDetail", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromAddressDetail")]);
            WriteElement("City", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromCity")]);
            WriteElement("PostalCode", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromPostalCode")]);
            WriteElement("Region", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromRegion")]);
            WriteElement("Country", pRow.Values[pXPSelectData.GetFieldIndexFromName("ShipFromCountry")]);
            _xmlWriter.WriteEndElement();
            //</Address>
            _xmlWriter.WriteEndElement();
            //</ShipFrom>

            //Export if not Null else gives wrong values ex "0001-01-01T00:00:00" | Always Null, Its not persisted yet, but has stub code here to work when its not null
            if (pRow.Values[pXPSelectData.GetFieldIndexFromName("MovementEndTime")] != null)
            {
                WriteElement("MovementEndTime", XPOUtility.DateTimeToCombinedDateTimeString(pRow.Values[pXPSelectData.GetFieldIndexFromName("MovementEndTime")]));
            }
            WriteElement("MovementStartTime", XPOUtility.DateTimeToCombinedDateTimeString(pRow.Values[pXPSelectData.GetFieldIndexFromName("MovementStartTime")]));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices><Invoice><Line>
        public class TotalLinesResult
        {
            public decimal TaxPayable;
            public decimal NetTotal;
            public decimal GrossTotal;//TotalFinal

            public TotalLinesResult()
            {
                TaxPayable = 0.0m;
                NetTotal = 0.0m;
                GrossTotal = 0.0m;
            }
        }

        private static TotalLinesResult SourceDocuments_Lines(SaftDocumentType pSaftDocumentType, Guid pDocumentMaster)
        {
            try
            {
                TotalLinesResult totalLineResult = new TotalLinesResult();

                string SqlQuery = @"
                    SELECT 
                        fmDiscount AS GlobalDiscount,                        
                        fdOid AS Oid,
                        ftCredit AS Credit,
                        fdOrd AS LineNumber,
                        fdArticle AS ProductCode,
                        fdDesignation AS ProductDescription,
                        fdQuantity AS Quantity,
                        fdPriceWithDiscount AS PriceWithDiscount,
                        fdUnitMeasure AS UnitOfMeasure,
                        fmDocumentDate AS TaxPointDate,
                        cvTaxType AS TaxType,
                        cvTaxCountryRegion AS TaxCountryRegion,
                        cvTaxCode AS TaxCode, 
                        fdVat AS TaxPercentage,
                        fdTotalTax AS TaxPayable,
                        fdTotalNet AS NetTotal,
                        fdTotalFinal AS GrossTotal,
                        fdTotalDiscount AS SettlementAmount,
                        fdVatExemptionReasonDesignation AS TaxExemptionReason,
                        cxAcronym AS TaxExemptionCode
                    FROM  
                        view_documentfinance
                    WHERE 
                        fmOid = '{0}'
                    ORDER BY 
                        fdOrd;
                ";

                string sql = string.Format(SqlQuery, pDocumentMaster.ToString());
                //_logger.Debug(string.Format("sql: [{0}]", sql));

                //Init Local Vars
                string nodeNameCreditOrDebitAmount;
                decimal globalDocumentDiscount = 0.0m;
                decimal linePriceWithDiscount = 0.0m;
                decimal lineUnitPrice = 0.0m;
                decimal lineQuantity = 0.0m;
                decimal lineCreditOrDebit = 0.0m;
                decimal lineTaxPayable = 0.0m;
                decimal lineNetTotal = 0.0m;
                decimal lineGrossTotal = 0.0m;

                Guid guidDocumentDetail = new Guid();
                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                if (xPSelectData.DataRows.Length > 0)
                    foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                    {
                        nodeNameCreditOrDebitAmount = (Convert.ToInt16(row.Values[xPSelectData.GetFieldIndexFromName("Credit")]) == 1) ? "CreditAmount" : "DebitAmount";

                        linePriceWithDiscount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("PriceWithDiscount")]);
                        globalDocumentDiscount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("GlobalDiscount")]); ;
                        //Remove Global Discount, this way UnitPrice dont have discounts (Line and Global) and dont have Taxs
                        lineUnitPrice = linePriceWithDiscount - ((linePriceWithDiscount * globalDocumentDiscount) / 100);
                        lineQuantity = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("Quantity")]);
                        lineCreditOrDebit = lineUnitPrice * lineQuantity;

                        lineTaxPayable = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPayable")]), CultureSettings.DecimalRoundTo);
                        lineNetTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("NetTotal")]), CultureSettings.DecimalRoundTo);
                        lineGrossTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("GrossTotal")]), CultureSettings.DecimalRoundTo);
                        totalLineResult.TaxPayable += lineTaxPayable;
                        totalLineResult.NetTotal += lineNetTotal;
                        totalLineResult.GrossTotal += lineGrossTotal;

                        if (row.Values[xPSelectData.GetFieldIndexFromName("ProductCode")] != null && lineNetTotal != 0.0m)
                        {
                            //<Line>
                            _xmlWriter.WriteStartElement("Line");

                            WriteElement("LineNumber", row.Values[xPSelectData.GetFieldIndexFromName("LineNumber")]);
                            switch (pSaftDocumentType)
                            {
                                case SaftDocumentType.SalesInvoices:
                                case SaftDocumentType.MovementOfGoods:
                                case SaftDocumentType.WorkingDocuments:
                                    //<OrderReferences>
                                    guidDocumentDetail = new Guid(row.Values[xPSelectData.GetFieldIndexFromName("Oid")].ToString());
                                    SourceDocuments_Lines_OrderReferences(guidDocumentDetail);
                                    //</OrderReferences>
                                    break;
                            }

                            WriteElement("ProductCode", row.Values[xPSelectData.GetFieldIndexFromName("ProductCode")].ToString());
                            WriteElement("ProductDescription", row.Values[xPSelectData.GetFieldIndexFromName("ProductDescription")]);
                            WriteElement("Quantity", DataConversionUtils.DecimalToString(lineQuantity, _decimalFormat));
                            WriteElement("UnitOfMeasure", row.Values[xPSelectData.GetFieldIndexFromName("UnitOfMeasure")]);
                            WriteElement("UnitPrice", DataConversionUtils.DecimalToString(lineUnitPrice, _decimalFormat));

                            switch (pSaftDocumentType)
                            {
                                case SaftDocumentType.SalesInvoices:
                                case SaftDocumentType.WorkingDocuments:
                                    //<TaxPointDate>
                                    WriteElement("TaxPointDate", row.Values[xPSelectData.GetFieldIndexFromName("TaxPointDate")]);
                                    //<//TaxPointDate>
                                    break;
                            }

                            switch (pSaftDocumentType)
                            {
                                case SaftDocumentType.SalesInvoices:
                                    //<References>
                                    SourceDocuments_Lines_References(guidDocumentDetail);
                                    //</References>
                                    break;
                            }

                            WriteElement("Description", row.Values[xPSelectData.GetFieldIndexFromName("ProductDescription")]);
                            //CreditAmount|DebitAmount
                            WriteElement(nodeNameCreditOrDebitAmount, DataConversionUtils.DecimalToString(lineCreditOrDebit, _decimalFormat));

                            //<Tax>
                            _xmlWriter.WriteStartElement("Tax");
                            WriteElement("TaxType", row.Values[xPSelectData.GetFieldIndexFromName("TaxType")]);
                            WriteElement("TaxCountryRegion", row.Values[xPSelectData.GetFieldIndexFromName("TaxCountryRegion")]);
                            WriteElement("TaxCode", row.Values[xPSelectData.GetFieldIndexFromName("TaxCode")]);
                            WriteElement("TaxPercentage", DataConversionUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPercentage")]), _decimalFormat));
                            _xmlWriter.WriteEndElement();
                            Console.WriteLine(row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionReason")]);
                            Console.WriteLine(row.Values[xPSelectData.GetFieldIndexFromName("TaxPercentage")]);

                            if (row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionReason")] == null || string.IsNullOrEmpty(row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionReason")].ToString()))
                            {
                                Console.WriteLine("Taxa 0 sem motivo de isenção");
                                if ((Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPercentage")])) == 0.00m)
                                {
                                    //vai buscar á configuração do artigo
                                    fin_article article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), Guid.Parse(row.Values[xPSelectData.GetFieldIndexFromName("ProductCode")].ToString()));
                                    if (article != null && article.VatExemptionReason != null)
                                    {
                                        row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionReason")] = article.VatExemptionReason.Designation;
                                        row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionCode")] = article.VatExemptionReason.Acronym;
                                    }
                                }
                            }
                            //</Tax>
                            if ((Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPercentage")])) == 0.00m)
                            {
                                WriteElement("TaxExemptionReason", row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionReason")]);
                                WriteElement("TaxExemptionCode", row.Values[xPSelectData.GetFieldIndexFromName("TaxExemptionCode")]);
                                if (Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("SettlementAmount")]) > 0.0m)
                                    WriteElement("SettlementAmount", DataConversionUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("SettlementAmount")]), _decimalFormat));
                            }


                            _xmlWriter.WriteEndElement();
                        }
                        //</Line>
                    }
                return totalLineResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices><Invoice><Line><OrderReferences>
        private static void SourceDocuments_Lines_OrderReferences(Guid pDocumentMasterDetail)
        {
            //<Line>
            //  <LineNumber>1</LineNumber>
            //  <OrderReferences>
            //    <OriginatingON>GR 1/2</OriginatingON>
            //    <OrderDate>2008-09-15</OrderDate>
            //  </OrderReferences>
            //  ...
            //</Line>

            //Protection to skip Export <OrderReferences> when Document Type is CreditNote
            fin_documentfinancedetail documentFinanceDetail = (fin_documentfinancedetail)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancedetail), pDocumentMasterDetail);
            if (documentFinanceDetail.DocumentMaster.DocumentType.Oid != CustomDocumentSettings.CreditNoteId)
            {
                try
                {
                    string sql = string.Format(@"
                        SELECT 
                            OriginatingON,
                            OrderDate
                        FROM 
                            fin_documentfinancedetailorderreference 
                        WHERE 
                            DocumentDetail = '{0}'
                        ORDER BY 
                            Ord;
                        "
                        , pDocumentMasterDetail.ToString()
                    );
                    //_logger.Debug(string.Format("sql: [{0}]", sql));

                    SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                    foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                    {
                        //<OrderReferences>
                        _xmlWriter.WriteStartElement("OrderReferences");
                        WriteElement("OriginatingON", row.Values[xPSelectData.GetFieldIndexFromName("OriginatingON")]);
                        WriteElement("OrderDate", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndexFromName("OrderDate")]).ToString(_dateTimeFormatDocumentDate));
                        _xmlWriter.WriteEndElement();
                        //</OrderReferences>
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices><Invoice><Line><References>
        private static void SourceDocuments_Lines_References(Guid pDocumentMasterDetail)
        {
            //<Line>
            //  <LineNumber>1</LineNumber>
            //  ...
            //  <TaxPointDate>2008-12-02</TaxPointDate>
            //  <References>
            //    <Reference>1T 1/6</Reference>
            //    <Reason>Não havia sido liquidado o valor da mão-de-obra</Reason>
            //  </References>
            //  <Description>Mão de obra (Hora) em falta referente à fatura 1T 1/6</Description>
            //  ...
            //</Line>	

            //Protection to skip Export <References> when Document Type is NOT CreditNote
            fin_documentfinancedetail documentFinanceDetail = (fin_documentfinancedetail)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancedetail), pDocumentMasterDetail);
            if (documentFinanceDetail.DocumentMaster.DocumentType.Oid == CustomDocumentSettings.CreditNoteId)
            {
                try
                {
                    string sql = string.Format(@"
                    SELECT 
                        Reference,
                        Reason
                    FROM 
                        fin_documentfinancedetailreference
                    WHERE 
                        DocumentDetail = '{0}'
                    ORDER BY 
                        Ord;
                    "
                        , pDocumentMasterDetail.ToString()
                    );
                    //_logger.Debug(string.Format("sql: [{0}]", sql));

                    SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                    foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                    {
                        //<References>
                        _xmlWriter.WriteStartElement("References");
                        WriteElement("Reference", row.Values[xPSelectData.GetFieldIndexFromName("Reference")]);
                        WriteElement("Reason", row.Values[xPSelectData.GetFieldIndexFromName("Reason")]);
                        _xmlWriter.WriteEndElement();
                        //</References>
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SourceDocuments : Payments
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><Payments>

        private static void SourceDocuments_Payments()
        {
            try
            {
                //<NumberOfEntries>18</NumberOfEntries>
                //<TotalDebit>2201.7838</TotalDebit>
                //<TotalCredit>14085.3790</TotalCredit>

                string sqlNumberOfEntries = string.Format(@"
                    select 
                        COUNT(*) as NumberOfEntries
                    FROM
                        fin_documentfinancepayment
                    WHERE 
                        (PaymentDate >= '{0}' AND PaymentDate <= '{1}')
                        AND (PaymentStatus = 'N' OR PaymentStatus = 'A')
                    ;"
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                );
                //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlNumberOfEntries: [{1}]", SaftDocumentType.Payments, sqlNumberOfEntries));

                //<NumberOfEntries>
                object numberOfEntries = XPOSettings.Session.ExecuteScalar(sqlNumberOfEntries);

                //RETURN if we dont have any Documents/NumberOfEntries
                if (Convert.ToInt16(numberOfEntries) <= 0) return;

                //<Payments>
                _xmlWriter.WriteStartElement("Payments");

                WriteElement("NumberOfEntries", numberOfEntries);

                string sqlTotalDebitTotalCredit = string.Format(@"
                    SELECT 
                        SUM(fmp{2}Amount) AS Total
                    FROM 
                        view_documentfinancepayment
                    WHERE 
	                    (fpaPaymentDate >= '{0}' AND fpaPaymentDate <= '{1}')
	                    AND (fpaPaymentStatus = 'N' AND fpaPaymentStatus <> 'A')
                    "
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                    , "{0}"
                    );

                //<TotalDebit>
                string sqlTotalDebit = string.Format(sqlTotalDebitTotalCredit, "Debit");
                //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalDebit: [{1}]", SaftDocumentType.Payments, sqlTotalDebit));

                object totalDebit = XPOSettings.Session.ExecuteScalar(sqlTotalDebit);
                if (totalDebit == null) totalDebit = 0;
                WriteElement("TotalDebit", DataConversionUtils.DecimalToString(Convert.ToDecimal(totalDebit), _decimalFormat));

                //<TotalCredit>
                string sqlTotalCredit = string.Format(sqlTotalDebitTotalCredit, "Credit");
                //_logger.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalCredit: [{1}]", SaftDocumentType.Payments, sqlTotalCredit));
                object totalCredit = XPOSettings.Session.ExecuteScalar(sqlTotalCredit);
                if (totalCredit == null) totalCredit = 0;
                WriteElement("TotalCredit", DataConversionUtils.DecimalToString(Convert.ToDecimal(totalCredit), _decimalFormat));

                //<Payment>
                SourceDocuments_Payments_Childs();
                //</Payment>

                _xmlWriter.WriteEndElement();
                //</Payments>
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void SourceDocuments_Payments_Childs()
        {
            //Sample for Payments
            //<Payment>
            //    <PaymentRefNo>PGT 1T1/1</PaymentRefNo>
            //    <!-- Added for 1.04_01-->
            //    <ATCUD>0</ATCUD>
            //    <Period>10</Period>
            //    <TransactionID>2008-10-22 BNC 4</TransactionID>
            //    <TransactionDate>2008-10-22</TransactionDate>
            //    <PaymentType>RG</PaymentType>
            //    <Description>Pagamento da fatura 1T 1/6</Description>
            //    <SystemID>14132</SystemID>
            //    <DocumentStatus>
            //        <PaymentStatus>N</PaymentStatus>
            //        <PaymentStatusDate>2008-10-22T09:11:34</PaymentStatusDate>
            //        <Reason>xxxxx</Reason>
            //        <SourceID>Z98-Carlos</SourceID>
            //        <SourcePayment>P</SourcePayment>
            //    </DocumentStatus>
            //    <PaymentMethod>
            //        <PaymentMechanism>TB</PaymentMechanism>
            //        <PaymentAmount>5500</PaymentAmount>
            //        <PaymentDate>2008-10-21</PaymentDate>
            //    </PaymentMethod>
            //    <PaymentMethod>
            //        <PaymentMechanism>CC</PaymentMechanism>
            //        <PaymentAmount>1500</PaymentAmount>
            //        <PaymentDate>2008-10-21</PaymentDate>
            //    </PaymentMethod>
            //    <SourceID>Z98-Carlos</SourceID>
            //    <SystemEntryDate>2008-10-22T09:11:34</SystemEntryDate>
            //    <CustomerID>CA3</CustomerID>
            //    <Line>
            //        <LineNumber>1</LineNumber>
            //        <SourceDocumentID>
            //            <OriginatingON>1T 1/6</OriginatingON>
            //            <InvoiceDate>2008-10-21</InvoiceDate>
            //            <Description>Bens à taxa normal de IVA</Description>
            //        </SourceDocumentID>
            //        <CreditAmount>5000</CreditAmount>
            //        <Tax>
            //            <TaxType>IVA</TaxType>
            //            <TaxCountryRegion>PT</TaxCountryRegion>
            //            <TaxCode>NOR</TaxCode>
            //            <TaxPercentage>20</TaxPercentage>
            //        </Tax>
            //    </Line>
            //    <DocumentTotals>
            //        <TaxPayable>1000</TaxPayable>
            //        <NetTotal>6000</NetTotal>
            //        <GrossTotal>7000.00</GrossTotal>
            //    </DocumentTotals>
            //    <Currency>
            //      <CurrencyCode>USD</CurrencyCode>
            //      <CurrencyAmount>317.45</CurrencyAmount>
            //      <ExchangeRate>0.7407465742636636</ExchangeRate>
            //    </Currency>
            //</Payment>
            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Oid,
	                    PaymentRefNo,
	                    TransactionID,
	                    TransactionDate,
	                    PaymentType,
	                    Notes AS Description,
	                    PaymentStatus,
	                    PaymentStatusDate,
	                    Reason,
	                    SourceID,
	                    SourcePayment,
	                    PaymentMechanism,
	                    PaymentAmount,
	                    PaymentDate,
	                    SystemEntryDate,
	                    EntityInternalCode as CustomerID,
                        CurrencyCode,
                        CurrencyAmount,
                        ExchangeRate, 
                        0 AS ATCUD
                    FROM 
	                    fin_documentfinancepayment
                    WHERE 
	                    (PaymentDate >= '{0}' AND PaymentDate <= '{1}')
	                    AND (PaymentStatus = 'N' OR PaymentStatus = 'A')
                    ORDER BY 
	                    SystemEntryDate
                    ;
                    "
                    , _documentDateStart.ToString(CultureSettings.DateTimeFormat)
                    , _documentDateEnd.ToString(CultureSettings.DateTimeFormat)
                );
                //_logger.Debug(string.Format("SaftDocumentType:[{0}] sql: [{1}]", SaftDocumentType.Payments, sql));

                Guid guidFinanceDocumentPayment;
                decimal currencyAmount;
                decimal exchangeRate;

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                {
                    //<Payment>
                    _xmlWriter.WriteStartElement("Payment");
                    WriteElement("PaymentRefNo", row.Values[xPSelectData.GetFieldIndexFromName("PaymentRefNo")]);
                    WriteElement("ATCUD", row.Values[xPSelectData.GetFieldIndexFromName("ATCUD")]);
                    WriteElement("Period", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]).Month);
                    WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndexFromName("TransactionID")]);
                    WriteElement("TransactionDate", row.Values[xPSelectData.GetFieldIndexFromName("TransactionDate")]);
                    WriteElement("PaymentType", row.Values[xPSelectData.GetFieldIndexFromName("PaymentType")]);
                    WriteElement("Description", row.Values[xPSelectData.GetFieldIndexFromName("Description")]);
                    //<DocumentStatus>
                    _xmlWriter.WriteStartElement("DocumentStatus");
                    WriteElement("PaymentStatus", row.Values[xPSelectData.GetFieldIndexFromName("PaymentStatus")]);
                    WriteElement("PaymentStatusDate", row.Values[xPSelectData.GetFieldIndexFromName("PaymentStatusDate")]);
                    WriteElement("Reason", row.Values[xPSelectData.GetFieldIndexFromName("Reason")]);
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceID")]);
                    WriteElement("SourcePayment", row.Values[xPSelectData.GetFieldIndexFromName("SourcePayment")]);
                    _xmlWriter.WriteEndElement();
                    //</DocumentStatus>
                    //<PaymentMethod>
                    _xmlWriter.WriteStartElement("PaymentMethod");
                    WriteElement("PaymentMechanism", row.Values[xPSelectData.GetFieldIndexFromName("PaymentMechanism")]);
                    WriteElement("PaymentAmount", DataConversionUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("PaymentAmount")]), _decimalFormat));
                    WriteElement("PaymentDate", row.Values[xPSelectData.GetFieldIndexFromName("PaymentDate")]);
                    _xmlWriter.WriteEndElement();
                    //</PaymentMethod>
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndexFromName("SourceID")]);
                    WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndexFromName("SystemEntryDate")]);
                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndexFromName("CustomerID")]);

                    //<Line>
                    guidFinanceDocumentPayment = new Guid(row.Values[xPSelectData.GetFieldIndexFromName("Oid")].ToString());
                    TotalLinesResult totalLineResult = SourceDocuments_Payments_Lines(guidFinanceDocumentPayment);
                    //</Line>

                    //<DocumentTotals>
                    _xmlWriter.WriteStartElement("DocumentTotals");
                    WriteElement("TaxPayable", DataConversionUtils.DecimalToString(totalLineResult.TaxPayable, _decimalFormatTotals));
                    WriteElement("NetTotal", DataConversionUtils.DecimalToString(totalLineResult.NetTotal, _decimalFormatTotals));
                    WriteElement("GrossTotal", DataConversionUtils.DecimalToString(totalLineResult.GrossTotal, _decimalFormatTotals));

                    //Note: 4.4.4.17 in 130823_Portaria_no_274_2013.pdf is Outside DocumentTotals, but gives error on validation, moved 4.4.4.17 to DocumentTotals to be valid in validation, may be a error in 130823_Portaria_no_274_2013.pdf

                    //Currency
                    if (_defaultCurrency.Acronym != row.Values[xPSelectData.GetFieldIndexFromName("CurrencyCode")].ToString())
                    {
                        currencyAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("CurrencyAmount")]);
                        exchangeRate = totalLineResult.GrossTotal / currencyAmount;
                        //<Currency>
                        _xmlWriter.WriteStartElement("Currency");
                        WriteElement("CurrencyCode", row.Values[xPSelectData.GetFieldIndexFromName("CurrencyCode")].ToString());
                        WriteElement("CurrencyAmount", DataConversionUtils.DecimalToString(currencyAmount, _decimalFormat));
                        //In SAT-F Example we have 2 examples one with decimals 0.00 and other with 0.00000000000 opted to use divide value without conversion
                        WriteElement("ExchangeRate", DataConversionUtils.DecimalToString(exchangeRate, _decimalFormat));
                        //WriteElement("ExchangeRate", exchangeRate);
                        _xmlWriter.WriteEndElement();
                        //</Currency>
                    }

                    _xmlWriter.WriteEndElement();
                    //</DocumentTotals>

                    //</Payment>
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static TotalLinesResult SourceDocuments_Payments_Lines(Guid pFinanceDocumentPayment)
        {
            //<Line>
            //    <LineNumber>1</LineNumber>
            //    <SourceDocumentID>
            //        <OriginatingON>1T 1/6</OriginatingON>
            //        <InvoiceDate>2008-10-21</InvoiceDate>
            //        <Description>Bens à taxa normal de IVA</Description>
            //    </SourceDocumentID>
            //    <CreditAmount>5000</CreditAmount>
            //    <Tax>
            //        <TaxType>IVA</TaxType>
            //        <TaxCountryRegion>PT</TaxCountryRegion>
            //        <TaxCode>ISE</TaxCode>
            //        <TaxPercentage>0</TaxPercentage>
            //    </Tax>
            //    <TaxExemptionReason>Al... do n.º... do DL nº...</TaxExemptionReason>
            //    <!-- Added for 1.04_01-->
            //    <TaxExemptionCode>M05</TaxExemptionCode>
            //</Line>

            try
            {
                TotalLinesResult totalLineResult = new TotalLinesResult();

                string SqlQuery = @"
                    SELECT 
	                    fmaOid,
	                    fmpOrd AS LineNumber,	
	                    fmaDocumentNumber AS OriginatingON,
	                    fmaDocumentDate AS InvoiceDate,
	                    fmaNotes AS Description,
	                    fmaTotalTax AS TaxPayable,
	                    fmaTotalGross AS NetTotal,
	                    fmaTotalFinal AS GrossTotal,
                        fmpCreditAmount AS CreditAmount,
                        fmpDebitAmount AS DebitAmount
                    FROM 
                        view_documentfinancepayment
                    WHERE 
                        fpaOid = '{0}'
                    ORDER BY
                        fmpOrd,ftpCode,fmaDocumentNumber
                    ;
                ";

                string sql = string.Format(SqlQuery, pFinanceDocumentPayment.ToString());
                //_logger.Debug(string.Format("sql: [{0}]", sql));

                bool isCredit = false;
                string nodeNameCreditOrDebitAmount;
                decimal lineTaxPayable = 0.0m;
                decimal lineNetTotal = 0.0m;
                decimal lineGrossTotal = 0.0m;
                //helper to get current percentage amount of documentMaster based on CreditAmount
                decimal percentage = 0.0m;
                decimal lineCreditAmount = 0.0m;

                SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(sql);
                if (xPSelectData.DataRows.Length > 0)
                    foreach (SelectStatementResultRow row in xPSelectData.DataRows)
                    {
                        isCredit = (Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("CreditAmount")]) > 0);
                        nodeNameCreditOrDebitAmount = (isCredit) ? "CreditAmount" : "DebitAmount";

                        lineTaxPayable = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("TaxPayable")]), CultureSettings.DecimalRoundTo);
                        lineNetTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("NetTotal")]), CultureSettings.DecimalRoundTo);
                        lineGrossTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("GrossTotal")]), CultureSettings.DecimalRoundTo);

                        //Only add to Total if is Credit
                        if (isCredit)
                        {
                            //Helper percentage
                            lineCreditAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName("CreditAmount")]);
                            percentage = lineCreditAmount * 100 / lineGrossTotal;
                            //Sum Document totalDocument Percentage
                            totalLineResult.TaxPayable += (lineTaxPayable * percentage) / 100;
                            totalLineResult.NetTotal += (lineNetTotal * percentage) / 100;
                            totalLineResult.GrossTotal += (lineGrossTotal * percentage) / 100;
                        }

                        //<Line>
                        _xmlWriter.WriteStartElement("Line");
                        WriteElement("LineNumber", row.Values[xPSelectData.GetFieldIndexFromName("LineNumber")]);
                        //<SourceDocumentID>
                        _xmlWriter.WriteStartElement("SourceDocumentID");
                        WriteElement("OriginatingON", row.Values[xPSelectData.GetFieldIndexFromName("OriginatingON")]);
                        WriteElement("InvoiceDate", row.Values[xPSelectData.GetFieldIndexFromName("InvoiceDate")]);
                        WriteElement("Description", row.Values[xPSelectData.GetFieldIndexFromName("Description")]);
                        _xmlWriter.WriteEndElement();
                        //</Line>
                        //CreditAmount|DebitAmount
                        WriteElement(nodeNameCreditOrDebitAmount, DataConversionUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndexFromName(nodeNameCreditOrDebitAmount)]), _decimalFormat));

                        //TODO : Nos recibos do sistema de IVA de Caixa, deve ser indicada uma linha por cada taxa de IVA diferente, que conste da fatura respetiva.
                        //<Tax>
                        //_xmlWriter.WriteStartElement("Tax");
                        //_xmlWriter.WriteEndElement();
                        //<Tax>
                        _xmlWriter.WriteEndElement();
                        //</Line>
                    }
                return totalLineResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Xml Elements Helper Functions
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private static void WriteElement(string pElementName, object pElementValue)
        {
            WriteElement(pElementName, pElementValue, string.Empty);
        }

        private static void WriteElement(string pElementName, object pElementValue, string pElementValueIfNull)
        {
            try
            {
                if ((pElementValue != null && (pElementValue as string) != string.Empty) || pElementValueIfNull != string.Empty)
                {
                    string elementValue = (pElementValue != null && pElementValue.ToString() != string.Empty) ? pElementValue.ToString() : pElementValueIfNull;
                    _xmlWriter.WriteStartElement(pElementName);
                    _xmlWriter.WriteString(elementValue);
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}