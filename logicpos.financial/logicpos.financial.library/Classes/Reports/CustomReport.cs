﻿using FastReport;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.documentviewer;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Reports.BOs;
using logicpos.financial.library.Classes.Reports.BOs.Articles;
using logicpos.financial.library.Classes.Reports.BOs.Customers;
using logicpos.financial.library.Classes.Reports.BOs.Documents;
using logicpos.financial.library.Classes.Reports.BOs.System;
using logicpos.financial.library.Classes.Reports.BOs.Users;
using logicpos.resources;
using logicpos.shared.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Controls.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace logicpos.financial.library.Classes.Reports
{
    public class CustomReport : Report
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string FILENAME_TEMPLATE_BASE = "TemplateBase.frx";
        private const string FILENAME_TEMPLATE_BASE_SIMPLE = "TemplateBaseSimple.frx";
        private bool _debug = false;
        // Use this to force ReleaseMode and force use Embedded Reports Resources
        private bool _forceReleaseMode = false;

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
        public CustomReport(string pReportFileName, string pTemplateBase, int pPrintCopies) : this(pReportFileName, pTemplateBase, null, pPrintCopies) { }
        public CustomReport(string pReportFileName, string pTemplateBase, List<int> pCopyNames) : this(pReportFileName, pTemplateBase, pCopyNames, 1) { }
        public CustomReport(string pReportFileName, string pTemplateBase, List<int> pCopyNames, int pPrintCopies)
        {
            //Assign Parameters
            _reportFileName = pReportFileName;

            if (_debug) _log.Debug("CustomReports: begin:" + _reportFileName);

            // If not in Debug mode use Stream reports from PluginSoftwareVendor else use local file location, usefulll to Develop Reports
            // This Workis without deleting temporary files equired for prevuiew and design loop
            List<string> tempReports = new List<string>();
            if ((!Debugger.IsAttached || _forceReleaseMode) && !string.IsNullOrEmpty(pTemplateBase))
            {
                // Get Protected temporary Reports
                tempReports = GlobalFramework.PluginSoftwareVendor.GetReportFileName(SettingsApp.SecretKey, _reportFileName, pTemplateBase);
                // Override Default Reports FileName
                _reportFileName = tempReports[0];
            }

            //First Load File report
            if (File.Exists(_reportFileName))
            {
                if (_debug) _log.Debug("CustomReports: loading :" + _reportFileName);

                // Load Report File
                this.Load(_reportFileName);

                // Delete temporary reports after Load
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
                _log.Debug("CustomReports: Environment.CurrentDirectory:" + Environment.CurrentDirectory);
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
                case CustomReportDisplayMode.ExportXls:
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
                            textSecondPrint.Text = (_secondCopy && i < 1) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_print_second_print") : String.Empty;
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
                //Impressão A4 abria Janela de impressão Fast Report [IN:009341]
                //case CustomReportDisplayMode.Print:
                //    this.PrintPrepared();
                //    break;
                case CustomReportDisplayMode.Design:
                    this.Design();
                    break;
                case CustomReportDisplayMode.ExportPDF:
                case CustomReportDisplayMode.ExportPDFSilent:
                case CustomReportDisplayMode.ExportXls:
                case CustomReportDisplayMode.Print:
                    //Prepare FileName
                    string fileName = String.Empty;
                    string fileNameExport = String.Empty;
                    if (pDestinationFileName != String.Empty)
                    {
                        fileName = pDestinationFileName;
                        fileNameExport = pDestinationFileName;
                    }
                    //Default Filename
                    else
                    {
                        string dateTimeFileFormat = SettingsApp.FileFormatDateTime;
                        string dateTime = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
                        string reportName = (this.ReportInfo.Name != String.Empty) ? string.Format("_{0}", this.ReportInfo.Name) : String.Empty;

                        if (pViewMode == CustomReportDisplayMode.ExportXls || FrameworkUtils.OSVersion() == "windows")
                        {
                            fileNameExport = string.Format("print_{0}{1}{2}", dateTime, reportName, ".xlsx");
                            fileNameExport = fileNameExport.Replace('/', '-').Replace(' ', '_');
                            fileNameExport = FrameworkUtils.OSSlash(string.Format(@"{0}{1}", GlobalFramework.Path["temp"], fileNameExport));
                        }
                       
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

                    //TK016206 Reports - Exportação para Xls/pdf
					//TK016249 - Impressoras - Diferenciação entre Tipos
                    if ((pViewMode == CustomReportDisplayMode.ExportXls) || FrameworkUtils.OSVersion() == "windows" || !GlobalFramework.UsingThermalPrinter)
                    {
                        var exportXlsx = new FastReport.Export.OoXML.Excel2007Export();
                        //if (exportXlsx.ShowDialog())
                        //{
                            this.Export(exportXlsx, fileNameExport);
                        //}
                        if (FrameworkUtils.IsLinux)
                        {
                            System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileNameExport)));
                        }
                        
                    }

                    //if (export.ShowDialog()) this.Export(export, fileName);
                    //TK016206 Reports - Exportação para Xls/pdf
					//TK016249 - Impressoras - Diferenciação entre Tipos
                    if ((pViewMode == CustomReportDisplayMode.ExportPDFSilent) || (pViewMode == CustomReportDisplayMode.ExportPDF) || !GlobalFramework.UsingThermalPrinter)
					{
                        FastReport.Export.Pdf.PDFExport export = new FastReport.Export.Pdf.PDFExport();
                        try
                        {
                            this.Export(export, fileName);
                        }
                        catch (Exception ex)
                        {
                            _log.Error("string Process(CustomReportDisplayMode pViewMode, string pDestinationFileName) :: fileName [ " + fileName + " ]: " + ex.Message, ex);
                        }
                    }
                    //Show Printer Dialog on Windows
                    //Impressão A4 abria Janela de impressão Fast Report [IN:009341]
                    if ((pViewMode == CustomReportDisplayMode.Print) && File.Exists(fileName))
                    {
                        if (!FrameworkUtils.IsLinux)
                        {
                            string docPath = FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName));

                            var pdf = PdfDocument.Load(docPath, null, null);

                            var printDoc = new PdfPrintDocument(pdf);
                            var dlg = new PrintDialog();
                            dlg.AllowCurrentPage = true;
                            dlg.AllowSomePages = true;
                            dlg.UseEXDialog = true;
                            dlg.Document = printDoc;
                            //OnPdfPrinDocumentCreaded(new EventArgs<PdfPrintDocument>(printDoc));
                            //ShowPrintDialogDelegate showprintdialog = ShowPrintDialog;

                            //Initialize a new thread for print dialog
                            Thread thread3 = new Thread(() =>
                            {
                                if (dlg.ShowDialog() == DialogResult.OK)
                                {
                                    try
                                    {
                                        Application.Exit();
                                        dlg.Document.Print();
                                    }
                                    catch (Exception ex)
                                    {
                                    //Printing was canceled
                                }
                                }
                            });
                            thread3.SetApartmentState(ApartmentState.STA);
                            thread3.Start();
                            thread3.Join();
                        }
                        else { this.PrintPrepared(); }


                    }

                    //Show Pdf 
                    if ((pViewMode == CustomReportDisplayMode.ExportPDF) && File.Exists(fileName))
                    {
                        if (GlobalFramework.CanOpenFiles)
                        {
                            //IN009240 Usar o PDF Viewer do POS                                                      
                            

                            if (!FrameworkUtils.IsLinux && FrameworkUtils.UsePosPDFViewer() == true)
                            {
                                string docPath = FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName));
                                var ScreenSizePDF = GlobalFramework.screenSize;
                                int widthPDF = ScreenSizePDF.Width;
                                int heightPDF = ScreenSizePDF.Height;
                                bool exportXls = true;

                                if (!GlobalFramework.UsingThermalPrinter) exportXls = false;

                                System.Windows.Forms.Application.Run(new logicpos.documentviewer.startDocumentViewer(docPath, widthPDF-50, heightPDF-20, exportXls));
                            }
                            else
                            {
                                System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName)));
                            }
                            //Use full Path, keep untoutched fileName for result
                            //System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName)));
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

            /// <summary>
            ///     This change refers to "ENH201810#04".
            /// </summary>
            /// <remarks>
            ///     <para>DESCRIPTION: This code change covers MZ and AO invoice layout enhancement, based on "CurrentCulture" settings.</para>
            ///     <para>ISSUE: all prices greater than million were being cut on invoice.</para>
            ///     <para>CAUSE: there was no proper locale based invoice files.</para>
            ///     <para>SOLUTION: It was created a file for each of those specific locale settings, based on original files. 
            ///     For example: based on "ReportDocumentFinance.frx" it was created "ReportDocumentFinance_pt-MZ.frx".
            ///     </para>
            /// </remarks>
            /// <example>
            ///     "Preço" value: 35 000 000,00
            ///     Shows the value:  000 000,00
            /// </example>
            /// 
            string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
            try
            {
                if (CultureInfo.CurrentUICulture.Name != currentCulture)
                {
                }
            }
            catch
            {
                currentCulture = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
            }


            //string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
            //CultureInfo.CurrentUICulture = GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["cultureFinancialRules"]);

            if(currentCulture != "pt-AO" && currentCulture != "pt-PT" && currentCulture != "pt-BR" && currentCulture != "pt-MZ" )
            {
                currentCulture = "pt-MZ";
            }

            List<string> financeDocumentsList = new List<string>
            {
                "ReportDocumentFinance_"        + currentCulture,
                "ReportDocumentFinancePayment_" + currentCulture,
                "ReportDocumentFinanceWayBill_" + currentCulture
            };

            /* IN005975: This is covering the scenario of "_reportFileName" does have a temp file path (noticed when not in debug mode)
             * Debug: Resources/Reports/UserReports/ReportDocumentFinance_pt-MZ.frx
             * Release: Temp/Q1bSDxUR.ReportDocumentFinance_pt-MZ.frx
             * Mocambique
             */
            string documentFileName = Path.GetFileNameWithoutExtension(_reportFileName);
            bool isFinanceDocument = financeDocumentsList.Contains(documentFileName.Substring(documentFileName.LastIndexOf(".") + 1)); // "Resources/Reports/UserReports/ReportDocumentFinance_pt-MZ.frx"

            if (isFinanceDocument)
            {
                //Processed|Emitted with certified Software Nº {0}/AT - Copyright {1} - Licenced to a {2} - Used only if System Country is Portugal
                if (SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryPortugal)
                {

                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = (_reportFileName.EndsWith(fileName))
                        ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_emitted")
                        : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed")
                    ;
                    if (currentCulture == "pt-AO")
                    {
                        textObjectOverlaySoftwareCertification.Text = string.Format(
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification"),
                        prefix,
                        SettingsApp.SaftSoftwareCertificateNumber,
                        SettingsApp.SaftProductID,
                        GlobalFramework.LicenceCompany,
                        "LOGICPULSE ANGOLA");
                    }
                    else
                    {
                        textObjectOverlaySoftwareCertification.Text = string.Format(
                            resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification"),
                            prefix,
                            SettingsApp.SaftSoftwareCertificateNumber,
                            SettingsApp.SaftProductID,
                            GlobalFramework.LicenceCompany);
                    }


                    //Add Hash Validation if Defined (In DocumentFinance Only)
                    if (_hash4Chars != String.Empty) textObjectOverlaySoftwareCertification.Text = string.Format("{0} - {1}", _hash4Chars, textObjectOverlaySoftwareCertification.Text);

                } /* IN005975 and IN005979 for Mozambique deployment */
                else if (SettingsApp.ConfigurationSystemCountry.Oid == SettingsApp.XpoOidConfigurationCountryMozambique)
                {
                    /* 
                     * IN006047
                     * {Processado por computador} || Autorização da Autoridade Tributária: {DAFM1 - 0198 / 2018}
                     */
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification"),
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed"),
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_moz_tax_authority_cert_number")
                    );
                }
                else if (SettingsApp.XpoOidConfigurationCountryAngola.Equals(SettingsApp.ConfigurationSystemCountry.Oid))
                {
                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_processed"); ;
                    string localDate = DateTime.Now.Year.ToString();
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_ao"),
                        prefix,
                        SettingsApp.SaftSoftwareCertificateNumberAO,
                        SettingsApp.SaftProductIDAO,
                        localDate);

                    //Add Hash Validation if Defined (In DocumentFinance Only)
                    if (_hash4Chars != String.Empty) textObjectOverlaySoftwareCertification.Text = string.Format("{0} - {1}", _hash4Chars, textObjectOverlaySoftwareCertification.Text);

                }
            }
            else
            {
                if (SettingsApp.XpoOidConfigurationCountryAngola.Equals(SettingsApp.ConfigurationSystemCountry.Oid))
                {
                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_emitted"); ;
                    string localDate = DateTime.Now.Year.ToString();
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_report_overlay_software_certification_ao"),
                        prefix,
                        SettingsApp.SaftSoftwareCertificateNumberAO,
                        SettingsApp.SaftProductIDAO,
                        localDate);
                }
            }
            
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
                fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), pDocumentFinanceMasterOid);


                /// <summary>
                ///     This change refers to "ENH201810#04".
                /// </summary>
                /// <remarks>
                ///     <para>DESCRIPTION: This code change covers MZ and AO invoice layout enhancement, based on "CurrentCulture" settings.</para>
                ///     <para>ISSUE: all prices greater than million were being cut on invoice.</para>
				///     <para>CAUSE: there was no proper locale based invoice files.</para>
                ///     <para>SOLUTION: It was created a file for each of those specific locale settings, based on original files. 
                ///     For example: based on "ReportDocumentFinance.frx" it was created "ReportDocumentFinance_pt-MZ.frx".
                ///     </para>
                /// </remarks>
                /// <example>
                ///     "Preço" value: 35 000 000,00
                ///     Shows the value:  000 000,00
                /// </example>
                // Build Final Filename Report
                //string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";                

                string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
                try
                {
                    if (CultureInfo.CurrentUICulture.Name != currentCulture)
                    {
                        //CultureInfo.CurrentUICulture = GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                    }
                }
                catch
                {
                    currentCulture = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                    //CultureInfo.CurrentUICulture = GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                }

                if (string.IsNullOrEmpty(currentCulture))
                {
                    currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];                    
                }
                _log.Debug("Current Culture: " + currentCulture);
                if (currentCulture != "pt-AO" && currentCulture != "pt-PT" && currentCulture != "pt-BR" && currentCulture != "pt-MZ" )
                {
                    currentCulture = "pt-MZ";
                }

                //string currentCulture = GlobalFramework.CurrentCulture.Name;
                string fileName = (documentMaster.DocumentType.WayBill) ? "ReportDocumentFinanceWayBill_" + currentCulture + ".frx" : "ReportDocumentFinance_" + currentCulture + ".frx";
				//ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                if (Convert.ToBoolean(GlobalFramework.PreferenceParameters["PRINT_QRCODE"]) && SettingsApp.ConfigurationSystemCountry.Oid.Equals(SettingsApp.XpoOidConfigurationCountryPortugal) && !string.IsNullOrEmpty(documentMaster.ATDocQRCode))
                {
                    fileName = fileName.Replace(".frx", "_QRCode.frx");
                }
                string fileUserReportDocumentFinance = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", fileName));
                
                CustomReport customReport = new CustomReport(fileUserReportDocumentFinance, FILENAME_TEMPLATE_BASE, pCopyNames);
                customReport.DoublePass = (documentMaster.DocumentDetail.Count > SettingsApp.CustomReportReportDocumentFinanceMaxDetail);  
                customReport.Hash4Chars = pHash4Chars;
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
				//ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                customReport.SetParameterValue("ATDocQRCodeVisible", GlobalFramework.PreferenceParameters["PRINT_QRCODE"].ToString());
                customReport.SetParameterValue("ATDocQRCode", documentMaster.ATDocQRCode);
                
                //Report Parameters
                //customReport.SetParameterValue("Invoice Noº", 280);

                //Get Result Objects from FRBOHelper
                ResultFRBODocumentFinanceMaster fRBOHelperResponseProcessReportFinanceDocument = FRBOHelper.GetFRBOFinanceDocument(pDocumentFinanceMasterOid);
                //Get Generic Collections From FRBOHelper Results
                FRBOGenericCollection<FRBODocumentFinanceMasterView> gcDocumentFinanceMaster = fRBOHelperResponseProcessReportFinanceDocument.DocumentFinanceMaster;         
                //Prepare and Enable DataSources
                customReport.RegisterData(gcDocumentFinanceMaster, "DocumentFinanceMaster");

                /* IN005976 for Mozambique deployment */
                if (SettingsApp.ConfigurationSystemCountry.Oid.Equals(SettingsApp.XpoOidConfigurationCountryMozambique) || ConfigurationManager.AppSettings["cultureFinancialRules"] == "fr-CF")
                {
                    //if (GlobalFramework.CurrentCulture.Name.Equals("pt-MZ")){
                    cfg_configurationcurrency defaultCurrencyForExchangeRate = 
                        (cfg_configurationcurrency)FrameworkUtils.GetXPGuidObject(
                            GlobalFramework.SessionXpo,
                            typeof(cfg_configurationcurrency),
                            SettingsApp.XpoOidConfigurationCurrencyUSDollar);

                    customReport.SetParameterValue("DefaultCurrencyForExchangeRateAcronym", defaultCurrencyForExchangeRate.Acronym);
                    customReport.SetParameterValue("DefaultCurrencyForExchangeRateTotal", defaultCurrencyForExchangeRate.ExchangeRate);
                    //}
                }

                if (customReport.GetDataSource("DocumentFinanceMaster") != null) customReport.GetDataSource("DocumentFinanceMaster").Enabled = true;
                if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail").Enabled = true;
                if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal").Enabled = true;


                if (SettingsApp.ConfigurationSystemCountry.Oid.Equals(SettingsApp.XpoOidConfigurationCountryAngola))
                {
                    if (documentMaster.DocumentParent != null && documentMaster.DocumentType.Oid.ToString() == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment.ToString())
                    {
                        documentMaster.Notes += string.Format(
                            resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_source_document") + ": " + documentMaster.DocumentParent.DocumentNumber);
                        customReport.SetParameterValue("DocumentFinanceMaster.Notes", documentMaster.Notes);
                    }
                }

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
                
                int n= customReport.Pages.Count;

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
                //string fileUserReportDocumentFinancePayment = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", "ReportDocumentFinancePayment.frx"));
                //TK016319 - Certificação Angola - Alterações para teste da AGT
                //Bug - Não eram utilizados templates por região nos recibos, apenas default
                string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
                try
                {
                    if (CultureInfo.CurrentUICulture.Name != currentCulture)
                    {
                        //CultureInfo.CurrentUICulture = GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                    }
                }
                catch
                {
                    currentCulture = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                    //CultureInfo.CurrentUICulture = GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                }

                if (currentCulture != "pt-AO" || currentCulture != "pt-PT" || currentCulture != "pt-BR" || currentCulture != "pt-MZ")
                {
                    currentCulture = "pt-MZ";
                }


                string fileFrxFromCulture = string.Format("ReportDocumentFinancePayment_" + currentCulture + ".frx");
                string fileUserReportDocumentFinancePayment = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", fileFrxFromCulture));
                _log.Debug("Current Culture: " + currentCulture);
                CustomReport customReport = new CustomReport(fileUserReportDocumentFinancePayment, FILENAME_TEMPLATE_BASE, pCopyNames);

                //Get Result Objects from FRBOHelper
                ResultFRBODocumentFinancePayment fRBOHelperResponseProcessReportFinancePayment = FRBOHelper.GetFRBOFinancePayment(pDocumentFinancePaymentOid);
                //Get Generic Collections From FRBOHelper Results
                FRBOGenericCollection<FRBODocumentFinancePaymentView> gcDocumentFinancePayment = fRBOHelperResponseProcessReportFinancePayment.DocumentFinancePayment;

                //Prepare and Enable DataSources
                customReport.RegisterData(gcDocumentFinancePayment, "DocumentFinancePayment");
                if (customReport.GetDataSource("DocumentFinancePayment") != null) customReport.GetDataSource("DocumentFinancePayment").Enabled = true;
                if (customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument") != null) customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument").Enabled = true;
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
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
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_family_subfamily_articles"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
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
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_customers"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                //"Oid,Designation,ButtonLabel"
                FRBOGenericCollection<FRBOCustomerType> gcCustomerType = new FRBOGenericCollection<FRBOCustomerType>();
                FRBOGenericCollection<FRBOCustomer> gcCustomers;

                //Render Child Bussiness Objects
                foreach (FRBOCustomerType customerType in gcCustomerType)
                {
                    //Get Customer from current customerType
                    gcCustomers = new FRBOGenericCollection<FRBOCustomer>(string.Format("CustomerType = '{0}'", customerType.Oid), "Ord");
                    customerType.Customer = gcCustomers.List;

                    if (gcCustomers != null && gcCustomers.List.Count > 0)
                    {
                        // Decrypt Phase
                        if (GlobalFramework.PluginSoftwareVendor != null)
                        {
                            foreach (var item in gcCustomers.List)
                            {
                                item.Name = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Name);
                                item.Address = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Address);
                                item.Locality = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Locality);
                                item.ZipCode = GlobalFramework.PluginSoftwareVendor.Decrypt(item.ZipCode);
                                item.City = GlobalFramework.PluginSoftwareVendor.Decrypt(item.City);
                                item.DateOfBirth = GlobalFramework.PluginSoftwareVendor.Decrypt(item.DateOfBirth);
                                item.Phone = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Phone);
                                item.Fax = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Fax);
                                item.MobilePhone = GlobalFramework.PluginSoftwareVendor.Decrypt(item.MobilePhone);
                                item.Email = GlobalFramework.PluginSoftwareVendor.Decrypt(item.Email);
                                item.WebSite = GlobalFramework.PluginSoftwareVendor.Decrypt(item.WebSite);
                                item.FiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.FiscalNumber);
                                item.CardNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.CardNumber);
                            }
                        }
                    }
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
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_stock_movements"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
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

        public static void ProcessReportArticleStockWarehouse(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportArticleStockWarehouseList.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_stock_warehouse"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections 
                FRBOGenericCollection<FRBOArticleStockWareHouseView> gcArticleStockWarehouse = new FRBOGenericCollection<FRBOArticleStockWareHouseView>(filter);

                //Prepare and Enable DataSources
                customReport.RegisterData(gcArticleStockWarehouse, "ArticleStockWarehouse");
                if (customReport.GetDataSource("ArticleStockWarehouse") != null) customReport.GetDataSource("ArticleStockWarehouse").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportArticleStock(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportArticleStockList.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_stock_article"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections 
                FRBOGenericCollection<FRBOArticleStockView> gcArticleStock = new FRBOGenericCollection<FRBOArticleStockView>(filter);

                //New collection to filter only unique results
                //Starts with 0 results
                FRBOGenericCollection<FRBOArticleStockView> gcArticleStockNew = new FRBOGenericCollection<FRBOArticleStockView>("Date <= '0001-01-01 00:00:00");
                List<string> listArticles = new List<string>();

                //Decrypt Name
                foreach (FRBOArticleStockView line in gcArticleStock)
                {
                    string sqlCount = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE {0} AND Article = '{1}' AND (Disabled = 0 OR Disabled is NULL)", filter, line.Article);
                    line.ArticleStockQuantity = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sqlCount));
                    line.ArticleStockDateDay = filterHumanReadable;
                    //Only add unique Articles to new collection
                    if (!listArticles.Contains(line.Article.ToString())) 
                    {
                        listArticles.Add(line.Article.ToString());
                        gcArticleStockNew.Add(line);
                    } 
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcArticleStockNew, "ArticleStock");
                if (customReport.GetDataSource("ArticleStock") != null) customReport.GetDataSource("ArticleStock").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportArticleStockSupplier(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportArticleStockSupplierList.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_stock_supplier"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                if (filter.Contains("stmOid"))
                {
                    var result1 = filterHumanReadable.Split(',');
                    var result2 = result1[result1.Length-1].Substring(result1[result1.Length-1].IndexOf('\''), result1[result1.Length-1].LastIndexOf('\''));

                    var result3 = filter.Substring(0, filter.IndexOf(')')+1);

                    filter = result3 + " AND (stmDocumentNumber = " + result2 + ")";

                }

                //Prepare and Declare FRBOGenericCollections 
                FRBOGenericCollection<FRBOArticleStockSupplierView> gcArticleStockSupplier = new FRBOGenericCollection<FRBOArticleStockSupplierView>(filter);

                //Get Default defaultCurrency
                cfg_configurationcurrency defaultCurrency = SettingsApp.ConfigurationSystemCurrency;
                //Currency - If Diferent from Default System Currency, get Currency Object from Parameter
                cfg_configurationcurrency configurationCurrency;                
                configurationCurrency = (cfg_configurationcurrency)GlobalFramework.SessionXpo.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid);                
        
                //Decrypt Name
                foreach (FRBOArticleStockSupplierView line in gcArticleStockSupplier)
                {
                    line.ArticleStockCostumerName = GlobalFramework.PluginSoftwareVendor.Decrypt(line.ArticleStockCostumerName);
                    if (string.IsNullOrEmpty(line.ArticleStockCurrency)) line.ArticleStockCurrency = configurationCurrency.Acronym;
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcArticleStockSupplier, "ArticleStockSupplier");
                if (customReport.GetDataSource("ArticleStockSupplier") != null) customReport.GetDataSource("ArticleStockSupplier").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }


        public static void ProcessReportVatSalesResumed(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc
                string reportFile = GetReportFilePath("ReportDocumentFinanceVatSalesSummary.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_sales_per_vat"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                customReport.SetParameterValue("Report Filter", filterHumanReadable);


                string query = String.Format(@"SELECT cfOid as Oid, fdVat as Vat, ftDocumentTypeDesignation AS DocumentTypeDesignation, SUM(fdTotalNet) AS TotalNet, SUM(fdTotalTax) AS TotalTax, SUM(fdTotalFinal) AS TotalFinal
                                FROM view_documentfinance
                                WHERE fdVat IS NOT NULL AND cfOid IS NOT NULL
                                AND (ftDocumentTypeAcronym = 'FT' OR ftDocumentTypeAcronym = 'FS'  OR ftDocumentTypeAcronym = 'FR') AND fmOid IN (select Oid From fin_documentfinancemaster where Oid = fmOid and DocumentStatusStatus <> 'A')
                                AND fmOid NOT IN (select DocumentParent From fin_documentfinancemaster where DocumentParent = fmOid AND DocumentStatusStatus <> 'A' AND (ftDocumentTypeAcronym = 'NC' OR ftDocumentTypeAcronym = 'ND' ))
                                AND {0}
                                GROUP BY cfOid, fdVat, ftDocumentTypeDesignation", filter);

                //Prepare and Declare FRBOGenericCollections 
                FRBOGenericCollection<FRBODocumentFinanceVatSalesSummary> GCDocumentFinanceVatSalesSummary = new FRBOGenericCollection<FRBODocumentFinanceVatSalesSummary>("", 0, query);

                foreach (var item in GCDocumentFinanceVatSalesSummary)
                {
                    item.Vat = Convert.ToDecimal(FrameworkUtils.DecimalToString(item.Vat, GlobalFramework.CurrentCultureNumberFormat, "0"));
                }

                //Get Default defaultCurrency
             cfg_configurationcurrency defaultCurrency = SettingsApp.ConfigurationSystemCurrency;
                //Currency - If Diferent from Default System Currency, get Currency Object from Parameter
                cfg_configurationcurrency configurationCurrency;
                configurationCurrency = (cfg_configurationcurrency)GlobalFramework.SessionXpo.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid);
                customReport.SetParameterValue("Currency", configurationCurrency.Acronym);

                //Prepare and Enable DataSources
                customReport.RegisterData(GCDocumentFinanceVatSalesSummary, "DocumentFinanceVatSalesSummary");
                if (customReport.GetDataSource("DocumentFinanceVatSalesSummary") != null) customReport.GetDataSource("DocumentFinanceVatSalesSummary").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportVatSalesByClassResumed(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc
                string reportFile = GetReportFilePath("ReportDocumentFinanceVatSalesByClassSummary.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_sales_per_vat_by_article_class"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                customReport.SetParameterValue("Report Filter", filterHumanReadable);


                string query = String.Format(@"SELECT cfOid as Oid, fdVat as Vat, acDesignation AS ArticleClassDesignation, SUM(fdTotalNet) AS TotalNet, SUM(fdTotalTax) AS TotalTax, SUM(fdTotalFinal) AS TotalFinal
                                FROM view_documentfinance
                                WHERE arClass IS NOT NULL AND cfOid IS NOT NULL
                                AND fmOid NOT IN (select DocumentParent From fin_documentfinancemaster where DocumentParent = fmOid AND DocumentStatusStatus <> 'A' AND (ftDocumentTypeAcronym = 'NC' OR ftDocumentTypeAcronym = 'ND' ))
                                AND fmOid IN (SELECT Oid FROM fin_documentfinancemaster WHERE Oid = fmOid and DocumentStatusStatus <> 'A' AND (ftDocumentTypeAcronym = 'FT' OR ftDocumentTypeAcronym = 'FS'  OR ftDocumentTypeAcronym = 'FR'))
                                AND {0}
                                GROUP BY cfOid, fdVat, acDesignation", filter);

                //Prepare and Declare FRBOGenericCollections 
                FRBOGenericCollection<FRBODocumentFinanceVatSalesByClassSummary> GCDocumentFinanceVatSalesByClassSummary = new FRBOGenericCollection<FRBODocumentFinanceVatSalesByClassSummary>("", 0, query);



                foreach (var item in GCDocumentFinanceVatSalesByClassSummary)
                {
                    item.Vat = Convert.ToDecimal(FrameworkUtils.DecimalToString(item.Vat, GlobalFramework.CurrentCultureNumberFormat, "0"));
                }

                //Get Default defaultCurrency
                cfg_configurationcurrency defaultCurrency = SettingsApp.ConfigurationSystemCurrency;
                //Currency - If Diferent from Default System Currency, get Currency Object from Parameter
                cfg_configurationcurrency configurationCurrency;
                configurationCurrency = (cfg_configurationcurrency)GlobalFramework.SessionXpo.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid);
                customReport.SetParameterValue("Currency", configurationCurrency.Acronym);

                //Prepare and Enable DataSources
                customReport.RegisterData(GCDocumentFinanceVatSalesByClassSummary, "DocumentFinanceVatSalesByClassSummary");
                if (customReport.GetDataSource("DocumentFinanceVatSalesByClassSummary") != null) customReport.GetDataSource("DocumentFinanceVatSalesByClassSummary").Enabled = true;

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
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_audit_table"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBOSystemAuditView> gcSystemAudit = new FRBOGenericCollection<FRBOSystemAuditView>(filter, String.Empty, "SauDate");

                // Decrypt Phase
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    foreach (var item in gcSystemAudit)
                    {
                        if (item.UserDetailName != null)
                        {
                            item.UserDetailName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.UserDetailName);
                        }
                    }
                }

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
        /// <summary>
        /// Calls the report flow for Customer Current Account details.
        /// </summary>
        /// <param name="pViewMode"></param>
        /// <param name="filter"></param>
        /// <param name="filterHumanReadable"></param>
        public static void ProcessReportDocumentFinanceCurrentAccount(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportDocumentFinanceCurrentAccount.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_current_account"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                /* IN006004 */
                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinanceCurrentAccount> gcCurrentAccount = new FRBOGenericCollection<FRBODocumentFinanceCurrentAccount>(filter);

                // Decrypt Phase
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    foreach (var item in gcCurrentAccount)
                    {
                        if (item.EntityName != null) item.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityName);
                        if (item.EntityFiscalNumber != null) item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);
                    }
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcCurrentAccount, "CurrentAccount");
                if (customReport.GetDataSource("CurrentAccount") != null) customReport.GetDataSource("CurrentAccount").Enabled = true;

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method calls the report flow for Customer Balance Details.
        /// Pleasee see #IN008018# for further details.
        /// </summary>
        /// <param name="pViewMode"></param>
        /// <param name="filter"></param>
        /// <param name="filterHumanReadable"></param>
        public static void ProcessReportCustomerBalanceDetails(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportDocumentFinanceCustomerBalanceDetails.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_customer_balance_details"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);

                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails> gcCustomerBalanceDetails = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails>(filter);
                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary> gcCustomerBalanceSummary = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary>();
                erp_customer customer = null;
                List<erp_customer> customersList = new List<erp_customer>();
                bool printTotalBalance = true;
                // Decrypt Phase
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    foreach (var item in gcCustomerBalanceDetails)
                    {

                        if (item.EntityName != null) item.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityName);
                        if (item.EntityFiscalNumber != null) item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);
                       
                        if (item.EntityOid != null) customer = (erp_customer)GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), Guid.Parse(item.EntityOid));
                        if (!customersList.Contains(customer))
                        {
                            customersList.Add(customer);
                        }
                        foreach (var summary in gcCustomerBalanceSummary)
                        {
                            if (summary.Oid != null && summary.Oid.Equals(item.EntityOid))
                            {
                                item.Balance = summary.Balance;
                                item.CustomerSinceDate = summary.CustomerSinceDate;
                                break;
                            }
                        }
                    }
                }
                if (customersList.Count > 1) printTotalBalance = false;
                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary> gcCustomerBalanceSummaryTotal = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary>(string.Format("(EntityOid = '{0}')", customer.Oid));
                customReport.SetParameterValue("PrintTotalBalance", printTotalBalance);
                customReport.SetParameterValue("TotalCreditFinal", gcCustomerBalanceSummaryTotal.List[0].TotalCredit);
                customReport.SetParameterValue("TotalDebitFinal", gcCustomerBalanceSummaryTotal.List[0].TotalDebit);
                customReport.SetParameterValue("TotalBalanceFinal", gcCustomerBalanceSummaryTotal.List[0].Balance);

                //Prepare and Enable DataSources
                customReport.RegisterData(gcCustomerBalanceDetails, "CustomerBalanceDetails");
                if (customReport.GetDataSource("CustomerBalanceDetails") != null) customReport.GetDataSource("CustomerBalanceDetails").Enabled = true;
                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// This method calls the Company Billing report flow.
        /// Please see #IN009204# for further details.
        /// </summary>
        /// <param name="pViewMode"></param>
        /// <param name="filter"></param>
        /// <param name="filterHumanReadable"></param>
        public static void ProcessReportCompanyBilling(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                string reportFile = GetReportFilePath("ReportDocumentFinanceCompanyBilling.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_company_billing"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);

                if (!string.IsNullOrEmpty(filterHumanReadable))
                {
                    customReport.SetParameterValue("Report Filter", filterHumanReadable);
                }

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails> gcCustomerBalanceDetails = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails>(filter);

                /* Decrypt phase */
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    foreach (var item in gcCustomerBalanceDetails)
                    {
                        if (item.EntityName != null) item.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityName);
                        if (item.EntityFiscalNumber != null) item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);
                    }
                }

                /* Enabling data sources */
                customReport.RegisterData(gcCustomerBalanceDetails, "CustomerBalanceDetails");
                if (customReport.GetDataSource("CustomerBalanceDetails") != null)
                {
                    customReport.GetDataSource("CustomerBalanceDetails").Enabled = true;
                }

                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error("void ProcessReportCompanyBilling(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable) :: Company Billing report: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// This method calls the report flow for Customer Balance Summary.
        /// Pleasee see #IN009010# for further details.
        /// </summary>
        /// <param name="pViewMode"></param>
        /// <param name="filter"></param>
        /// <param name="filterHumanReadable"></param>
        public static void ProcessReportCustomerBalanceSummary(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                string reportFile = GetReportFilePath("ReportDocumentFinanceCustomerBalanceSummary.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_customer_balance_summary"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);

                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary> gcCustomerBalanceSummary = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceSummary>(filter);
                FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails> gcCustomerBalanceDetails = new FRBOGenericCollection<FRBODocumentFinanceCustomerBalanceDetails>(filter.Replace("CustomerSinceDate", "Date"));

                // Decrypt Phase
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    foreach (var customerBalance in gcCustomerBalanceDetails)
                    {
                        erp_customer customer = null;
                        if (customerBalance.EntityName != null) customerBalance.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(customerBalance.EntityName);
                        if (customerBalance.EntityFiscalNumber != null) customerBalance.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(customerBalance.EntityFiscalNumber);

                        foreach (var summary in gcCustomerBalanceSummary)
                        {
                            if (summary.Oid != null && summary.Oid.Equals(customerBalance.EntityOid))
                            {
                                if (!string.IsNullOrEmpty(customerBalance.EntityOid))
                                {
                                    customer = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), new Guid(customerBalance.EntityOid));
                                    summary.EntityName = customer.Name;
                                    summary.EntityFiscalNumber = customer.FiscalNumber;
                                }
                                else
                                {
                                    summary.EntityName = "No data";
                                    summary.EntityFiscalNumber = "No data";
                                }
                            }
                        }
                    }
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcCustomerBalanceSummary, "CustomerBalanceSummary");
                if (customReport.GetDataSource("CustomerBalanceSummary") != null)
                {
                    customReport.GetDataSource("CustomerBalanceSummary").Enabled = true;
                }

                //customReport.ReportInfo.Name = FILL THIS WITH REPORT NAME;
                customReport.Process(pViewMode);
                customReport.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ProcessReportUserCommission(CustomReportDisplayMode pViewMode, string filter, string filterHumanReadable)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = GetReportFilePath("ReportUserCommission.frx");
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_list_user_commission"));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                //customReport.SetParameterValue("Factura No", 280);

                //Prepare and Declare FRBOGenericCollections
                FRBOGenericCollection<FRBOUserCommission> gcUserCommission = new FRBOGenericCollection<FRBOUserCommission>(filter);

                // Decrypt Phase
                if (GlobalFramework.PluginSoftwareVendor != null && gcUserCommission != null)
                {
                    foreach (var item in gcUserCommission)
                    {
                        if (item.UserName != null)
                        {
                            item.UserName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.UserName);
                        }
                    }
                }

                //Prepare and Enable DataSources
                customReport.RegisterData(gcUserCommission, "UserCommission");
                if (customReport.GetDataSource("UserCommission") != null) customReport.GetDataSource("UserCommission").Enabled = true;

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
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);
                //Report Parameters
                customReport.SetParameterValue("Report Title", reportTitle);
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
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

                /* IN009077 - # TO DO (IN009078, IN009079, IN009080) */
                DevExpress.Xpo.UnitOfWork uowSession = new DevExpress.Xpo.UnitOfWork();
                foreach (var item in gcDocumentFinanceMaster)
                {
                    /* Decrypt phase - see IN009078, IN009079 and IN009080 */
                    if (GlobalFramework.PluginSoftwareVendor != null)
                    {
						/* IN009076 */
                        item.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityName);
                        /* IN009075 */
                        item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);
                    }

                    /* If "Nota de Crédito" (NC) */
                    if (SettingsApp.XpoOidDocumentFinanceTypeCreditNote.Equals(item.DocumentType.Oid))
                    {
                        item.PaymentMethod = new fin_configurationpaymentmethod { Designation = item.DocumentType.Designation, Ord = 999, Code = 999 }; /* Setting to 999 to avoid NC being grouped with other Payment Method created */
                        item.PaymentCondition = new fin_configurationpaymentcondition { Designation = item.DocumentType.Designation, Ord = 999, Code = 999 }; /* Sets the same as above in order to keep the pattern */

                        /* IN009084 - Make total values negative when NC (see IN009066) */
                        item.TotalFinalRound  *= -1;
                        item.TotalFinal *= -1;
                        item.TotalDiscount *= -1;
                        item.TotalDelivery *= -1;
                        item.TotalTax *= -1;
                        item.TotalNet *= -1;
                        item.TotalGross *= -1;

                    } else {
                        /* Case FS */
                        if (item.PaymentCondition == null)
                        {
                            item.PaymentCondition = (fin_configurationpaymentcondition)uowSession.GetObjectByKey(typeof(fin_configurationpaymentcondition), SettingsApp.XpoOidConfigurationPaymentMethodInstantPayment); /* Sets "Pronto Pagamento" to FS */
                        }
                        /* Case FT */
                        if (item.PaymentMethod == null)
                        {
                            if (!item.Payed)
                            {
                                item.PaymentMethod = new fin_configurationpaymentmethod();
                                item.PaymentMethod.Designation = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_detailed_grouped_pending_payment");

                            } else {
                                item.PaymentMethod = new fin_configurationpaymentmethod { Designation = item.DocumentType.Designation, Ord = 998, Code = 998 }; /* Setting to 998 to avoid Payed FT being grouped with other Payment Method created */
                            }
                        }
                    }
                }

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

        // Used in Detail
        public static void ProcessReportDocumentDetail(CustomReportDisplayMode pViewMode, string resourceString, string groupCondition, string groupTitle, string filter, string filterHumanReadable)
        {
            ProcessReportDocumentDetail(pViewMode, resourceString, null, null, groupCondition, groupTitle, filter, filterHumanReadable, false);
        }

        // Used in Detail/Group
        public static void ProcessReportDocumentDetail(CustomReportDisplayMode pViewMode, string resourceString, string groupField, string groupSelectFields, string groupCondition, string groupTitle, string filter, string filterHumanReadable, bool grouped, bool decryptGroupField = false)
        {
            try
            {
                //Move This to CustomReport SubClasses ex Filename, Params, DataSources etc

                string reportFile = (grouped)
                    ? GetReportFilePath("ReportDocumentFinanceDetailGroupList.frx")
                    : GetReportFilePath("ReportDocumentFinanceDetailList.frx")
                ;
                CustomReport customReport = new CustomReport(reportFile, FILENAME_TEMPLATE_BASE_SIMPLE, 1);

                // Add PostFix to Report Title 
                Tuple<string, string> tuppleResourceString = GetResourceString(resourceString);
                string reportTitleString = tuppleResourceString.Item1;
                string reportTitleStringPostfix = tuppleResourceString.Item2;

                //Report Parameters
                customReport.SetParameterValue("Report Title", String.Format("{0}{1}", reportTitleString, reportTitleStringPostfix));
                customReport.SetParameterValue("Report_FileName_Logo", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO"]);
                customReport.SetParameterValue("Report_FileName_Logo_Small", GlobalFramework.PreferenceParameters["REPORT_FILENAME_LOGO_SMALL"]);
                if (!string.IsNullOrEmpty(filterHumanReadable)) customReport.SetParameterValue("Report Filter", filterHumanReadable);

                // Get Objects
                GroupHeaderBand groupHeaderBand = (GroupHeaderBand)customReport.FindObject("GroupHeader1");
                TextObject groupHeaderBandText = (TextObject)customReport.FindObject("TextGroupHeader1");
                if (groupHeaderBand != null && groupHeaderBandText != null)
                {
                    groupHeaderBand.Condition = groupCondition;
                    groupHeaderBandText.Text = groupTitle;
                    if (groupTitle == "[DocumentFinanceDetail.ArticleVat]")
                    {

                        groupHeaderBandText.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_vat_rates") + ": " + groupTitle + "%";
                        string documentNodeFilter = " AND (ftSaftDocumentType = 1 AND (fmDocumentStatusStatus = 'N' OR fmDocumentStatusStatus = 'F' OR fmDocumentStatusStatus = 'A'))";
                        filter += documentNodeFilter;
                    }
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
                    /* IN009085 - FastReport throws error when there is no register */
                    if (gcDocumentFinanceMasterDetail.List.Count == 0)
                    {
                        gcDocumentFinanceMasterDetail = new FRBOGenericCollection<FRBODocumentFinanceMasterDetailView>();
                    }
                    // Decrypt Phase
                    if (GlobalFramework.PluginSoftwareVendor != null)
                    {
                        /* IN009077 - # TO DO (IN009078, IN009079, IN009080) */
                        DevExpress.Xpo.UnitOfWork uowSession = new DevExpress.Xpo.UnitOfWork();

                        foreach (var item in gcDocumentFinanceMasterDetail)
                        {
                            item.UserDetailName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.UserDetailName);
                            item.EntityName = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityName);
                            /* IN009075 */
                            item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);
                            /* IN009072 - this is used on reports to subtract the below values from totals when financial document is "NC" (see IN009066) */
                            if (SettingsApp.XpoOidDocumentFinanceTypeCreditNote.Equals(new Guid(item.DocumentType)))
                            {
                                item.ArticleQuantity *= -1;
                                item.ArticleTotalFinal *= -1;
                                item.ArticleTotalNet *= -1;
                                item.ArticleTotalTax *= -1;

                                /* IN009077 - # TO DO (IN009078, IN009079, IN009080) and implement this for Detailed/Grouped Reports */
                                item.PaymentMethod = item.DocumentTypeDesignation;
                                /* Setting to 999 to avoid NC being grouped with other Payment Method created */
                                item.PaymentMethodOrd = 999;
                                item.PaymentMethodCode = 999;

                                item.PaymentCondition = item.DocumentTypeDesignation;
                                /* Setting to 999 to avoid NC being grouped with other Condition Method created */
                                item.PaymentConditionOrd = 999;
                                item.PaymentConditionCode = 999;
                                /* IN009077 - end */

                           } else { /* IN009077 - # TO DO (IN009078, IN009079, IN009080) */

                                /* Case FS */
                                if (item.PaymentCondition == null)
                                {
                                    /* Sets "Pronto Pagamento" to FS */
                                    fin_configurationpaymentcondition paymentCondition =  (fin_configurationpaymentcondition)uowSession.GetObjectByKey(typeof(fin_configurationpaymentcondition), SettingsApp.XpoOidConfigurationPaymentMethodInstantPayment);
                                    item.PaymentCondition = paymentCondition.Designation;
                                }
                                /* Case FT */
                                if (item.PaymentMethod == null)
                                {
                                    if (!item.Payed)
                                    {
                                        item.PaymentMethod = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_detailed_grouped_pending_payment");
                                    }
                                    else
                                    {
                                        /* IN009077 - # TO DO (IN009078, IN009079, IN009080) */
                                        item.PaymentMethod = item.DocumentTypeDesignation;
                                        /* Setting to 998 to avoid Payed FT being grouped with other Payment Method created */
                                        item.PaymentMethodOrd = 998;
                                        item.PaymentMethodCode = 998;
                                    }                                }
                            }

                            /* IN009086 - when document has no area */
                            //if (string.IsNullOrEmpty(item.PlaceCode))
                            if (item.PlaceCode == 0)
                                {
                                item.PlaceOrd = 999;
                                item.PlaceCode = 999;
                                item.PlaceDesignation = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_others");
                            }

                            /* IN009086 - when document has no order/table */
                            //if (string.IsNullOrEmpty(item.PlaceTableCode))
                            if (item.PlaceTableCode == 0)
                                {
                                item.PlaceTableOrd = 999;
                                item.PlaceTableCode = 999;
                                item.PlaceTableDesignation = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_others"); 
                            }
                        }
                    }
                    //Prepare and Enable DataSources
                    customReport.RegisterData(gcDocumentFinanceMasterDetail, "DocumentFinanceDetail");
                }
                else
                {
                    // Add Common GroupFields (Required for SQLServer Grouping)
                    string queryGroupFields = string.Format("fdArticle, fdCode, fdDesignation, fdUnitMeasure, {0}", groupField);
                    // Add Common Select Fields (Required for SQLServer Grouping) : Must use same FieldNames has Detail
                    string queryFields = string.Format("{0}, fdArticle AS ArticleOid, fdCode AS ArticleCode, fdDesignation AS ArticleDesignation, AVG((fdPrice - ((fdPrice * fdDiscount) / 100))) AS ArticlePriceWithDiscount, SUM(fdQuantity) AS ArticleQuantity, fdUnitMeasure AS ArticleUnitMeasure, SUM(fdTotalDiscount) AS ArticleTotalDiscount, SUM(fdTotalNet) AS ArticleTotalNet, SUM(fdTotalTax) AS ArticleTotalTax, SUM(fdTotalFinal) AS ArticleTotalFinal,COUNT(*) AS GroupCount", groupSelectFields);

                    // Using view_documentfinancesellgroup
                    FRBOGenericCollection<FRBODocumentFinanceMasterDetailGroupView> gcDocumentFinanceMasterDetail = new FRBOGenericCollection<FRBODocumentFinanceMasterDetailGroupView>(filter, queryGroupFields, string.Empty, queryFields);

                    // Decrypt Phase
                    if (GlobalFramework.PluginSoftwareVendor != null) /* IN009072 */
                    {
                        //gcDocumentFinanceMasterDetail.Get(0).GroupDesignation                    
                        foreach (var item in gcDocumentFinanceMasterDetail)
                        {
                            /* IN009075 - #TODO if necessary (test it), uncomment this when detailed/grouped reports available */
                            // item.EntityFiscalNumber = GlobalFramework.PluginSoftwareVendor.Decrypt(item.EntityFiscalNumber);

                            if (decryptGroupField)
                            {
                                item.GroupDesignation = GlobalFramework.PluginSoftwareVendor.Decrypt(item.GroupDesignation);
                            }
                            
                            /* IN009072 - "NCs" must have their values subtracted from totals (see IN009066) */
                            if (SettingsApp.XpoOidDocumentFinanceTypeCreditNote.Equals(new Guid(item.DocumentType)))
                            {
                                item.ArticleQuantity *= -1;
                                item.ArticleTotalNet *= -1;
                                item.ArticleTotalTax *= -1;
                                item.ArticleTotalFinal *= -1;
                            }
                        }
                    }
                    //Prepare and Enable DataSources
                    customReport.RegisterData(gcDocumentFinanceMasterDetail, "DocumentFinanceDetail");
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

        public static string DocumentMasterCreatePDF(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //return DocumentMasterCreatePDF(pDocumentFinanceMaster, String.Empty, CustomReportDisplayMode.ExportPDFSilent);
            return DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinanceMaster, String.Empty);
        }

        public static string DocumentMasterCreatePDF(fin_documentfinancemaster pDocumentFinanceMaster, string pDestinationFileName)
        {
            //return DocumentMasterCreatePDF(pDocumentFinanceMaster, pDestinationFileName, CustomReportDisplayMode.ExportPDFSilent);
            return DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinanceMaster, pDestinationFileName);
        }

        //public static string DocumentMasterCreatePDF(DocumentFinanceMaster pDocumentFinanceMaster, string pDestinationFileName, CustomReportDisplayMode pCustomReportDisplayMode)
        public static string DocumentMasterCreatePDF(CustomReportDisplayMode pDisplayMode, fin_documentfinancemaster pDocumentFinanceMaster, string pDestinationFileName)
        {
            string result = String.Empty;
            try
            {
                //Generate Default CopyNames from DocumentType
                List<int> copyNames = CopyNames(pDocumentFinanceMaster.DocumentType.PrintCopies);
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(pDocumentFinanceMaster.Hash);
                pDestinationFileName = pDestinationFileName.Replace('\n'.ToString(), "");
                result = ProcessReportFinanceDocument(pDisplayMode, pDocumentFinanceMaster.Oid, hash4Chars, copyNames, pDestinationFileName);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Generate FinanceDocumentPayment PDFs

        public static string DocumentPaymentCreatePDF(fin_documentfinancepayment pDocumentFinancePayment)
        {
            return DocumentPaymentCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinancePayment, String.Empty);
        }

        public static string DocumentPaymentCreatePDF(fin_documentfinancepayment pDocumentFinancePayment, string pDestinationFileName)
        {
            return DocumentPaymentCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinancePayment, pDestinationFileName);
        }

        public static string DocumentPaymentCreatePDF(CustomReportDisplayMode pDisplayMode, fin_documentfinancepayment pDocumentFinancePayment, string pDestinationFileName)
        {
            string result = String.Empty;
            try
            {
                //Generate Default CopyNames from DocumentType
                List<int> copyNames = CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
                result = ProcessReportFinanceDocumentPayment(pDisplayMode, pDocumentFinancePayment.Oid, copyNames, pDestinationFileName);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Barcode labels
        public static void ProcessReportBarcodeLabel(CustomReportDisplayMode pViewMode, fin_articleserialnumber pArticleSerialNumber, string pFooter, bool pBigSize, List<fin_articleserialnumber> pListArticleSerialNumber = null)
        {
            string reportFile = (!pBigSize)
                   ? GetReportFilePath("BarCodeTemplate_100x50.frx")
                   : GetReportFilePath("BarCodeTemplate_40x30.frx")
               ;

            //Get template from selected article
            if(pArticleSerialNumber != null && pArticleSerialNumber.Article.TemplateBarCode != null)
            {
                reportFile = pArticleSerialNumber.Article.TemplateBarCode.FileTemplate.ToString();
            }
            else if(pListArticleSerialNumber != null && pListArticleSerialNumber.Count > 0 && pListArticleSerialNumber[0].Article.TemplateBarCode != null)
            {
                reportFile = pListArticleSerialNumber[0].Article.TemplateBarCode.FileTemplate.ToString();
            }
            CustomReport customReport = new CustomReport(reportFile, "", 1);
            if (string.IsNullOrEmpty(pFooter)) pFooter = GlobalFramework.PreferenceParameters["COMPANY_WEBSITE"];

            //Report Parameters
            //customReport.SetParameterValue("SerialNumber", pArticleSerialNumber.SerialNumber);
            //customReport.SetParameterValue("ArticleName", pArticleSerialNumber.Article.Designation);
            //customReport.SetParameterValue("ArticleRef", pArticleSerialNumber.Article.Code);
            //customReport.SetParameterValue("footerText", pFooter

            List<FRBOArticleSerialNumber> fRBOArticleSerialNumbers = new List<FRBOArticleSerialNumber>();
     
            if (GlobalFramework.PluginSoftwareVendor != null)
            {
                if (pListArticleSerialNumber != null)
                {
                    foreach(var item in pListArticleSerialNumber)
                    {
                        fRBOArticleSerialNumbers.Add(new FRBOArticleSerialNumber() { SerialNumber = item.SerialNumber, ArticleName = item.Article.Designation, ArticleRef = "ref." + item.Article.Code, footerText = pFooter, Oid = item.Oid.ToString() });
                    }                    
                }
                else
                {
                    fRBOArticleSerialNumbers.Add(new FRBOArticleSerialNumber() { SerialNumber = pArticleSerialNumber.SerialNumber, ArticleName = pArticleSerialNumber.Article.Designation, ArticleRef = "ref." + pArticleSerialNumber.Article.Code, footerText = pFooter, Oid = pArticleSerialNumber.Oid.ToString() });
                }        
            }
            //Prepare and Enable DataSources
            customReport.RegisterData(fRBOArticleSerialNumbers, "ArticleSerialNumber");
            //customReport.MaxPages = 1;
            //Scripts - Dont Delete this Comment, may be usefull if we remove Script from Report File
            //Print X Records per Data Band
            FastReport.DataBand dataBand = (DataBand)customReport.FindObject("Data1");
            int dataBandRec = 1;
            //Used to Break Page on X Recs, Usefull to leave open space for Report Summary
            int dataBandMaxRecs = 30;

            dataBand.BeforePrint += delegate
            {
                var barcodeObject = (dataBand.Objects[0] as FastReport.Barcode.BarcodeObject);
                barcodeObject.Text = (dataBand.DataSource.CurrentRow as FRBOArticleSerialNumber).SerialNumber;
                barcodeObject.AutoSize = false;
            };
           

            dataBand.AfterPrint += delegate
            {
                Console.WriteLine(string.Format("dataBandRec.RowNo:[{0}], dataBandMaxRecs:[{1}], , dataBand.RowNo[{2}], report.Pages.Count[{3}]", dataBandRec, dataBandMaxRecs, dataBand.RowNo, customReport.Pages.Count));
                if (dataBandRec == dataBandMaxRecs)
                {
                    dataBandRec = 1;
                    dataBand.StartNewPage = true;
                }
                else
                {
                    dataBandRec++;
                    dataBand.StartNewPage = false;
                };
            };

            if (customReport.GetDataSource("ArticleSerialNumber") != null) customReport.GetDataSource("ArticleSerialNumber").Enabled = true;
            var dataSource = customReport.GetDataSource("ArticleSerialNumber");

            customReport.Dictionary.SystemVariables.FindByName("PreparedPages").Value = (customReport.PreparedPages != null) ? customReport.PreparedPages.Count : 0;
            //Call Report Prepare
            customReport.Prepare(true);
            if (!FrameworkUtils.IsLinux)
            {
                customReport.ShowPrepared(true);
            }
            else
            {
                string dateTimeFileFormat = SettingsApp.FileFormatDateTime;
                string dateTime = FrameworkUtils.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
                string fileName = string.Format("print_{0}{1}{2}", dateTime, "barCodeLabel", ".pdf");
                fileName = fileName.Replace('/', '-').Replace(' ', '_');
                fileName = FrameworkUtils.OSSlash(string.Format(@"{0}{1}", GlobalFramework.Path["temp"], fileName));
                FastReport.Export.Pdf.PDFExport export = new FastReport.Export.Pdf.PDFExport();
                try
                {
                    customReport.Export(export, fileName);
                }
                catch (Exception ex)
                {
                    _log.Error("Error printing barcode label :: fileName [ " + fileName + " ]: " + ex.Message, ex);
                }

                System.Diagnostics.Process.Start(FrameworkUtils.OSSlash(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName)));
            }
                
            //customReport.Process(CustomReportDisplayMode.Print);

            customReport.Dispose();

            //customReport.Print();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //CopyNames

        //Get CopyNames String Array from integer List ex pCopyNames[1] = result[1] = Duplicate
        public static string[] CopyNames(List<int> pCopyNames)
        {
            string[] result = new string[pCopyNames.Count];
            for (int i = 0; i < pCopyNames.Count; i++)
            {
                result[i] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], string.Format("global_print_copy_title{0}", pCopyNames[i] + 1));
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

            result = FrameworkUtils.OSSlash(string.Format("{0}{1}\\{2}", GlobalFramework.Path["reports"], "UserReports", fileName));
            if (!File.Exists(result))
            {
                // Force Exception, Report must Exist else its hard to find errors, dont catch exception
                throw new Exception(string.Format("Error required File Not Found: [{0}]", fileName));
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
                    resourceStringPostfix = string.Format(" {0}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_sales_detail_group_postfix"));
                }
                // Detail
                else if (reportResourceString.StartsWith("report_sales_detail_"))
                {
                    // Remove extra chars from token to re use default resx
                    resourceString = reportResourceString.Replace("report_sales_detail_", "report_sales_");
                    resourceStringPostfix = string.Format(" {0}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_sales_detail_postfix"));
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
            if ((!string.IsNullOrEmpty(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], resourceString))))
            {
                resourceString = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], resourceString);
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