using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.resources.Resources.Localization;
using System;
using System.Text;
using System.Xml;

//Notes Tests in DocumentFinanceDialogPage7

namespace logicpos.financial.library.Classes.Finance
{
    //Notes: SourceID and CustomerID required CodeInternal (30 Chars Key), View Customer and UserDetail XPOObject/CodeInternal Notes
    //Notes: IN SAF-T PT the TotalFinal correspond to GrossTotal (Total do documento com impostos (GrossTotal))

    public class SaftPt
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private static XmlWriter _xmlWriter;
        private static DateTime _currentDate;

        //Filter DocumentFinanceMaster
        private static DateTime _documentDateStart;
        private static DateTime _documentDateEnd;

        //Settings
        private static string _dateTimeFormatDocumentDate = SettingsApp.DateTimeFormatDocumentDate;
        //Custom Number Format
        private static string _decimalFormat = SettingsApp.DecimalFormatSAFTPT;
        private static string _decimalFormatTotals = SettingsApp.DecimalFormatGrossTotalSAFTPT;
        //Default Customer
        private static ERP_Customer _defaultCustomer = (ERP_Customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(ERP_Customer), SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);
        //Default Currency
        private static CFG_ConfigurationCurrency _defaultCurrency = SettingsApp.ConfigurationSystemCurrency;

