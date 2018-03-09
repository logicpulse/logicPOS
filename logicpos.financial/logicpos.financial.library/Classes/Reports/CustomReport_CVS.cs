using FastReport;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.financial.library.Classes.Reports.BOs.Articles;
using logicpos.financial.library.Classes.Reports.BOs.Customers;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using logicpos.financial.library.Classes.Reports.BOs.System;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace logicpos.financial.library.Classes.Reports
{
    public class CustomReport : Report
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _debug = false;

        //Constructor Parameters
        string _reportFileName = String.Empty;
        //Other
        private bool _addCodeData = true;
        //Other Static
        private static bool _secondCopy;
        // Used Only in DocumentFinance Documents to Show Document Hash
        private string _hash4Chars = String.Empty;
        public string Hash4Chars
        {
            get { return _hash4Chars; }
            set { _hash4Chars = value; }
        }

        //FastReports Required Parameterless Constructor Else NULL Exception Occurs
        public CustomReport() { }
        public CustomReport(string pReportFileName, int pPrintCopies) : this(pReportFileName, null, pPrintCopies) { }
        public CustomReport(string pReportFileName, List<int> pCopyNames) : this(pReportFileName, pCopyNames, 1) { }
        public CustomReport(string pReportFileName, List<int> pCopyNames, int pPrintCopies)
        {
            //Assign Parameters
            _reportFileName = pReportFileName;

            if (_debug) _log.Debug("CustomReports: begin:" + _reportFileName);

            // If not in Debug mode use Stream reports from PluginSoftwareVendor else use local file location, usefulll to Develop Reports
            // This Workis without deleting temporary files equired for prevuiew and design loop
            List<string> tempReports = new List<string>();
            if (!Debugger.IsAttached)
            {
                // Get Protected temporary Reports
                tempReports = GlobalFramework.PluginSoftwareVendor.GetReportFileName(SettingsApp.SecretKey, _reportFileName);
                // Override Reports FileName
                _reportFileName = tempReports[0];

            }

            //First Load File report
            if (File.Exists(_reportFileName))
            {
                if (_debug) _log.Debug("CustomReports: loading :" + _reportFileName);

                // Load Report File
                this.Load(_reportFileName);

                // Delete temporary reports after Load it
                for (int i = 0; i < tempReports.Count; i++)
                {
                    if (File.Exists(tempReports[i]))
                    {
                        File.Delete(tempReports[i]);
                    }
                }
            }

            //Add NonUser Overlay, Like Certification, Licence etc, one first Page
            this.StartReport += delegate { if (_addCodeData) AddCodeData(); };

            //WIP EnvironmentSettings: Put This to Work
            //EnvironmentSettings environmentSettings = new EnvironmentSettings();
            //environmentSettings.ReportSettings.ShowProgress = false;
            //environmentSettings.UIStyle = UIStyle.Office2007Black;

            //WIP Styles: Put This to Work
            //<Style Name="NEW STYLE" Border.Lines="All" Fill.Color="DarkGray" TextFill.Color="Maroon" Font="Arial, 9.75pt, style=Bold"/>
            //Style style = new Style() {Name = "NEW STYLE"};
            //style.Border.Lines =  BorderLines.All;
            //this.Styles.Add(style);

            //Working but Commented
            //Load Script from External File
            //StreamReader streamReader = new StreamReader(@"Resources\Reports\Scripts\ReportDocumentFinance.cs");
            //string scriptText = streamReader.ReadToEnd();
            //streamReader.Close();
            //this.ScriptText = scriptText;

            //Add Assemblies
            string[] referencedAssemblies = this.ReferencedAssemblies;
            //Add logicPos reference to use methods, only if wotk in logicpos mode vs lax/web mode
            string filenameLogicPos = "logicpos.exe";
            if (File.Exists(filenameLogicPos))
            {
                Array.Resize(ref referencedAssemblies, this.ReferencedAssemblies.Length + 1);
                referencedAssemblies[referencedAssemblies.Length - 1] = "logicpos.exe";
            }
            this.ReferencedAssemblies = referencedAssemblies;

            if (_debug)
            {
                for (int i = 0; i < referencedAssemblies.Length; i++)
                {
                    _log.Debug("CustomReports: Load:" + referencedAssemblies[i]);
                }
                if (_debug) _log.Debug("CustomReports: Environment.CurrentDirectory:" + Environment.CurrentDirectory);
            }

            //ReportInfo Author
            //this.ReportInfo.Author = string.Format("{0} {1}", GlobalFramework.Settings["appName"], FrameworkUtils.ProductVersion);

            //PrintSettings CopyNames
            if (pCopyNames != null)
            {
                string[] copyNamesArray = CopyNames(pCopyNames);
                this.PrintSettings.CopyNames = copyNamesArray;
            }

            //Number Of Copies to Print
            this.PrintSettings.Copies = pPrintCopies;

            //Parameters Specific to Report
            //this.SetParameterValue("Report Title", "Parameter Report Title");
            //this.SetParameterValue("Report SubTitle", "Parameter Report SubTitle");

            //SystemVariables
            object[] systemVariables = this.Dictionary.SystemVariables.ToArray();
            //Add SystemVars
            FastReport.Data.SystemVariable systemVariable;
            foreach (var item in GlobalFramework.FastReportSystemVars)
            {
                systemVariable = new FastReport.Data.SystemVariable();
                systemVariable.Name = item.Key;
                systemVariable.AsString = item.Value;
                this.Dictionary.SystemVariables.Add(systemVariable);
            }

            //Custom Vars that was not Assigned on Startup
            if (GlobalFramework.LoggedUser != null)
            {
                GlobalFramework.FastReportCustomVars["Session_Logged_User"] = GlobalFramework.LoggedUser.Name;
            }
        }

        public string Process(CustomReportDisplayMode pViewMode, string pDestinationFileName = "")
        {
            string result = String.Empty;

            //Prepare Modes
            switch (pViewMode)
            {
                case CustomReportDisplayMode.Preview:
                case CustomReportDisplayMode.Print:
                case CustomReportDisplayMode.ExportPDF:
                case CustomReportDisplayMode.ExportPDFSilent:
                    //Get Object Reference to Change CopyName
                    TextObject textCopyName = (TextObject)this.FindObject("TextCopyName");
                    //Get Object Reference to Change SecondPrint Label for DocumentFinanceDocuments
                    TextObject textSecondPrint = (TextObject)this.FindObject("TextSecondPrint");

                    //Loop Copies and Change CopyName
                    for (int i = 0; i < this.PrintSettings.CopyNames.Length; i++)
                    {
                        if (textCopyName != null)
                        {
                            textCopyName.Text = this.PrintSettings.CopyNames[i];
                        }
                        if (textSecondPrint != null)
                        {
                            textSecondPrint.Text = (_secondCopy && i < 1) ? Resx.global_print_second_print : String.Empty;
                        }
                        //Store PreparedFiles in Custom SystemVariable, Required to PageNo in Reports ex "[ToInt32([PreparedPages]) + [Page]]"
                        //Else Page start aways in 1, when we call prepare, and we cannot have a usefull Page Counter working with .Prepare
                        this.Dictionary.SystemVariables.FindByName("PreparedPages").Value = (this.PreparedPages != null) ? this.PreparedPages.Count : 0;
                        //Call Report Prepare
                        this.Prepare(true);
                    }
                    break;
            }

            //NOT USED ANYMORE : Now we can Reset Copies to 1
            //this.PrintSettings.Copies = 1;

            //Send to ViewMode
            switch (pViewMode)
            {
                case CustomReportDisplayMode.Preview:
                    this.ShowPrepared();
                    break;
                case CustomReportDisplayMode.Print:
                    this.PrintPrepared();
                    break;
                case CustomReportDisplayMode.Design:
                    this.Design();
                    break;
                case CustomReportDisplayMode.ExportPDF:
                case CustomReportDisplayMode.ExportPDFSilent:
                    //Prepare FileName
                    string fileName = String.Empty;
                    if (pDestinationFileName != String.Empty)
                    {
                        fileName = pDestinationFileName;
                    }
                    //Default Filename
                    else
                    {
                        string dateTimeFileFormat = SettingsApp.FileFormatDateTime;
                        string dateTime = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
                        string reportName = (this.ReportInfo.Name != String.Empty) ? string.Format("_{0}", this.ReportInfo.Name) : String.Empty;
                        fileName = string.Format("print_{0}{1}{2}", dateTime, reportName, ".pdf");
                        fileName = fileName.Replace('/', '-').Replace(' ', '_');
                        //2015-06-12 apmuga
                        fileName = FrameworkUtils.OSSlash(string.Format(@"{0}{1}", GlobalFramework.Path["temp"], fileName));
                        //Mario
                        //fileName = (GlobalFramework.Settings["AppEnvironment"].ToUpper() == "web".ToUpper()) 
                        //    ? FrameworkUtils.OSSlash(string.Format(@"{0}{1}", GlobalFramework.Path["temp"], fileName))
                        //    : FrameworkUtils.OSSlash(string.Format(@"{0}\{1}{2}", Environment.CurrentDirectory, GlobalFramework.Path["temp"], fileName))
                        //;
                    }
                    FastReport.Export.Pdf.PDFExport export = new FastReport.Export.Pdf.PDFExport();
                    //if (export.ShowDialog()) report.Export(export, fileName);
                    try
                    {
                        this.Export(export, fileName);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }

                    //Show Pdf 
                    if (pViewMode == CustomReportDisplayMode.ExportPDF && File.Exists(fileName))
                    {
                        if (GlobalFramework.CanOpenFiles)
                        {
                            //Use full Path, keep untoutched fileName for result
                            System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName)));
                        }
                    }
                    result = fileName;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Add/Overlay Application Data Over Report
        /// </summary>
        public void AddCodeData()
        {
            PageFooterBand pageFooterBand = (PageFooterBand)this.FindObject("PageFooter1");
            // create title text
            TextObject textObjectOverlaySoftwareCertification = new TextObject();
            textObjectOverlaySoftwareCertification.Parent = pageFooterBand;
            textObjectOverlaySoftwareCertification.CreateUniqueName();
            //textObjectOverlaySoftwareCertification.Bounds = new RectangleF(Units.Centimeters, 0, Units.Centimeters * 10, Units.Centimeters * 1);

            //All Finance Documents use Processed, else Payments that use Emmited 
            string prefix = (_reportFileName.EndsWith("ReportDocumentFinancePayment.frx"))
                ? Resx.global_report_overlay_software_certification_emitted
                : Resx.global_report_overlay_software_certification_processed
            ;

            //Processed|Emitted with certified Software Nº {0}/AT - Copyright {1} - Licenced to a {2} - Used only if System Country is Portugal
            if (SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal &&
                (
                    _reportFileName.Contains("ReportDocumentFinance.frx") ||
                    _reportFileName.Contains("ReportDocumentFinancePayment.frx") ||
                    _reportFileName.Contains("ReportDocumentFinanceWayBill.frx")
                )
            )
            {
                textObjectOverlaySoftwareCertification.Text = string.Format(
                    Resx.global_report_overlay_software_certification,
                    prefix,
                    SettingsApp.SaftSoftwareCertificateNumber,
                    SettingsApp.SaftProductID,
                    GlobalFramework.LicenceCompany);

                //Add Hash Validation if Defined (In DocumentFinance Only)
                if (_hash4Chars != String.Empty) textObjectOverlaySoftwareCertification.Text = string.Format("{0} - {1}", _hash4Chars, textObjectOverlaySoftwareCertification.Text);
            };

            //Finnally Add Overlay
            textObjectOverlaySoftwareCertification.ZOrder = 1;
            textObjectOverlaySoftwareCertification.Left = 0.0f;
            textObjectOverlaySoftwareCertification.Top = 5.0f;
            textObjectOverlaySoftwareCertification.Width = 718.2f;
            textObjectOverlaySoftwareCertification.Height = 18.9f;
            textObjectOverlaySoftwareCertification.HorzAlign = HorzAlign.Center;
            textObjectOverlaySoftwareCertification.Font = new Font("Arial", 8, FontStyle.Bold);

            //Assign _addCodeData to true to prevent repeat add CodeData
            _addCodeData = false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessReportFinanceDocument - All FinanceMasterDocuments - Except Payments

        public static string ProcessReportFinanceDocument(CustomReportDisplayMode pViewMode, Guid pDocumentFinanceMasterOid, string pHash4Chars, List<int> pCopyNames, string pDestinationFileName = "")
        {
            return ProcessReportFinanceDocument(pViewMode, pDocumentFinanceMasterOid, pHash4Chars, pCopyNames, false, String.Empty, pDestinationFileName);
        }

        public static string ProcessReportFinanceDocument(CustomReportDisplayMode pViewMode, Guid pDocumentFinanceMasterOid, string pHash4Chars, List<int> pCopyNames, bool pSecondCopy, string pMotive, string pDestinationFileName = "")
        {
            string result = String.Empty;

            try
            {
                //TODO: Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                //Get DocumentFinanceMaster
                FIN_DocumentFinanceMaster documentMaster = (FIN_DocumentFinanceMaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), pDocumentFinanceMasterOid);

                // Build Final Filename Report
                string fileName = (documentMaster.DocumentType.WayBill) ? "ReportDocumentFinanceWayBill.frx" : "ReportDocumentFinance.frx";
                string fileUserReportDocumentFinance = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", fileName));

                CustomReport customReport = new CustomReport(fileUserReportDocumentFinance, pCopyNames);
                customReport.DoublePass = (documentMaster.DocumentDetail.Count > SettingsApp.CustomReportReportDocumentFinanceMaxDetail);
                customReport.Hash4Chars = pHash4Chars;
                //Report Parameters
                //customReport.SetParameterValue("Invoice Noº", 280);

                //Get Result Objects from FRBOHelper
                ResultFRBODocumentFinanceMaster fRBOHelperResponseProcessReportFinanceDocument = FRBOHelper.GetFRBOFinanceDocument(pDocumentFinanceMasterOid);
                //Get Generic Collections From FRBOHelper Results
                FRBOGenericCollection<FRBODocumentFinanceMasterView> gcDocumentFinanceMaster = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster;

                //Prepare and Enable DataSources
                customReport.RegisterData(gcDocumentFinanceMaster, "DocumentFinanceMaster");
                if (customReport.GetDataSource("DocumentFinanceMaster") != null) customReport.GetDataSource("DocumentFinanceMaster").Enabled = true;
                if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail").Enabled = true;
                if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal").Enabled = true;

                //Scripts - Dont Delete this Comment, may be usefull if we remove Script from Report File
                //Print X Records per Data Band
                //FastReport.DataBand dataBand = (DataBand) customReport.FindObject("Data1");
                //int dataBandRec = 1;
                ////Used to Break Page on X Recs, Usefull to leave open space for Report Summary
                //int dataBandMaxRecs = 30;
                //dataBand.AfterPrint += delegate {
                //  Console.WriteLine(string.Format("dataBandRec.RowNo:[{0}], dataBandMaxRecs:[{1}], , dataBand.RowNo[{2}], report.Pages.Count[{3}]", dataBandRec, dataBandMaxRecs, dataBand.RowNo, report.Pages.Count));
                //  if (dataBandRec == dataBandMaxRecs) { 
                //    dataBandRec = 1;  
                //    dataBand.StartNewPage = true; 
                //  } 
                //  else 
                //  { 
                //    dataBandRec++;
                //    dataBand.StartNewPage = false; 
                //  };
                //};

                //Assign Second Copy Reference
                _secondCopy = pSecondCopy;

                //Add ReportInfo.Name, to be used for Ex in Pdf Filenames, OS etc
                customReport.ReportInfo.Name = gcDocumentFinanceMaster.List[0].DocumentNumber;
                result = customReport.Process(pViewMode, pDestinationFileName);
                customReport.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessReportFinanceDocumentPayment

        public static string ProcessReportFinanceDocumentPayment(CustomReportDisplayMode pViewMode, Guid pDocumentFinancePaymentOid, List<int> pCopyNames, string pDestinationFileName = "")
        {
            string result = String.Empty;

            try
            {
                string fileUserReportDocumentFinancePayment = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", "ReportDocumentFinancePayment.frx"));
                CustomReport customReport = new CustomReport(fileUserReportDocumentFinancePayment, pCopyNames);

                //Get Result Objects from FRBOHelper
                ResultFRBODocumentFinancePayment fRBOHelperResponseProcessReportFinancePayment = FRBOHelper.GetFRBOFinancePayment(pDocumentFinancePaymentOid);
                //Get Generic Collections From FRBOHelper Results
                FRBOGenericCollection<FRBODocumentFinancePaymentView> gcDocumentFinancePayment = fRBOHelperResponseProcessReportFinancePayment.DocumentFinancePayment;

                //Prepare and Enable DataSources
                customReport.RegisterData(gcDocumentFinancePayment, "DocumentFinancePayment");
                if (customReport.GetDataSource("DocumentFinancePayment") != null) customReport.GetDataSource("DocumentFinancePayment").Enabled = true;
                if (customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument") != null) customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument").Enabled = true;

                //Add ReportInfo.Name, to be used for Ex in Pdf Filenames, OS etc
                customReport.ReportInfo.Name = gcDocumentFinancePayment.List[0].PaymentRefNo;
                result = customReport.Process(pViewMode, pDestinationFileName);
                customReport.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static

        // report_label_list_family_subfamily_articles : Relatório - Familias, SubFamilias e Artigos
        public static void ProcessReportArticle(CustomReportDisplayMode pViewMode)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportArticleList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", Resx.report_list_family_subfamily_articles);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                //"Oid,Designation,ButtonLabel"
                FRBOGenericCollection<FRBOArticleFamily> gcArticleFamily = new FRBOGenericCollection<FRBOArticleFamily>();
                FRBOGenericCollection<FRBOArticleSubFamily> gcArticleSubFamily;
                FRBOGenericCollection<FRBOArticle> gcArticle;

                //Render Child Bussiness Objects
                foreach (FRBOArticleFamily family in gcArticleFamily)
                {
                    //Get SubFamily
                    gcArticleSubFamily = new FRBOGenericCollection<FRBOArticleSubFamily>(string.Format("Family = '{0}'", family.Oid), "Ord");
                    family.ArticleSubFamily = gcArticleSubFamily.List;

                    //Get Articles
                    foreach (FRBOArticleSubFamily subFamily in family.ArticleSubFamily)
                    {
                        gcArticle = new FRBOGenericCollection<FRBOArticle>(string.Format("SubFamily = '{0}'", subFamily.Oid), "Ord");
                        subFamily.Article = gcArticle.List;
                    }
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcArticleFamily, "ArticleFamily");
                if (customReport.GetDataSource("ArticleFamily") != null) customReport.GetDataSource("ArticleFamily").Enabled = true;
                if (customReport.GetDataSource("ArticleFamily.ArticleSubFamily") != null) customReport.GetDataSource("ArticleFamily.ArticleSubFamily").Enabled = true;
                if (customReport.GetDataSource("ArticleFamily.ArticleSubFamily.Article") != null) customReport.GetDataSource("ArticleFamily.ArticleSubFamily.Article").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        // report_label_list_customers : Relatório - Clientes
        public static void ProcessReportCustomer(CustomReportDisplayMode pViewMode)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportCustomerList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", Resx.report_list_customers);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                //"Oid,Designation,ButtonLabel"
                FRBOGenericCollection<FRBOCustomerType> gcCustomerType = new FRBOGenericCollection<FRBOCustomerType>();
                FRBOGenericCollection<FRBOCustomer> gcCustomer;

                //Render Child Bussiness Objects
                foreach (FRBOCustomerType customerType in gcCustomerType)
                {
                    //Get SubFamily
                    gcCustomer = new FRBOGenericCollection<FRBOCustomer>(string.Format("CustomerType = '{0}'", customerType.Oid), "Ord");
                    customerType.Customer = gcCustomer.List;
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcCustomerType, "CustomerType");
                if (customReport.GetDataSource("CustomerType") != null) customReport.GetDataSource("CustomerType").Enabled = true;
                if (customReport.GetDataSource("CustomerType.Customer") != null) customReport.GetDataSource("CustomerType.Customer").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportArticleStockMovement(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportArticleStockMovementList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", Resx.report_list_stock_movements);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBOArticleStockMovementView> gcArticleStockMovement = new FRBOGenericCollection<FRBOArticleStockMovementView>(filter);

                //Prepare and Enable DataSources
                customReport.RegisterData(gcArticleStockMovement, "ArticleStockMovement");
                if (customReport.GetDataSource("ArticleStockMovement") != null) customReport.GetDataSource("ArticleStockMovement").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportSystemAudit(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportSystemAuditList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", Resx.report_list_audit_table);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBOSystemAuditView> gcSystemAudit = new FRBOGenericCollection<FRBOSystemAuditView>(filter);

                //Prepare and Enable DataSources
                customReport.RegisterData(gcSystemAudit, "SystemAudit");
                if (customReport.GetDataSource("SystemAudit") != null) customReport.GetDataSource("SystemAudit").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportDocumentMasterList(CustomReportDisplayMode pViewMode, string resourceString, string groupCondition, string groupTitle)
        {
            ProcessReportDocumentMasterList(pViewMode, resourceString, groupCondition, groupTitle, null, null);
        }

        public static void ProcessReportDocumentMasterList(CustomReportDisplayMode pViewMode, string reportTitle, string groupCondition, string groupTitle, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportDocumentFinanceMasterList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", reportTitle);
                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                // Get Objects
                GroupHeaderBand groupHeaderBand = (GroupHeaderBand)customReport.FindObject("GroupHeader1");
                TextObject groupHeaderBandText = (TextObject)customReport.FindObject("TextGroupHeader1");
                if (groupHeaderBand != null && groupHeaderBandText != null)
                {
                    groupHeaderBand.Condition = groupCondition;
                    groupHeaderBandText.Text = groupTitle;
                }
                else
                {
                    _log.Error("Error cant find Report Objects");
                }

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinanceMaster> gcDocumentFinanceMaster = new FRBOGenericCollection<FRBODocumentFinanceMaster>(filter);

                //Prepare and Enable DataSources
                customReport.RegisterData(gcDocumentFinanceMaster, "DocumentFinanceMaster");
                if (customReport.GetDataSource("DocumentFinanceMaster") != null) customReport.GetDataSource("DocumentFinanceMaster").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportDocumentDetail(CustomReportDisplayMode pViewMode, string resourceString, string groupField, string groupCondition, string groupTitle, bool grouped)
        {
            ProcessReportDocumentDetail(pViewMode, resourceString, groupField, groupCondition, groupTitle, null, null, grouped);
        }

        public static void ProcessReportDocumentDetail(CustomReportDisplayMode pViewMode, string resourceString, string groupField, string groupCondition, string groupTitle, string filter, string filterHumanReadable, bool grouped)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportDocumentFinanceDetailList.frx");
                CustomReport customReport = new CustomReport(reportFile, 1);

                // Add PostFix to Report Title 
                Tuple<string, string> tuppleResourceString = GetResourceString(resourceString);
                string reportTitleString = tuppleResourceString.Item1;
                string reportTitleStringPostfix = tuppleResourceString.Item2;

                //Report Parameters
                customReport.SetParameterValue("Report Title", String.Format("{0}{1}", reportTitleString, reportTitleStringPostfix));
                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                // Get Objects
                GroupHeaderBand groupHeaderBand = (GroupHeaderBand)customReport.FindObject("GroupHeader1");
                TextObject groupHeaderBandText = (TextObject)customReport.FindObject("TextGroupHeader1");
                if (groupHeaderBand != null && groupHeaderBandText != null)
                {
                    groupHeaderBand.Condition = groupCondition;
                    groupHeaderBandText.Text = groupTitle;
                }
                else
                {
                    _log.Error("Error cant find Report Objects");
                }

                //Prepare and Declare FRBOGenericCollections for non grouped and gouped reports
                if (!grouped)
                {
                    // Using view_documentfinance
                    FRBOGenericCollection<FRBODocumentFinanceMasterDetailView> gcDocumentFinanceMasterDetail = new FRBOGenericCollection<FRBODocumentFinanceMasterDetailView>(filter);
                    //Prepare and Enable DataSources
                    customReport.RegisterData(gcDocumentFinanceMasterDetail, "DocumentFinanceDetail");
                }
                else
                {
                    // Using view_documentfinancesellgroup
                    FRBOGenericCollection<FRBODocumentFinanceMasterDetailGroupView> gcDocumentFinanceMasterDetail = new FRBOGenericCollection<FRBODocumentFinanceMasterDetailGroupView>(filter, groupField, string.Empty);
                    //Prepare and Enable DataSources
                    customReport.RegisterData(gcDocumentFinanceMasterDetail, "DocumentFinanceDetail");

                    DataBand dataBandData3 = (DataBand)customReport.FindObject("Data3");
                    TextObject textColumnDateHeader = (TextObject)customReport.FindObject("TextColumnDateHeader");
                    TextObject textColumnDateData = (TextObject)customReport.FindObject("TextColumnDateData");
                    TextObject textColumnCodeHeader = (TextObject)customReport.FindObject("TextColumnCodeHeader");
                    TextObject textColumnCodeData = (TextObject)customReport.FindObject("TextColumnCodeData");
                    TextObject textColumnDesignationHeader = (TextObject)customReport.FindObject("TextColumnDesignationHeader");
                    TextObject textColumnDesignationData = (TextObject)customReport.FindObject("textColumnDesignationData");
                    // Change Objects
                    if (dataBandData3 != null && groupHeaderBandText != null && textColumnDateData != null && textColumnCodeHeader != null && textColumnCodeData != null && textColumnDesignationHeader != null && textColumnDesignationData != null)
                    {
                        // Remove [DocumentFinanceDetail.Date] and use sencon Sort has Priority
                        dataBandData3.Sort[0].Expression = dataBandData3.Sort[1].Expression;
                        dataBandData3.Sort[1].Expression = string.Empty;
                        // Date : Disable non exitence fields [DocumentFinanceDetail.Date] in sort and firts colum data
                        textColumnDateHeader.Delete();
                        textColumnDateData.Delete();
                        // Code
                        textColumnCodeHeader.Left = 0;
                        textColumnCodeData.Left = 0;
                        textColumnDesignationHeader.Left = 56.7F;
                        textColumnDesignationData.Left = 56.7F;
                        textColumnDesignationHeader.Width = 292.95F;
                        textColumnDesignationData.Width = 292.95F;
                    }
                    else
                    {
                        _log.Error("Error cant find Report Objects");
                    }
                }

                if (customReport.GetDataSource("DocumentFinanceDetail") != null) customReport.GetDataSource("DocumentFinanceDetail").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Generate DocumentMaster PDFs

        public static string DocumentMasterCreatePDF(FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            //return DocumentMasterCreatePDF(pDocumentFinanceMaster, String.Empty, CustomReportDisplayMode.ExportPDFSilent);
            return DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinanceMaster, String.Empty);
        }

        public static string DocumentMasterCreatePDF(FIN_DocumentFinanceMaster pDocumentFinanceMaster, string pDestinationFileName)
        {
            //return DocumentMasterCreatePDF(pDocumentFinanceMaster, pDestinationFileName, CustomReportDisplayMode.ExportPDFSilent);
            return DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinanceMaster, pDestinationFileName);
        }

        //public static string DocumentMasterCreatePDF(DocumentFinanceMaster pDocumentFinanceMaster, string pDestinationFileName, CustomReportDisplayMode pCustomReportDisplayMode)
        public static string DocumentMasterCreatePDF(CustomReportDisplayMode pViewMode, FIN_DocumentFinanceMaster pDocumentFinanceMaster, string pDestinationFileName)
        {
            string result = String.Empty;
            try
            {
                //Generate Default CopyNames from DocumentType
                List<int> copyNames = CopyNames(pDocumentFinanceMaster.DocumentType.PrintCopies);
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(pDocumentFinanceMaster.Hash);
                result = ProcessReportFinanceDocument(pViewMode, pDocumentFinanceMaster.Oid, hash4Chars, copyNames, pDestinationFileName);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Generate FinanceDocumentPayment PDFs

        public static string DocumentPaymentCreatePDF(FIN_DocumentFinancePayment pDocumentFinancePayment)
        {
            return DocumentPaymentCreatePDF(pDocumentFinancePayment, String.Empty, CustomReportDisplayMode.ExportPDFSilent);
        }

        public static string DocumentPaymentCreatePDF(FIN_DocumentFinancePayment pDocumentFinancePayment, string pDestinationFileName)
        {
            return DocumentPaymentCreatePDF(pDocumentFinancePayment, pDestinationFileName, CustomReportDisplayMode.ExportPDFSilent);
        }

        public static string DocumentPaymentCreatePDF(FIN_DocumentFinancePayment pDocumentFinancePayment, string pDestinationFileName, CustomReportDisplayMode pCustomReportDisplayMode)
        {
            string result = String.Empty;
            try
            {
                //Generate Default CopyNames from DocumentType
                List<int> copyNames = CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
                result = ProcessReportFinanceDocumentPayment(pCustomReportDisplayMode, pDocumentFinancePayment.Oid, copyNames, pDestinationFileName);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //CopyNames

        //Get CopyNames String Array from integer List ex pCopyNames[1] = result[1] = Duplicate
        public static string[] CopyNames(List<int> pCopyNames)
        {
            string[] result = new string[pCopyNames.Count];
            for (int i = 0; i < pCopyNames.Count; i++)
            {
                result[i] = Resx.ResourceManager.GetString(string.Format("global_print_copy_title{0}", pCopyNames[i] + 1));
            }
            return result;
        }

        //Get CopyNames Int List from PrintCopies ex pPrintCopies = 2 | result[0] = Original, result[1] = Duplicate
        public static List<int> CopyNames(int pPrintCopies)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < pPrintCopies; i++)
            {
                result.Add(i);
            }
            return result;
        }

        //Get CopyNames Delimted String from Generic List
        public static string CopyNamesCommaDelimited(List<int> pCopyNames)
        {
            string result = String.Empty;

            try
            {
                if (pCopyNames.Count > 0)
                {
                    result = FrameworkUtils.StringListToCommaDelimitedString<int>(pCopyNames, ',');
                }
            }
            catch (Exception ex)
            {

                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private static string GetReportFilePath(string fileName)
        {
            string result = string.Empty;

            try
            {
                result = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", fileName));
                if (!File.Exists(result)) return string.Empty;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Shared method to get Common ResourceString for all 3 types of Financial Reports

        public static Tuple<string, string> GetResourceString(string reportResourceString)
        {
            string resourceString = string.Empty;
            string resourceStringPostfix = string.Empty;

            try
            {
                // Reuse Same resource String for 3 Sales Reports Default(Resx) report-sales-*, report-sales-detail-*, report-sales-detail-group-
                // Detail/Group
                if (reportResourceString.StartsWith("report_sales_detail_group_"))
                {//be first
                 // Remove extra chars from token to re use default resx
                    resourceString = reportResourceString.Replace("report_sales_detail_group_", "report_sales_");
                    resourceStringPostfix = string.Format(" {0}", Resx.report_sales_detail_group_postfix);
                }
                // Detail
                else if (reportResourceString.StartsWith("report_sales_detail_"))
                {
                    // Remove extra chars from token to re use default resx
                    resourceString = reportResourceString.Replace("report_sales_detail_", "report_sales_");
                    resourceStringPostfix = string.Format(" {0}", Resx.report_sales_detail_postfix);
                }
                // Default
                else
                {
                    resourceString = reportResourceString;
                    resourceStringPostfix = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            // Get Resource Content for all modes
            if ((!string.IsNullOrEmpty(Resx.ResourceManager.GetString(resourceString))))
            {
                resourceString = Resx.ResourceManager.GetString(resourceString);
            }
            else
            {
                resourceString = string.Format("Error: Can't find resourceString:[{0}]", resourceString);
                _log.Error(resourceString);
            }

            return new Tuple<string, string>(resourceString, resourceStringPostfix);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //OLD TEST Report Code Before FRBOGenericCollection
        //public static void ProcessReport()
        //{
        //  try
        //  {
        //    string fileUserReportDocumentFinance = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", "ReportDocumentFinance.frx"));
        //    CustomReport customReport = new CustomReport(fileUserReportDocumentFinance);
        //    List<FRBOArticleFamily> businessObject = FRBOArticleFamily.GetList();

        //    //Render Child Bussiness Objects
        //    foreach (FRBOArticleFamily family in businessObject)
        //    {
        //      family.ArticleSubFamily = FRBOArticleSubFamily.GetList(string.Format("Family = '{0}'", family.Oid));

        //      foreach (FRBOArticleSubFamily subfamily in family.ArticleSubFamily)
        //      {
        //        subfamily.Article = FRBOArticle.GetList(string.Format("SubFamily = '{0}'", subfamily.Oid));
        //      }
        //    }

        //    customReport.RegisterData(businessObject, "Family");
        //    customReport.GetDataSource("Family").Enabled = true;
        //    customReport.GetDataSource("Family.SubFamily").Enabled = true;
        //    customReport.GetDataSource("Family.SubFamily.Article").Enabled = true;

        //    //customReport.Design();
        //    customReport.Show();
        //    customReport.Dispose();
        //  }
        //  catch (Exception ex)
        //  {
        //    _log.Error(ex.Message, ex);
        //  }
        //}
    }
}