        public static string ExportSaftPt()
        {
            //DEVELOPER : Assign pastMonths=0 to Work in Curent Month Range, Else it Works in Past Month by Default (-1)
            int pastMonths = 0;

            //TODO: Move to Filter Date Dialog
            DateTime workingDate = FrameworkUtils.CurrentDateTimeAtomic().AddMonths(-pastMonths);
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

            _currentDate = FrameworkUtils.CurrentDateTimeAtomic();

            //Settings
            string fileSaftPT = SettingsApp.FileFormatSaftPT;
            string dateTimeFileFormat = SettingsApp.FileFormatDateTime;
            string dateTime = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
            string fileName = GlobalFramework.Path["saftpt"] + string.Format(fileSaftPT, SettingsApp.SaftVersionPrefix, SettingsApp.SaftVersion, dateTime).ToLower();

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
                    string standardAuditFileTax = string.Format("{0}_{1}", SettingsApp.SaftVersionPrefix, SettingsApp.SaftVersion);
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
                FrameworkUtils.Audit("EXPORT_SAF-T", string.Format(Resx.audit_message_export_saft, fileName, _documentDateStart.ToString(SettingsApp.DateFormat), _documentDateEnd.ToString(SettingsApp.DateFormat)));

                return fileName;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                WriteElement("AuditFileVersion", SettingsApp.SaftVersion);
                string companyID = string.Format("{0} {1}"
                    , GlobalFramework.PreferenceParameters["COMPANY_CIVIL_REGISTRATION"].Replace(' ', '_')
                    , GlobalFramework.PreferenceParameters["COMPANY_CIVIL_REGISTRATION_ID"].Replace(' ', '_')
                );
                WriteElement("CompanyID", companyID);
                WriteElement("TaxRegistrationNumber", GlobalFramework.PreferenceParameters["COMPANY_FISCALNUMBER"]);
                WriteElement("TaxAccountingBasis", SettingsApp.TaxAccountingBasis);
                WriteElement("CompanyName", GlobalFramework.PreferenceParameters["COMPANY_NAME"]);
                WriteElement("BusinessName", GlobalFramework.PreferenceParameters["COMPANY_BUSINESS_NAME"]);

                //<CompanyAddress>
                _xmlWriter.WriteStartElement("CompanyAddress");
                WriteElement("AddressDetail", GlobalFramework.PreferenceParameters["COMPANY_ADDRESS"]);
                WriteElement("City", GlobalFramework.PreferenceParameters["COMPANY_CITY"]);
                WriteElement("PostalCode", GlobalFramework.PreferenceParameters["COMPANY_POSTALCODE"]);
                WriteElement("Region", GlobalFramework.PreferenceParameters["COMPANY_REGION"]);
                WriteElement("Country", GlobalFramework.PreferenceParameters["COMPANY_COUNTRY_CODE2"]);
                _xmlWriter.WriteEndElement();
                //</CompanyAddress>

                WriteElement("FiscalYear", _currentDate.Year);
                WriteElement("StartDate", _documentDateStart.ToString(_dateTimeFormatDocumentDate));
                WriteElement("EndDate", _documentDateEnd.ToString(_dateTimeFormatDocumentDate));
                WriteElement("CurrencyCode", SettingsApp.SaftCurrencyCode);
                WriteElement("DateCreated", _currentDate.ToString(_dateTimeFormatDocumentDate));
                WriteElement("TaxEntity", GlobalFramework.PreferenceParameters["COMPANY_TAX_ENTITY"]);
                WriteElement("ProductCompanyTaxID", SettingsApp.SaftProductCompanyTaxID);
                WriteElement("SoftwareCertificateNumber", SettingsApp.SaftSoftwareCertificateNumber);
                WriteElement("ProductID", SettingsApp.SaftProductID);
                WriteElement("ProductVersion", FrameworkUtils.ProductVersion);
                //WriteElement("HeaderComment", "Comentários ao SAFT exportado");
                WriteElement("Telephone", GlobalFramework.PreferenceParameters["COMPANY_TELEPHONE"]);
                WriteElement("Fax", GlobalFramework.PreferenceParameters["COMPANY_FAX"]);
                WriteElement("Email", GlobalFramework.PreferenceParameters["COMPANY_EMAIL"]);
                WriteElement("Website", GlobalFramework.PreferenceParameters["COMPANY_WEBSITE"]);

                //</Header>
                _xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<GeneralLedgerEntries>

        private static void GeneralLedgerEntries()
        {
            try
            {
                //<GeneralLedgerEntries>
                _xmlWriter.WriteStartElement("GeneralLedgerEntries");

                //...

                //</GeneralLedgerEntries>
                _xmlWriter.WriteEndElement();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><GeneralLedger>

        //<GeneralLedger> Plano oficial de contas com todas as contas tanto as agregadoras como as de movimento 
        private static void MasterFiles_GeneralLedger()
        {
            try
            {
                //<GeneralLedger>
                _xmlWriter.WriteStartElement("GeneralLedger");

                //...

                //</GeneralLedger>
                _xmlWriter.WriteEndElement();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><Supplier>

        private static void MasterFiles_Supplier()
        {
            try
            {
                //<Supplier>
                _xmlWriter.WriteStartElement("Supplier");

                //...

                //</Supplier>
                _xmlWriter.WriteEndElement();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<MasterFiles><Customer>

        private static void MasterFiles_Customer()
        {
            // Local Variables to store Values
            string customerTaxID = string.Empty;
            string companyName = string.Empty;
            string addressDetail = string.Empty;
            string city = string.Empty;
            string postalCode = string.Empty;
            string telephone = string.Empty;
            string fax = string.Empty;
            string email = string.Empty;
            string website = string.Empty;

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
	                    fin_documentfinancemaster as fm
	                    left join erp_customer cu ON (fm.EntityOid = cu.Oid)
	                    left join cfg_configurationcountry cc ON (cu.Country = cc.Oid)
                    WHERE 
                        cu.Oid IS NOT NULL 
                        AND (fm.Date >= '{0}' AND fm.Date <= '{1}')
                    GROUP BY 
	                    cu.CodeInternal, cu.FiscalNumber, cu.Name, cu.Address, cu.City, cu.ZipCode, cc.Code2, cu.Phone, cu.Fax, cu.Email, cu.Website
                    ORDER BY
	                    cu.Name
                    ;"
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                );
                //_log.Debug(string.Format("sql: [{0}]", sql));

                //Used to Add Default Customer if not in Query, Required to Always have a Default Customer for ex to Documents that Donta Have a Customer (NULL), like Conference Documents, etc
                MasterFiles_Customer_DefaultCustomer();

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    // Decrypt Properties
                    if (GlobalFramework.PluginSoftwareVendor != null)
                    {
                        customerTaxID = (row.Values[xPSelectData.GetFieldIndex("CustomerTaxID")] != null) 
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("CustomerTaxID")].ToString())
                            : null
                            ;
                        companyName = (row.Values[xPSelectData.GetFieldIndex("CompanyName")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("CompanyName")].ToString())
                            : null;
                        addressDetail = (row.Values[xPSelectData.GetFieldIndex("AddressDetail")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("AddressDetail")].ToString())
                            : null;
                        city = (row.Values[xPSelectData.GetFieldIndex("City")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("City")].ToString())
                            : null;
                        postalCode =(row.Values[xPSelectData.GetFieldIndex("PostalCode")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("PostalCode")].ToString())
                            : null;
                        telephone = (row.Values[xPSelectData.GetFieldIndex("Telephone")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("Telephone")].ToString())
                            : null;
                        fax = (row.Values[xPSelectData.GetFieldIndex("Fax")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("Fax")].ToString())
                            : null;
                        email = (row.Values[xPSelectData.GetFieldIndex("Email")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("Email")].ToString())
                            : null;
                        website = (row.Values[xPSelectData.GetFieldIndex("Website")] != null)
                            ? GlobalFramework.PluginSoftwareVendor.Decrypt(row.Values[xPSelectData.GetFieldIndex("Website")].ToString())
                            : null;
                    }
                    // Dont Decrypt Properties, use Normal Values
                    else
                    {
                        customerTaxID = (row.Values[xPSelectData.GetFieldIndex("")] != null) 
                            ? row.Values[xPSelectData.GetFieldIndex("CustomerTaxID")].ToString()
                            : null;
                        companyName = (row.Values[xPSelectData.GetFieldIndex("")] != null)
                            ? row.Values[xPSelectData.GetFieldIndex("CompanyName")].ToString()
                            : null;
                        addressDetail = row.Values[xPSelectData.GetFieldIndex("AddressDetail")].ToString();
                        city = row.Values[xPSelectData.GetFieldIndex("City")].ToString();
                        postalCode = row.Values[xPSelectData.GetFieldIndex("PostalCode")].ToString();
                        telephone = row.Values[xPSelectData.GetFieldIndex("Telephone")].ToString();
                        fax = row.Values[xPSelectData.GetFieldIndex("Fax")].ToString();
                        email = row.Values[xPSelectData.GetFieldIndex("Email")].ToString();
                        website = row.Values[xPSelectData.GetFieldIndex("Website")].ToString();
                    }

                    //<Customer>
                    _xmlWriter.WriteStartElement("Customer");
                    if (row.Values[xPSelectData.GetFieldIndex("CustomerID")] != null)
                    {
                        WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndex("CustomerID")]);
                    }
                    else
                    {
                        WriteElement("CustomerID", _defaultCustomer.CodeInternal);
                    }
                    WriteElement("AccountID", row.Values[xPSelectData.GetFieldIndex("AccountID")], Resx.saft_value_unknown);
WriteElement("CustomerTaxID", customerTaxID, Resx.saft_value_unknown);
WriteElement("CompanyName", companyName, Resx.saft_value_unknown);
                    //<BillingAddress>
                    _xmlWriter.WriteStartElement("BillingAddress");
WriteElement("AddressDetail", addressDetail, Resx.saft_value_unknown);
WriteElement("City", city, Resx.saft_value_unknown);
WriteElement("PostalCode", postalCode, Resx.saft_value_unknown);
                    WriteElement("Country", row.Values[xPSelectData.GetFieldIndex("Country")], Resx.saft_value_unknown);
                    _xmlWriter.WriteEndElement();
                    //</BillingAddress>
WriteElement("Telephone", telephone);
WriteElement("Fax", fax);
WriteElement("Email", email);
WriteElement("Website", website);
                    WriteElement("SelfBillingIndicator", 0);
                    //</Customer>
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                , SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity
            );
            //_log.Debug(string.Format("sqlCheckDefaultCustomer: [{0}]", sqlCheckDefaultCustomer));

            //<NumberOfEntries>
            object customerCount = GlobalFramework.SessionXpo.ExecuteScalar(sqlCheckDefaultCustomer);

            //RETURN if have Default Customer
            if (Convert.ToInt16(customerCount) > 0) return;

            //Else Add Dafault Customer
            //<Customer>
            _xmlWriter.WriteStartElement("Customer");
            WriteElement("CustomerID", _defaultCustomer.CodeInternal);
            WriteElement("AccountID", Resx.saft_value_unknown);
            WriteElement("CustomerTaxID", _defaultCustomer.FiscalNumber);
            WriteElement("CompanyName", _defaultCustomer.Name, Resx.saft_value_unknown);
            //<BillingAddress>
            _xmlWriter.WriteStartElement("BillingAddress");
            WriteElement("AddressDetail", _defaultCustomer.Address, Resx.saft_value_unknown);
            WriteElement("City", _defaultCustomer.City, Resx.saft_value_unknown);
            WriteElement("PostalCode", _defaultCustomer.ZipCode, Resx.saft_value_unknown);
            WriteElement("Country", _defaultCustomer.Country.Code2, Resx.saft_value_unknown);
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
                    "
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                );
                //_log.Debug(string.Format("sql: [{0}]", sql));

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    //<Product>
                    _xmlWriter.WriteStartElement("Product");
                    WriteElement("ProductType", row.Values[xPSelectData.GetFieldIndex("ProductType")]);
                    WriteElement("ProductCode", row.Values[xPSelectData.GetFieldIndex("ProductCode")].ToString());
                    //Utilizado o descritivo da tabela “Familias”.
                    WriteElement("ProductGroup", row.Values[xPSelectData.GetFieldIndex("ProductGroup")]);
                    WriteElement("ProductDescription", row.Values[xPSelectData.GetFieldIndex("ProductDescription")]);
                    //Código EAN. Deve ser utilizado o código EAN (código de barras) do produto. Quando este não existir, preencher com o valor do campo “Identificador do Produto” 
                    string productNumberCodeField = (row.Values[xPSelectData.GetFieldIndex("ProductNumberCode")] != null) ? "ProductNumberCode" : "ProductCode";
                    WriteElement("ProductNumberCode", row.Values[xPSelectData.GetFieldIndex(productNumberCodeField)]);
                    //</Product>
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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

                string sql = @"
                    SELECT 
                        TaxType,
                        TaxCode,
                        TaxCountryRegion,
                        TaxDescription AS Description,
                        Value AS TaxPercentage
                    FROM 
                        fin_configurationvatrate 
                    ORDER BY 
                        Ord;
                ";
                //_log.Debug(string.Format("sql: [{0}]", sql));

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    //<TaxTableEntry>
                    _xmlWriter.WriteStartElement("TaxTableEntry");
                    WriteElement("TaxType", row.Values[xPSelectData.GetFieldIndex("TaxType")]);
                    WriteElement("TaxCountryRegion", row.Values[xPSelectData.GetFieldIndex("TaxCountryRegion")]);
                    WriteElement("TaxCode", row.Values[xPSelectData.GetFieldIndex("TaxCode")]);
                    WriteElement("Description", row.Values[xPSelectData.GetFieldIndex("Description")]);
                    WriteElement("TaxPercentage", FrameworkUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("TaxPercentage")]), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                    _xmlWriter.WriteEndElement();
                    //</TaxTableEntry>
                }

                //</TaxTable>
                _xmlWriter.WriteEndElement();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                _log.Error(ex.Message, ex);
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
                _log.Error(ex.Message, ex);
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
                _log.Error(ex.Message, ex);
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
            string documentNodeName = String.Empty;
            string documentNodeNameChild = String.Empty;
            string documentNodeNameChildNo = String.Empty;
            string documentNodeKeyWord = String.Empty;
            string documentNodeFilter = String.Empty;
            string documentNodeFilterTotalControl = String.Empty;

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
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                    , documentNodeFilter
                );
                //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlNumberOfEntries: [{1}]", pSaftDocumentType, sqlNumberOfEntries));

                //<NumberOfEntries>
                object numberOfEntries = GlobalFramework.SessionXpo.ExecuteScalar(sqlNumberOfEntries);

                //RETURN if we dont have any Documents/NumberOfEntries
                if (Convert.ToInt16(numberOfEntries) <= 0) return;

                //<SalesInvoices|MovementOfGoods|WorkingDocuments>
                _xmlWriter.WriteStartElement(documentNodeName);

                switch (pSaftDocumentType)
                {
                    case SaftDocumentType.SalesInvoices:
                    case SaftDocumentType.WorkingDocuments:

                        WriteElement("NumberOfEntries", numberOfEntries);

                        string sqlTotalDebitTotalCredit = string.Format(@"
                            SELECT 
                                SUM(fdTotalGross) AS Total
                            FROM 
                                view_documentfinance
                            WHERE 
                                ftSaftAuditFile = 1 AND ftCredit = {3} 
                                AND (fmDate >= '{0}' AND fmDate <= '{1}')
                                AND {2} 
                            "
                            , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                            , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                            //, documentNodeFilter.Replace("ft.", "ft").Replace("fm.", "fm")
                            , documentNodeFilterTotalControl.Replace("ft.", "ft").Replace("fm.", "fm")
                            , "{0}"
                         );

                        //<TotalDebit>
                        string sqlTotalDebit = string.Format(sqlTotalDebitTotalCredit, 0);
                        //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalDebit: [{1}]", pSaftDocumentType, sqlTotalDebit));

                        object totalDebit = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalDebit);
                        if (totalDebit == null) totalDebit = 0;
                        WriteElement("TotalDebit", FrameworkUtils.DecimalToString(Convert.ToDecimal(totalDebit), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                        //<TotalCredit>
                        string sqlTotalCredit = string.Format(sqlTotalDebitTotalCredit, 1);
                        //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalCredit: [{1}]", pSaftDocumentType, sqlTotalCredit));
                        object totalCredit = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalCredit);
                        if (totalCredit == null) totalCredit = 0;
                        WriteElement("TotalCredit", FrameworkUtils.DecimalToString(Convert.ToDecimal(totalCredit), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

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
                            , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                            , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                            , documentNodeFilterTotalControl.Replace("ft.", "ft").Replace("fm.", "fm")
                            , "{0}"
                         );

                        //<TotalDebit>
                        string sqlTotalQuantity = string.Format(sqlTotalQuantityIssued, 0);
                        //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalQuantityIssued: [{1}]", pSaftDocumentType, sqlTotalQuantityIssued));

                        object totalQuantity = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalQuantity);
                        if (totalQuantity == null) totalDebit = 0;

                        WriteElement("NumberOfMovementLines", numberOfEntries);
                        WriteElement("TotalQuantityIssued", totalQuantity); //2015-01-20 apmuga verificar se é soma de kg+lt+m

                        //<Invoice|StockMovement|WorkDocument>
                        SourceDocuments_DocumentType_Childs(pSaftDocumentType, documentNodeKeyWord, documentNodeNameChild, documentNodeNameChildNo, documentNodeFilter);
                        //</Invoice|StockMovement|WorkDocument>

                        _xmlWriter.WriteEndElement();
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                        0 AS ATCUD
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
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                    , pdocumentNodeFilter
                );
                //_log.Debug(string.Format("SaftDocumentType:[{0}] sql: [{1}]", pSaftDocumentType, sql));

                Guid guidDocumentMaster;
                //Declare Vars for Currency/ExchangeRate
                decimal documentExchangeRate = 0.0m;
                decimal currencyCurrencyAmount = 0.0m;
                decimal currencyExchangeRate = 0.0m;
                //Declare Var to Store Acronym Type for Replace OR and FP with DC (Acronym)
                string documentType = string.Empty;
                bool wayBill = false;

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    //<Invoice|StockMovement|WorkDocument>
                    _xmlWriter.WriteStartElement(pDocumentNodeNameChild);
                    WriteElement(pDocumentNodeNameChildNo, row.Values[xPSelectData.GetFieldIndex("DocumentNo")]);
                    WriteElement("ATCUD", row.Values[xPSelectData.GetFieldIndex("ATCUD")]);
                    //<DocumentStatus>
                    _xmlWriter.WriteStartElement("DocumentStatus");
                    WriteElement(string.Format("{0}Status", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndex("DocumentStatusStatus")]);
                    WriteElement(string.Format("{0}StatusDate", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndex("DocumentStatusDate")]);
                    WriteElement("Reason", row.Values[xPSelectData.GetFieldIndex("DocumentStatusReason")]);
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceIDStatus")].ToString());
                    WriteElement("SourceBilling", row.Values[xPSelectData.GetFieldIndex("SourceBilling")]);
                    _xmlWriter.WriteEndElement();
                    //</DocumentStatus>
                    WriteElement("Hash", row.Values[xPSelectData.GetFieldIndex("Hash")]);
                    WriteElement("HashControl", row.Values[xPSelectData.GetFieldIndex("HashControl")]);
                    WriteElement("Period", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]).Month);
                    WriteElement(string.Format("{0}Date", pDocumentNodeKeyWord), row.Values[xPSelectData.GetFieldIndex("DocumentDate")]);
                    //Required to Replace WorkType OR and FP with DC
                    documentType = row.Values[xPSelectData.GetFieldIndex("DocumentType")].ToString();
                    //Replace DocumentType if Detect a ConferenceDocument (OR|FP|DC)
                    if (documentType == "OR" || documentType == "FP") documentType = "DC";
                    //Detect if DocumentType is WayBill
                    wayBill = Convert.ToBoolean(row.Values[xPSelectData.GetFieldIndex("WayBill")]);

                    //Write Element
                    WriteElement(string.Format("{0}Type", pDocumentNodeKeyWord), documentType);

                    switch (pSaftDocumentType)
                    {
                        case SaftDocumentType.SalesInvoices:
                            try
                            {
                                //<SpecialRegimes>
                                _xmlWriter.WriteStartElement("SpecialRegimes");
                                WriteElement("SelfBillingIndicator", row.Values[xPSelectData.GetFieldIndex("SelfBillingIndicator")]);
                                WriteElement("CashVATSchemeIndicator", row.Values[xPSelectData.GetFieldIndex("CashVatSchemeIndicator")]);
                                WriteElement("ThirdPartiesBillingIndicator", row.Values[xPSelectData.GetFieldIndex("ThirdPartiesBillingIndicator")]);
                                _xmlWriter.WriteEndElement();
                                //</SpecialRegimes> 

                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndex("EACCode")]);
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]);
                                //WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndex("TransactionID")]);
                                if (row.Values[xPSelectData.GetFieldIndex("CustomerID")] != null)
                                {
                                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndex("CustomerID")]);
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
                                _log.Error(ex.Message, ex);
                            };
                            break;

                        case SaftDocumentType.MovementOfGoods:
                            try
                            {
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]);
                                WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndex("TransactionID")]);
                                WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndex("CustomerID")].ToString());
                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndex("EACCode")]);

                                //Allways Call ShipDetails Helper to Output ShipTo|ShipFrom Details
                                SourceDocuments_DocumentType_Childs_ShipDetails(xPSelectData, row);

                                //4.2.3.19: Used only in MovementOfGoods: Código de identificação atribuído pela AT ao documento, nos termos do Decreto - Lei n.º 147/2003, de 11 de julho.
                                WriteElement("ATDocCodeID", row.Values[xPSelectData.GetFieldIndex("ATDocCodeID")]);
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex.Message, ex);
                            }
                            break;

                        case SaftDocumentType.WorkingDocuments:
                            try
                            {
                                WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceIDCreator")].ToString());
                                WriteElement("EACCode", row.Values[xPSelectData.GetFieldIndex("EACCode")]);
                                WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]);
                                if (row.Values[xPSelectData.GetFieldIndex("CustomerID")] != null)
                                {
                                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndex("CustomerID")]);
                                }
                                else
                                {
                                    WriteElement("CustomerID", _defaultCustomer.CodeInternal);
                                }
                            }
                            catch (Exception ex)
                            {
                                _log.Error(ex.Message, ex);
                            }
                            break;
                    }

                    //<Line>
                    guidDocumentMaster = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString());
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
                    WriteElement("TaxPayable", FrameworkUtils.DecimalToString(totalLineResult.TaxPayable, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));
                    WriteElement("NetTotal", FrameworkUtils.DecimalToString(totalLineResult.NetTotal, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));
                    WriteElement("GrossTotal", FrameworkUtils.DecimalToString(totalLineResult.GrossTotal, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));

                    //Currency
                    if (_defaultCurrency.Acronym != row.Values[xPSelectData.GetFieldIndex("CurrencyCode")].ToString())
                    {
                        //Calculate Totals
                        documentExchangeRate = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("ExchangeRate")]);
                        currencyCurrencyAmount = totalLineResult.GrossTotal * documentExchangeRate;
                        currencyExchangeRate = totalLineResult.GrossTotal / currencyCurrencyAmount;
                        //<Currency>
                        _xmlWriter.WriteStartElement("Currency");
                        WriteElement("CurrencyCode", row.Values[xPSelectData.GetFieldIndex("CurrencyCode")].ToString());
                        WriteElement("CurrencyAmount", FrameworkUtils.DecimalToString(currencyCurrencyAmount, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                        //In SAT-F Example we have 2 examples one with decimals 0.00 and other with 0.00000000000 opted to use divide value without conversion
                        WriteElement("ExchangeRate", FrameworkUtils.DecimalToString(currencyExchangeRate, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                        //WriteElement("ExchangeRate", currencyExchangeRate);
                        _xmlWriter.WriteEndElement();
                        //</Currency>
                    }

                    //</SalesInvoices|MovementOfGoods|WorkingDocuments>
                    switch (pSaftDocumentType)
                    {
                        case SaftDocumentType.SalesInvoices:
                            //Get decimal to check if Greater than Zero
                            decimal paymentAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("PaymentAmount")]);
                            //Only Export if Greater Than Zero (SAF-T Recomendation)
                            if (paymentAmount > 0.0m)
                            {
                                //<Payment>
                                _xmlWriter.WriteStartElement("Payment");
                                //Default : OU : OtherPayments /Outros Pagamentos
                                WriteElement("PaymentMechanism", row.Values[xPSelectData.GetFieldIndex("PaymentMechanism")], "OU");
                                WriteElement("PaymentAmount", FrameworkUtils.DecimalToString(paymentAmount, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                                WriteElement("PaymentDate", row.Values[xPSelectData.GetFieldIndex("DocumentDate")]);
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
                _log.Error(ex.Message, ex);
            }
        }

        //Helper to Output ShiptTo and ShipFrom Details, Shared for Invoice|StockMovement SaftDocumentType
        private static void SourceDocuments_DocumentType_Childs_ShipDetails(XPSelectData pXPSelectData, SelectStatementResultRow pRow)
        {
            //<ShipTo>
            _xmlWriter.WriteStartElement("ShipTo");
            WriteElement("DeliveryID", pRow.Values[pXPSelectData.GetFieldIndex("ShipToDeliveryID")]);
            WriteElement("DeliveryDate", FrameworkUtils.DateToString(pRow.Values[pXPSelectData.GetFieldIndex("ShipToDeliveryDate")]));
            WriteElement("WarehouseID", pRow.Values[pXPSelectData.GetFieldIndex("ShipToWarehouseID")]);
            WriteElement("LocationID", pRow.Values[pXPSelectData.GetFieldIndex("ShipToLocationID")]);
            //<Address>
            _xmlWriter.WriteStartElement("Address");
            WriteElement("BuildingNumber", pRow.Values[pXPSelectData.GetFieldIndex("ShipToBuildingNumber")]);
            WriteElement("StreetName", pRow.Values[pXPSelectData.GetFieldIndex("ShipToStreetName")]);
            WriteElement("AddressDetail", pRow.Values[pXPSelectData.GetFieldIndex("ShipToAddressDetail")]);
            WriteElement("City", pRow.Values[pXPSelectData.GetFieldIndex("ShipToCity")]);
            WriteElement("PostalCode", pRow.Values[pXPSelectData.GetFieldIndex("ShipToPostalCode")]);
            WriteElement("Region", pRow.Values[pXPSelectData.GetFieldIndex("ShipToRegion")]);
            WriteElement("Country", pRow.Values[pXPSelectData.GetFieldIndex("ShipToCountry")]);
            _xmlWriter.WriteEndElement();
            //</Address>
            _xmlWriter.WriteEndElement();
            //<ShipTo>

            //<ShipFrom>
            _xmlWriter.WriteStartElement("ShipFrom");
            WriteElement("DeliveryID", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromDeliveryID")]);
            WriteElement("DeliveryDate", FrameworkUtils.DateToString(pRow.Values[pXPSelectData.GetFieldIndex("ShipFromDeliveryDate")]));
            WriteElement("WarehouseID", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromWarehouseID")]);
            WriteElement("LocationID", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromLocationID")]);
            //<Address>
            _xmlWriter.WriteStartElement("Address");
            WriteElement("BuildingNumber", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromBuildingNumber")]);
            WriteElement("StreetName", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromStreetName")]);
            WriteElement("AddressDetail", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromAddressDetail")]);
            WriteElement("City", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromCity")]);
            WriteElement("PostalCode", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromPostalCode")]);
            WriteElement("Region", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromRegion")]);
            WriteElement("Country", pRow.Values[pXPSelectData.GetFieldIndex("ShipFromCountry")]);
            _xmlWriter.WriteEndElement();
            //</Address>
            _xmlWriter.WriteEndElement();
            //</ShipFrom>

            //Export if not Null else gives wrong values ex "0001-01-01T00:00:00" | Always Null, Its not persisted yet, but has stub code here to work when its not null
            if (pRow.Values[pXPSelectData.GetFieldIndex("MovementEndTime")] != null)
            {
                WriteElement("MovementEndTime", FrameworkUtils.DateTimeToCombinedDateTimeString(pRow.Values[pXPSelectData.GetFieldIndex("MovementEndTime")]));
            }
            WriteElement("MovementStartTime", FrameworkUtils.DateTimeToCombinedDateTimeString(pRow.Values[pXPSelectData.GetFieldIndex("MovementStartTime")]));
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
            //<Line>
            //  <LineNumber>1</LineNumber>
            //  <OrderReferences>
            //    <OriginatingON>GR 1/2</OriginatingON>
            //    <OrderDate>2008-09-15</OrderDate>
            //  </OrderReferences>
            //  <ProductCode>FAT03</ProductCode>
            //  <ProductDescription>Fato de algodão (Produção própria)</ProductDescription>
            //  <Quantity>1</Quantity>
            //  <UnitOfMeasure>UN</UnitOfMeasure>
            //  <UnitPrice>5000</UnitPrice>
            //  <TaxPointDate>2008-10-21</TaxPointDate>
            //  <References>
            //    <Reference>1T 1/6</Reference>
            //    <Reason>Não havia sido liquidado o valor da mão-de-obra</Reason>
            //  </References>
            //  <Description>Fato de algodão (Produção própria)</Description>
            //  <CreditAmount>5000</CreditAmount>
            //  <Tax>
            //    <TaxType>IVA</TaxType>
            //    <TaxCountryRegion>PT</TaxCountryRegion>
            //    <TaxCode>NOR</TaxCode>
            //    <TaxPercentage>20</TaxPercentage>
            //  </Tax>
            //</Line>

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
                //_log.Debug(string.Format("sql: [{0}]", sql));

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
                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                if (xPSelectData.Data.Length > 0)
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        nodeNameCreditOrDebitAmount = (Convert.ToInt16(row.Values[xPSelectData.GetFieldIndex("Credit")]) == 1) ? "CreditAmount" : "DebitAmount";

                        linePriceWithDiscount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("PriceWithDiscount")]);
                        globalDocumentDiscount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("GlobalDiscount")]); ;
                        //Remove Global Discount, this way UnitPrice dont have discounts (Line and Global) and dont have Taxs
                        lineUnitPrice = linePriceWithDiscount - ((linePriceWithDiscount * globalDocumentDiscount) / 100);
                        lineQuantity = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("Quantity")]);
                        lineCreditOrDebit = lineUnitPrice * lineQuantity;

                        lineTaxPayable = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("TaxPayable")]), SettingsApp.DecimalRoundTo);
                        lineNetTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("NetTotal")]), SettingsApp.DecimalRoundTo);
                        lineGrossTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("GrossTotal")]), SettingsApp.DecimalRoundTo);
                        totalLineResult.TaxPayable += lineTaxPayable;
                        totalLineResult.NetTotal += lineNetTotal;
                        totalLineResult.GrossTotal += lineGrossTotal;

                        //<Line>
                        _xmlWriter.WriteStartElement("Line");

                        WriteElement("LineNumber", row.Values[xPSelectData.GetFieldIndex("LineNumber")]);
                        switch (pSaftDocumentType)
                        {
                            case SaftDocumentType.SalesInvoices:
                            case SaftDocumentType.MovementOfGoods:
                            case SaftDocumentType.WorkingDocuments:
                                //<OrderReferences>
                                guidDocumentDetail = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString());
                                SourceDocuments_Lines_OrderReferences(guidDocumentDetail);
                                //</OrderReferences>
                                break;
                        }

                        WriteElement("ProductCode", row.Values[xPSelectData.GetFieldIndex("ProductCode")].ToString());
                        WriteElement("ProductDescription", row.Values[xPSelectData.GetFieldIndex("ProductDescription")]);
                        WriteElement("Quantity", FrameworkUtils.DecimalToString(lineQuantity, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                        WriteElement("UnitOfMeasure", row.Values[xPSelectData.GetFieldIndex("UnitOfMeasure")]);
                        WriteElement("UnitPrice", FrameworkUtils.DecimalToString(lineUnitPrice, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                        switch (pSaftDocumentType)
                        {
                            case SaftDocumentType.SalesInvoices:
                            case SaftDocumentType.WorkingDocuments:
                                //<TaxPointDate>
                                WriteElement("TaxPointDate", row.Values[xPSelectData.GetFieldIndex("TaxPointDate")]);
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

                        WriteElement("Description", row.Values[xPSelectData.GetFieldIndex("ProductDescription")]);
                        //CreditAmount|DebitAmount
                        WriteElement(nodeNameCreditOrDebitAmount, FrameworkUtils.DecimalToString(lineCreditOrDebit, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                        //<Tax>
                        _xmlWriter.WriteStartElement("Tax");
                        WriteElement("TaxType", row.Values[xPSelectData.GetFieldIndex("TaxType")]);
                        WriteElement("TaxCountryRegion", row.Values[xPSelectData.GetFieldIndex("TaxCountryRegion")]);
                        WriteElement("TaxCode", row.Values[xPSelectData.GetFieldIndex("TaxCode")]);
                        WriteElement("TaxPercentage", FrameworkUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("TaxPercentage")]), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                        _xmlWriter.WriteEndElement();
                        //</Tax>

                        WriteElement("TaxExemptionReason", row.Values[xPSelectData.GetFieldIndex("TaxExemptionReason")]);
                        WriteElement("TaxExemptionCode", row.Values[xPSelectData.GetFieldIndex("TaxExemptionCode")]);
                        if (Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("SettlementAmount")]) > 0.0m)
                            WriteElement("SettlementAmount", FrameworkUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("SettlementAmount")]), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                        _xmlWriter.WriteEndElement();
                        //</Line>
                    }
                return totalLineResult;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
            FIN_DocumentFinanceDetail documentFinanceDetail = (FIN_DocumentFinanceDetail)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceDetail), pDocumentMasterDetail);
            if (documentFinanceDetail.DocumentMaster.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
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
                    //_log.Debug(string.Format("sql: [{0}]", sql));

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        //<OrderReferences>
                        _xmlWriter.WriteStartElement("OrderReferences");
                        WriteElement("OriginatingON", row.Values[xPSelectData.GetFieldIndex("OriginatingON")]);
                        WriteElement("OrderDate", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndex("OrderDate")]).ToString(_dateTimeFormatDocumentDate));
                        _xmlWriter.WriteEndElement();
                        //</OrderReferences>
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
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
            FIN_DocumentFinanceDetail documentFinanceDetail = (FIN_DocumentFinanceDetail)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceDetail), pDocumentMasterDetail);
            if (documentFinanceDetail.DocumentMaster.DocumentType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
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
                    //_log.Debug(string.Format("sql: [{0}]", sql));

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        //<References>
                        _xmlWriter.WriteStartElement("References");
                        WriteElement("Reference", row.Values[xPSelectData.GetFieldIndex("Reference")]);
                        WriteElement("Reason", row.Values[xPSelectData.GetFieldIndex("Reason")]);
                        _xmlWriter.WriteEndElement();
                        //</References>
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
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
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                );
                //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlNumberOfEntries: [{1}]", SaftDocumentType.Payments, sqlNumberOfEntries));

                //<NumberOfEntries>
                object numberOfEntries = GlobalFramework.SessionXpo.ExecuteScalar(sqlNumberOfEntries);

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
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                    , "{0}"
                    );

                //<TotalDebit>
                string sqlTotalDebit = string.Format(sqlTotalDebitTotalCredit, "Debit");
                //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalDebit: [{1}]", SaftDocumentType.Payments, sqlTotalDebit));

                object totalDebit = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalDebit);
                if (totalDebit == null) totalDebit = 0;
                WriteElement("TotalDebit", FrameworkUtils.DecimalToString(Convert.ToDecimal(totalDebit), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                //<TotalCredit>
                string sqlTotalCredit = string.Format(sqlTotalDebitTotalCredit, "Credit");
                //_log.Debug(string.Format("SaftDocumentType:[{0}]: sqlTotalCredit: [{1}]", SaftDocumentType.Payments, sqlTotalCredit));
                object totalCredit = GlobalFramework.SessionXpo.ExecuteScalar(sqlTotalCredit);
                if (totalCredit == null) totalCredit = 0;
                WriteElement("TotalCredit", FrameworkUtils.DecimalToString(Convert.ToDecimal(totalCredit), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

                //<Payment>
                SourceDocuments_Payments_Childs();
                //</Payment>

                _xmlWriter.WriteEndElement();
                //</Payments>
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                    , _documentDateStart.ToString(SettingsApp.DateTimeFormat)
                    , _documentDateEnd.ToString(SettingsApp.DateTimeFormat)
                );
                //_log.Debug(string.Format("SaftDocumentType:[{0}] sql: [{1}]", SaftDocumentType.Payments, sql));

                Guid guidFinanceDocumentPayment;
                decimal currencyAmount;
                decimal exchangeRate;

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    //<Payment>
                    _xmlWriter.WriteStartElement("Payment");
                    WriteElement("PaymentRefNo", row.Values[xPSelectData.GetFieldIndex("PaymentRefNo")]);
                    WriteElement("ATCUD", row.Values[xPSelectData.GetFieldIndex("ATCUD")]);
                    WriteElement("Period", Convert.ToDateTime(row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]).Month);
                    WriteElement("TransactionID", row.Values[xPSelectData.GetFieldIndex("TransactionID")]);
                    WriteElement("TransactionDate", row.Values[xPSelectData.GetFieldIndex("TransactionDate")]);
                    WriteElement("PaymentType", row.Values[xPSelectData.GetFieldIndex("PaymentType")]);
                    WriteElement("Description", row.Values[xPSelectData.GetFieldIndex("Description")]);
                    //<DocumentStatus>
                    _xmlWriter.WriteStartElement("DocumentStatus");
                    WriteElement("PaymentStatus", row.Values[xPSelectData.GetFieldIndex("PaymentStatus")]);
                    WriteElement("PaymentStatusDate", row.Values[xPSelectData.GetFieldIndex("PaymentStatusDate")]);
                    WriteElement("Reason", row.Values[xPSelectData.GetFieldIndex("Reason")]);
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceID")]);
                    WriteElement("SourcePayment", row.Values[xPSelectData.GetFieldIndex("SourcePayment")]);
                    _xmlWriter.WriteEndElement();
                    //</DocumentStatus>
                    //<PaymentMethod>
                    _xmlWriter.WriteStartElement("PaymentMethod");
                    WriteElement("PaymentMechanism", row.Values[xPSelectData.GetFieldIndex("PaymentMechanism")]);
                    WriteElement("PaymentAmount", FrameworkUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("PaymentAmount")]), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                    WriteElement("PaymentDate", row.Values[xPSelectData.GetFieldIndex("PaymentDate")]);
                    _xmlWriter.WriteEndElement();
                    //</PaymentMethod>
                    WriteElement("SourceID", row.Values[xPSelectData.GetFieldIndex("SourceID")]);
                    WriteElement("SystemEntryDate", row.Values[xPSelectData.GetFieldIndex("SystemEntryDate")]);
                    WriteElement("CustomerID", row.Values[xPSelectData.GetFieldIndex("CustomerID")]);

                    //<Line>
                    guidFinanceDocumentPayment = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString());
                    TotalLinesResult totalLineResult = SourceDocuments_Payments_Lines(guidFinanceDocumentPayment);
                    //</Line>

                    //<DocumentTotals>
                    _xmlWriter.WriteStartElement("DocumentTotals");
                    WriteElement("TaxPayable", FrameworkUtils.DecimalToString(totalLineResult.TaxPayable, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));
                    WriteElement("NetTotal", FrameworkUtils.DecimalToString(totalLineResult.NetTotal, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));
                    WriteElement("GrossTotal", FrameworkUtils.DecimalToString(totalLineResult.GrossTotal, GlobalFramework.CurrentCultureNumberFormat, _decimalFormatTotals));

                    //Note: 4.4.4.17 in 130823_Portaria_no_274_2013.pdf is Outside DocumentTotals, but gives error on validation, moved 4.4.4.17 to DocumentTotals to be valid in validation, may be a error in 130823_Portaria_no_274_2013.pdf

                    //Currency
                    if (_defaultCurrency.Acronym != row.Values[xPSelectData.GetFieldIndex("CurrencyCode")].ToString())
                    {
                        currencyAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("CurrencyAmount")]);
                        exchangeRate = totalLineResult.GrossTotal / currencyAmount;
                        //<Currency>
                        _xmlWriter.WriteStartElement("Currency");
                        WriteElement("CurrencyCode", row.Values[xPSelectData.GetFieldIndex("CurrencyCode")].ToString());
                        WriteElement("CurrencyAmount", FrameworkUtils.DecimalToString(currencyAmount, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
                        //In SAT-F Example we have 2 examples one with decimals 0.00 and other with 0.00000000000 opted to use divide value without conversion
                        WriteElement("ExchangeRate", FrameworkUtils.DecimalToString(exchangeRate, GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));
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
                _log.Error(ex.Message, ex);
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
                //_log.Debug(string.Format("sql: [{0}]", sql));

                bool isCredit = false;
                string nodeNameCreditOrDebitAmount;
                decimal lineTaxPayable = 0.0m;
                decimal lineNetTotal = 0.0m;
                decimal lineGrossTotal = 0.0m;
                //helper to get current percentage amount of documentMaster based on CreditAmount
                decimal percentage = 0.0m;
                decimal lineCreditAmount = 0.0m;

                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                if (xPSelectData.Data.Length > 0)
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        isCredit = (Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("CreditAmount")]) > 0);
                        nodeNameCreditOrDebitAmount = (isCredit) ? "CreditAmount" : "DebitAmount";

                        lineTaxPayable = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("TaxPayable")]), SettingsApp.DecimalRoundTo);
                        lineNetTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("NetTotal")]), SettingsApp.DecimalRoundTo);
                        lineGrossTotal = Math.Round(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("GrossTotal")]), SettingsApp.DecimalRoundTo);

                        //Only add to Total if is Credit
                        if (isCredit)
                        {
                            //Helper percentage
                            lineCreditAmount = Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex("CreditAmount")]);
                            percentage = lineCreditAmount * 100 / lineGrossTotal;
                            //Sum Document totalDocument Percentage
                            totalLineResult.TaxPayable += (lineTaxPayable * percentage) / 100;
                            totalLineResult.NetTotal += (lineNetTotal * percentage) / 100;
                            totalLineResult.GrossTotal += (lineGrossTotal * percentage) / 100;
                        }

                        //<Line>
                        _xmlWriter.WriteStartElement("Line");
                        WriteElement("LineNumber", row.Values[xPSelectData.GetFieldIndex("LineNumber")]);
                        //<SourceDocumentID>
                        _xmlWriter.WriteStartElement("SourceDocumentID");
                        WriteElement("OriginatingON", row.Values[xPSelectData.GetFieldIndex("OriginatingON")]);
                        WriteElement("InvoiceDate", row.Values[xPSelectData.GetFieldIndex("InvoiceDate")]);
                        WriteElement("Description", row.Values[xPSelectData.GetFieldIndex("Description")]);
                        _xmlWriter.WriteEndElement();
                        //</Line>
                        //CreditAmount|DebitAmount
                        WriteElement(nodeNameCreditOrDebitAmount, FrameworkUtils.DecimalToString(Convert.ToDecimal(row.Values[xPSelectData.GetFieldIndex(nodeNameCreditOrDebitAmount)]), GlobalFramework.CurrentCultureNumberFormat, _decimalFormat));

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
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Xml Elements Helper Functions
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private static void WriteElement(string pElementName, object pElementValue)
        {
            WriteElement(pElementName, pElementValue, String.Empty);
        }

        private static void WriteElement(string pElementName, object pElementValue, string pElementValueIfNull)
        {
            try
            {
                if ((pElementValue != null && (pElementValue as string) != String.Empty) || pElementValueIfNull != String.Empty)
                {
                    string elementValue = (pElementValue != null && pElementValue.ToString() != String.Empty) ? pElementValue.ToString() : pElementValueIfNull;
                    _xmlWriter.WriteStartElement(pElementName);
                    _xmlWriter.WriteString(elementValue);
                    _xmlWriter.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
