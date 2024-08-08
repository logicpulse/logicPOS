using FastReport;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Reporting.Utility;
using LogicPOS.Settings;
using LogicPOS.Utility;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Controls.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace LogicPOS.Reporting.Common
{
    public class FastReport : Report
    {
        protected const string LOGICPOS_EXECUTABLE = "logicpos.exe";
        public const string FILENAME_TEMPLATE_BASE = "TemplateBase.frx";
        public const string FILENAME_TEMPLATE_BASE_SIMPLE = "TemplateBaseSimple.frx";
        private static bool _secondCopy;
        private readonly bool _debug = false;
        private readonly bool _forceReleaseMode = false;
        private string _reportFileLocation = string.Empty;
        private bool _addCodeDataToReport = true;
        public string Hash4Chars { get; set; } = string.Empty;

        //FastReports Require Parameterless Constructor
        public FastReport() { }

        public FastReport(
            string reportFileName,
            string templateBase,
            List<int> copiesNumbers = null,
            int numberOfCopies = 1)
        {
            _reportFileLocation = FastReportUtils.GetReportFilePath(reportFileName);

            TryLoadReportFile();

            RegisterReportEvents();

            AddReferencedAssemblies();

            AddCopyNames(copiesNumbers);

            PrintSettings.Copies = numberOfCopies;
            AddReportVariables();

            if (XPOSettings.LoggedUser != null)
            {
                PrintingSettings.FastReportCustomVars["Session_loggerged_User"] = XPOSettings.LoggedUser.Name;
            }
        }

        private void AddReportVariables()
        {
            foreach (var item in PrintingSettings.FastReportSystemVars)
            {
                var systemVariable = new global::FastReport.Data.SystemVariable
                {
                    Name = item.Key,
                    AsString = item.Value
                };

                Dictionary.SystemVariables.Add(systemVariable);
            }
        }

        private void AddCopyNames(List<int> copiesNumbers)
        {
            if (copiesNumbers != null)
            {
                string[] copiesNames = PrintingUtils.GetDocumentsCopiesNamesByNumbers(copiesNumbers);
                PrintSettings.CopyNames = copiesNames;
            }
        }

        private void TryLoadReportFile()
        {
            if (File.Exists(_reportFileLocation) == false)
            {
                throw new FileNotFoundException(string.Format("Report File Not Found: {0}", _reportFileLocation));
            }
               
            Load(_reportFileLocation);
        }

        private void RegisterReportEvents()
        {
            StartReport += delegate
            {
                if (_addCodeDataToReport)
                {
                    AddCodeDataToReport();
                }
            };
        }

        private void AddReferencedAssemblies()
        {
            string[] referencedAssemblies = ReferencedAssemblies;

            if (File.Exists(LOGICPOS_EXECUTABLE))
            {
                Array.Resize(ref referencedAssemblies, ReferencedAssemblies.Length + 1);
                referencedAssemblies[referencedAssemblies.Length - 1] = LOGICPOS_EXECUTABLE;
            }
            ReferencedAssemblies = referencedAssemblies;
        }

        public string Process(CustomReportDisplayMode displayMode, string pDestinationFileName = "")
        {
            string result = string.Empty;

            //Prepare Modes
            switch (displayMode)
            {
                case CustomReportDisplayMode.Preview:
                case CustomReportDisplayMode.Print:
                case CustomReportDisplayMode.ExportPDF:
                case CustomReportDisplayMode.ExportPDFSilent:
                case CustomReportDisplayMode.ExportXls:
                    //Get Object Reference to Change CopyName
                    TextObject textCopyName = (TextObject)FindObject("TextCopyName");
                    //Get Object Reference to Change SecondPrint Label for DocumentFinanceDocuments
                    TextObject textSecondPrint = (TextObject)FindObject("TextSecondPrint");

                    //Loop Copies and Change CopyName
                    for (int i = 0; i < PrintSettings.CopyNames.Length; i++)
                    {
                        if (textCopyName != null)
                        {
                            textCopyName.Text = PrintSettings.CopyNames[i];
                        }
                        if (textSecondPrint != null)
                        {
                            textSecondPrint.Text = _secondCopy && i < 1 ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_print_second_print") : string.Empty;
                        }
                        //Store PreparedFiles in Custom SystemVariable, Required to PageNo in Reports ex "[ToInt32([PreparedPages]) + [Page]]"
                        //Else Page start aways in 1, when we call prepare, and we cannot have a usefull Page Counter working with .Prepare
                        Dictionary.SystemVariables.FindByName("PreparedPages").Value = PreparedPages != null ? PreparedPages.Count : 0;
                        //Call Report Prepare
                        Prepare(true);
                    }
                    break;
            }

            //NOT USED ANYMORE : Now we can Reset Copies to 1
            //this.PrintSettings.Copies = 1;

            //Send to ViewMode
            switch (displayMode)
            {
                case CustomReportDisplayMode.Preview:
                    ShowPrepared();
                    break;
                //Impressão A4 abria Janela de impressão Fast Report [IN:009341]
                //case CustomReportDisplayMode.Print:
                //    this.PrintPrepared();
                //    break;
                case CustomReportDisplayMode.Design:
                    Design();
                    break;
                case CustomReportDisplayMode.ExportPDF:
                case CustomReportDisplayMode.ExportPDFSilent:
                case CustomReportDisplayMode.ExportXls:
                case CustomReportDisplayMode.Print:
                    //Prepare FileName
                    string fileName = string.Empty;
                    string fileNameExport = string.Empty;
                    if (pDestinationFileName != string.Empty)
                    {
                        fileName = pDestinationFileName;
                        fileNameExport = pDestinationFileName;
                    }
                    //Default Filename
                    else
                    {
                        string dateTimeFileFormat = CultureSettings.FileFormatDateTime;
                        string dateTime = XPOUtility.CurrentDateTimeAtomic().ToString(dateTimeFileFormat);
                        string reportName = ReportInfo.Name != string.Empty ? string.Format("_{0}", ReportInfo.Name) : string.Empty;


                        fileNameExport = string.Format("print_{0}{1}{2}", dateTime, reportName, ".xlsx");
                        fileNameExport = fileNameExport.Replace('/', '-').Replace(' ', '_');
                        fileNameExport = string.Format(@"{0}{1}", PathsSettings.TempFolderLocation, fileNameExport);


                        fileName = string.Format("print_{0}{1}{2}", dateTime, reportName, ".pdf");
                        fileName = fileName.Replace('/', '-').Replace(' ', '_');
                        //2015-06-12 apmuga
                        fileName = string.Format(@"{0}{1}", PathsSettings.TempFolderLocation, fileName);

                        //Mario
                        //fileName = (LogicPOS.Settings.GeneralSettings.Settings["AppEnvironment"].ToUpper() == "web".ToUpper()) 
                        //    ? SharedUtils.OSSlash(string.Format(@"{0}{1}", GeneralSettings.Path["temp"], fileName))
                        //    : SharedUtils.OSSlash(string.Format(@"{0}\{1}{2}", Environment.CurrentDirectory, GeneralSettings.Path["temp"], fileName))
                        //;
                    }

                    //TK016206 Reports - Exportação para Xls/pdf

                    var exportXlsx = new global::FastReport.Export.OoXML.Excel2007Export();
                    //if (exportXlsx.ShowDialog())
                    //{
                    Export(exportXlsx, fileNameExport);
                    //}



                    //if (export.ShowDialog()) this.Export(export, fileName);
                    //TK016206 Reports - Exportação para Xls/pdf
                    //TK016249 - Impressoras - Diferenciação entre Tipos
                    if (displayMode == CustomReportDisplayMode.ExportPDFSilent || displayMode == CustomReportDisplayMode.ExportPDF || !PrintingSettings.ThermalPrinter.UsingThermalPrinter)
                    {
                        global::FastReport.Export.Pdf.PDFExport export = new global::FastReport.Export.Pdf.PDFExport();

                        Export(export, fileName);

                    }
                    //Show Printer Dialog on Windows
                    //Impressão A4 abria Janela de impressão Fast Report [IN:009341]
                    if (displayMode == CustomReportDisplayMode.Print && File.Exists(fileName))
                    {

                        string docPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName);

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
                                catch (Exception)
                                {
                                    //Printing was canceled
                                }
                            }
                        });
                        thread3.SetApartmentState(ApartmentState.STA);
                        thread3.Start();
                        thread3.Join();



                    }

                    ShowPdf(displayMode, fileName);

                    result = fileName;
                    break;
                default:
                    break;
            }
            return result;
        }

        private static void ShowPdf(
            CustomReportDisplayMode displayMode,
            string fileName)
        {
            if (displayMode == CustomReportDisplayMode.ExportPDF && File.Exists(fileName))
            {
                if (GeneralSettings.UsePosPDFViewer)
                {
                    string docPath = $@"{Environment.CurrentDirectory}\{fileName}";
                    var ScreenSizePDF = GeneralSettings.ScreenSize;
                    int widthPDF = ScreenSizePDF.Width;
                    int heightPDF = ScreenSizePDF.Height;
                    bool exportXls = true;

                    if (!PrintingSettings.ThermalPrinter.UsingThermalPrinter) exportXls = false;

                    Application.Run(new PDFViewer.Winforms.PDFViewer(docPath, widthPDF - 50, heightPDF - 20, exportXls));
                }
                else
                {
                    System.Diagnostics.Process.Start($@"{Environment.CurrentDirectory}\{fileName}");
                }
            }
        }

        public void AddCodeDataToReport()
        {
            PageFooterBand pageFooterBand = (PageFooterBand)FindObject("PageFooter1");

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
            //CultureInfo.CurrentUICulture = LogicPOS.Settings.CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["cultureFinancialRules"]);

            if (currentCulture != "pt-AO" && currentCulture != "pt-PT" && currentCulture != "pt-BR" && currentCulture != "pt-MZ")
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
            string documentFileName = Path.GetFileNameWithoutExtension(_reportFileLocation);
            bool isFinanceDocument = financeDocumentsList.Contains(documentFileName.Substring(documentFileName.LastIndexOf(".") + 1)); // "Resources/Reports/UserReports/ReportDocumentFinance_pt-MZ.frx"

            if (isFinanceDocument)
            {
                //Processed|Emitted with certified Software Nº {0}/AT - Copyright {1} - Licenced to a {2} - Used only if System Country is Portugal
                if (CultureSettings.CountryIdIsPortugal(XPOSettings.ConfigurationSystemCountry.Oid))
                {

                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = _reportFileLocation.EndsWith(fileName)
                        ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_emitted")
                        : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_processed")
                    ;
                    if (currentCulture == "pt-AO")
                    {
                        textObjectOverlaySoftwareCertification.Text = string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification"),
                        prefix,
                        SaftSettings.SaftSoftwareCertificateNumber,
                        SaftSettings.SaftProductID,
                        LicenseSettings.LicenseCompany,
                        "LOGICPULSE ANGOLA");
                    }
                    else
                    {
                        textObjectOverlaySoftwareCertification.Text = string.Format(
                            CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification"),
                            prefix,
                            SaftSettings.SaftSoftwareCertificateNumber,
                            SaftSettings.SaftProductID,
                            LicenseSettings.LicenseCompany);
                    }


                    //Add Hash Validation if Defined (In DocumentFinance Only)
                    if (Hash4Chars != string.Empty) textObjectOverlaySoftwareCertification.Text = string.Format("{0} - {1}", Hash4Chars, textObjectOverlaySoftwareCertification.Text);

                } /* IN005975 and IN005979 for Mozambique deployment */
                else if (CultureSettings.CountryIdIsMozambique(XPOSettings.ConfigurationSystemCountry.Oid))
                {
                    /* 
                     * IN006047
                     * {Processado por computador} || Autorização da Autoridade Tributária: {DAFM1 - 0198 / 2018}
                     */
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification"),
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_processed"),
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_moz_tax_authority_cert_number")
                    );
                }
                else if (CultureSettings.AngolaCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                {
                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_processed"); ;
                    string localDate = DateTime.Now.Year.ToString();
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_ao"),
                        prefix,
                        SaftSettings.SaftSoftwareCertificateNumberAO,
                        SaftSettings.SaftProductIDAO,
                        localDate);

                    //Add Hash Validation if Defined (In DocumentFinance Only)
                    if (Hash4Chars != string.Empty) textObjectOverlaySoftwareCertification.Text = string.Format("{0} - {1}", Hash4Chars, textObjectOverlaySoftwareCertification.Text);

                }
            }
            else
            {
                if (CultureSettings.AngolaCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                {
                    string fileName = "ReportDocumentFinancePayment_" + currentCulture + ".frx";
                    string prefix = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_emitted"); ;
                    string localDate = DateTime.Now.Year.ToString();
                    textObjectOverlaySoftwareCertification.Text = string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_report_overlay_software_certification_ao"),
                        prefix,
                        SaftSettings.SaftSoftwareCertificateNumberAO,
                        SaftSettings.SaftProductIDAO,
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
            _addCodeDataToReport = false;
        }

        public static string ProcessReportFinanceDocument(
            CustomReportDisplayMode viewMode,
            Guid financeMasterId,
            string hash4Chars,
            List<int> copyNumbers,
            bool secondCopy = false,
            string motive = "",
            string destFileName = "")
        {

            fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), financeMasterId);

            string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
            try
            {
                if (CultureInfo.CurrentUICulture.Name != currentCulture)
                {
                    //CultureInfo.CurrentUICulture = LogicPOS.Settings.CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                }
            }
            catch
            {
                currentCulture = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                //CultureInfo.CurrentUICulture = LogicPOS.Settings.CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
            }

            if (string.IsNullOrEmpty(currentCulture))
            {
                currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
            }

            if (currentCulture != "pt-AO" && currentCulture != "pt-PT" && currentCulture != "pt-BR" && currentCulture != "pt-MZ")
            {
                currentCulture = "pt-MZ";
            }

            //string currentCulture = LogicPOS.Settings.CultureSettings.CurrentCulture.Name;
            string reportFileName = documentMaster.DocumentType.WayBill ? "ReportDocumentFinanceWayBill_" + currentCulture + ".frx" : "ReportDocumentFinance_" + currentCulture + ".frx";
            //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
            if (Convert.ToBoolean(GeneralSettings.PreferenceParameters["PRINT_QRCODE"]) && XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.PortugalCountryId) && !string.IsNullOrEmpty(documentMaster.ATDocQRCode))
            {
                reportFileName = reportFileName.Replace(".frx", "_QRCode.frx");
            }

            FastReport customReport = new FastReport(reportFileName, FILENAME_TEMPLATE_BASE, copyNumbers);
            customReport.DoublePass = documentMaster.DocumentDetail.Count > 15;
            customReport.Hash4Chars = hash4Chars;
            customReport.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            customReport.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);
            //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
            customReport.SetParameterValue("ATDocQRCodeVisible", GeneralSettings.PreferenceParameters["PRINT_QRCODE"].ToString());
            customReport.SetParameterValue("ATDocQRCode", documentMaster.ATDocQRCode);

            //Report Parameters
            //customReport.SetParameterValue("Invoice Noº", 280);

            //Get Result Objects from FRBOHelper
            ReportDataList<FinanceMasterViewReportData> financeMasters = ReportDataHelper.GetFinanceMasterViewReportDataList(financeMasterId);
            //Get Generic Collections From FRBOHelper Results
            ReportDataList<FinanceMasterViewReportData> gcDocumentFinanceMaster = financeMasters;
            //Prepare and Enable DataSources
            customReport.RegisterData(gcDocumentFinanceMaster, "DocumentFinanceMaster");

            /* IN005976 for Mozambique deployment */
            if (XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.MozambiqueCountryId) || ConfigurationManager.AppSettings["cultureFinancialRules"] == "fr-CF")
            {
                //if (LogicPOS.Settings.CultureSettings.CurrentCulture.Name.Equals("pt-MZ")){
                cfg_configurationcurrency defaultCurrencyForExchangeRate =
                        XPOUtility.GetEntityById<cfg_configurationcurrency>(
                            CultureSettings.USDCurrencyId,
                            XPOSettings.Session);

                customReport.SetParameterValue("DefaultCurrencyForExchangeRateAcronym", defaultCurrencyForExchangeRate.Acronym);
                customReport.SetParameterValue("DefaultCurrencyForExchangeRateTotal", defaultCurrencyForExchangeRate.ExchangeRate);
                //}
            }

            if (customReport.GetDataSource("DocumentFinanceMaster") != null) customReport.GetDataSource("DocumentFinanceMaster").Enabled = true;
            if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceDetail").Enabled = true;
            if (customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal") != null) customReport.GetDataSource("DocumentFinanceMaster.DocumentFinanceMasterTotal").Enabled = true;


            if (XPOSettings.ConfigurationSystemCountry.Oid.Equals(CultureSettings.AngolaCountryId))
            {
                if (documentMaster.DocumentParent != null && documentMaster.DocumentType.Oid.ToString() == DocumentSettings.InvoiceAndPaymentId.ToString())
                {
                    documentMaster.Notes += string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_source_document") + ": " + documentMaster.DocumentParent.DocumentNumber);
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
            _secondCopy = secondCopy;

            int n = customReport.Pages.Count;

            //Add ReportInfo.Name, to be used for Ex in Pdf Filenames, OS etc
            customReport.ReportInfo.Name = gcDocumentFinanceMaster.List[0].DocumentNumber;
            string result = customReport.Process(viewMode, destFileName);
            customReport.Dispose();

            return result;

        }

        public static string ProcessReportFinanceDocumentPayment(
            CustomReportDisplayMode pViewMode, 
            Guid pDocumentFinancePaymentOid, 
            List<int> pCopyNames, 
            string pDestinationFileName = "")
        {
            //string fileUserReportDocumentFinancePayment = SharedUtils.OSSlash(string.Format("{0}{1}\\{2}", GeneralSettings.Path["reports"], "UserReports", "ReportDocumentFinancePayment.frx"));
            //TK016319 - Certificação Angola - Alterações para teste da AGT
            //Bug - Não eram utilizados templates por região nos recibos, apenas default
            string currentCulture = ConfigurationManager.AppSettings["cultureFinancialRules"];
            try
            {
                if (CultureInfo.CurrentUICulture.Name != currentCulture)
                {
                    //CultureInfo.CurrentUICulture = LogicPOS.Settings.CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
                }
            }
            catch
            {
                currentCulture = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                //CultureInfo.CurrentUICulture = LogicPOS.Settings.CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(currentCulture);
            }

            if (currentCulture != "pt-AO" || currentCulture != "pt-PT" || currentCulture != "pt-BR" || currentCulture != "pt-MZ")
            {
                currentCulture = "pt-MZ";
            }


            string fileFrxFromCulture = string.Format("ReportDocumentFinancePayment_" + currentCulture + ".frx");
            string fileUserReportDocumentFinancePayment = string.Format("{0}{1}\\{2}", PathsSettings.Paths["reports"], "UserReports", fileFrxFromCulture);

            FastReport customReport = new FastReport(fileUserReportDocumentFinancePayment, FILENAME_TEMPLATE_BASE, pCopyNames);

            //Get Result Objects from FRBOHelper
            ReportDataList<FinancePaymentViewReportData> financePayments = ReportDataHelper.GetFinancePaymentViewReportDataList(pDocumentFinancePaymentOid);
            //Get Generic Collections From FRBOHelper Results
            ReportDataList<FinancePaymentViewReportData> gcDocumentFinancePayment = financePayments;

            //Prepare and Enable DataSources
            customReport.RegisterData(gcDocumentFinancePayment, "DocumentFinancePayment");
            if (customReport.GetDataSource("DocumentFinancePayment") != null) customReport.GetDataSource("DocumentFinancePayment").Enabled = true;
            if (customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument") != null) customReport.GetDataSource("DocumentFinancePayment.DocumentFinancePaymentDocument").Enabled = true;
            customReport.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            customReport.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);
            //Add ReportInfo.Name, to be used for Ex in Pdf Filenames, OS etc
            customReport.ReportInfo.Name = gcDocumentFinancePayment.List[0].PaymentRefNo;
            string result = customReport.Process(pViewMode, pDestinationFileName);
            customReport.Dispose();

            return result;
        }

        public static string DocumentMasterCreatePDF(fin_documentfinancemaster pDocumentFinanceMaster)
        {
            //return DocumentMasterCreatePDF(pDocumentFinanceMaster, String.Empty, CustomReportDisplayMode.ExportPDFSilent);
            return DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, pDocumentFinanceMaster, string.Empty);
        }

        public static string DocumentMasterCreatePDF(
            CustomReportDisplayMode displayMode,
            fin_documentfinancemaster financeMaster,
            string destinationFileName)
        {
            string result = string.Empty;

            List<int> copiesNumbers = CopyNames(financeMaster.DocumentType.PrintCopies);
            string hash4Chars = CryptographyUtils.GetDocumentHash4Chars(financeMaster.Hash);
            destinationFileName = destinationFileName.Replace('\n'.ToString(), "");

            result = ProcessReportFinanceDocument(
                viewMode: displayMode,
                financeMasterId: financeMaster.Oid,
                hash4Chars: hash4Chars,
                copyNumbers: copiesNumbers,
                destFileName: destinationFileName);

            return result;
        }

        public static string DocumentPaymentCreatePDF(
            CustomReportDisplayMode pDisplayMode,
            fin_documentfinancepayment pDocumentFinancePayment,
            string pDestinationFileName)
        {
            string result = string.Empty;

            List<int> copyNames = CopyNames(pDocumentFinancePayment.DocumentType.PrintCopies);
            result = ProcessReportFinanceDocumentPayment(pDisplayMode, pDocumentFinancePayment.Oid, copyNames, pDestinationFileName);

            return result;
        }

        public static void ProcessReportBarcodeLabel(
            CustomReportDisplayMode pViewMode,
            fin_articleserialnumber pArticleSerialNumber,
            string pFooter,
            bool pBigSize,
            List<fin_articleserialnumber> pListArticleSerialNumber = null)
        {
            string reportFileName = !pBigSize
                   ? FastReportUtils.GetReportFilePath("BarCodeTemplate_100x50.frx")
                   : FastReportUtils.GetReportFilePath("BarCodeTemplate_40x30.frx")
               ;

            //Get template from selected article
            if (pArticleSerialNumber != null && pArticleSerialNumber.Article.TemplateBarCode != null)
            {
                reportFileName = pArticleSerialNumber.Article.TemplateBarCode.FileTemplate.ToString();
            }
            else if (pListArticleSerialNumber != null && pListArticleSerialNumber.Count > 0 && pListArticleSerialNumber[0].Article.TemplateBarCode != null)
            {
                reportFileName = pListArticleSerialNumber[0].Article.TemplateBarCode.FileTemplate.ToString();
            }

            reportFileName = Path.GetFileName(reportFileName);

            FastReport customReport = new FastReport(
                reportFileName: reportFileName,
                templateBase: "",
                numberOfCopies: 1);

            if (string.IsNullOrEmpty(pFooter)) pFooter = GeneralSettings.PreferenceParameters["COMPANY_WEBSITE"];

            //Report Parameters
            //customReport.SetParameterValue("SerialNumber", pArticleSerialNumber.SerialNumber);
            //customReport.SetParameterValue("ArticleName", pArticleSerialNumber.Article.Designation);
            //customReport.SetParameterValue("ArticleRef", pArticleSerialNumber.Article.Code);
            //customReport.SetParameterValue("footerText", pFooter

            List<ArticleSerialNumberReportData> fRBOArticleSerialNumbers = new List<ArticleSerialNumberReportData>();

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                if (pListArticleSerialNumber != null)
                {
                    foreach (var item in pListArticleSerialNumber)
                    {
                        fRBOArticleSerialNumbers.Add(new ArticleSerialNumberReportData() { SerialNumber = item.SerialNumber, ArticleName = item.Article.Designation, ArticleRef = "ref." + item.Article.Code, footerText = pFooter, Oid = item.Oid.ToString() });
                    }
                }
                else
                {
                    fRBOArticleSerialNumbers.Add(new ArticleSerialNumberReportData() { SerialNumber = pArticleSerialNumber.SerialNumber, ArticleName = pArticleSerialNumber.Article.Designation, ArticleRef = "ref." + pArticleSerialNumber.Article.Code, footerText = pFooter, Oid = pArticleSerialNumber.Oid.ToString() });
                }
            }
            //Prepare and Enable DataSources
            customReport.RegisterData(fRBOArticleSerialNumbers, "ArticleSerialNumber");
            //customReport.MaxPages = 1;
            //Scripts - Dont Delete this Comment, may be usefull if we remove Script from Report File
            //Print X Records per Data Band
            DataBand dataBand = (DataBand)customReport.FindObject("Data1");
            int dataBandRec = 1;
            //Used to Break Page on X Recs, Usefull to leave open space for Report Summary
            int dataBandMaxRecs = 30;

            dataBand.BeforePrint += delegate
            {
                var barcodeObject = dataBand.Objects[0] as global::FastReport.Barcode.BarcodeObject;
                barcodeObject.Text = (dataBand.DataSource.CurrentRow as ArticleSerialNumberReportData).SerialNumber;
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

            customReport.Dictionary.SystemVariables.FindByName("PreparedPages").Value = customReport.PreparedPages != null ? customReport.PreparedPages.Count : 0;
            //Call Report Prepare
            customReport.Prepare(true);

            customReport.ShowPrepared(true);


            //customReport.Process(CustomReportDisplayMode.Print);

            customReport.Dispose();

            //customReport.Print();
        }

        public static List<int> CopyNames(int printCopies)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < printCopies; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public static Tuple<string, string> GetResourceString(string reportResourceString)
        {
            string resourceString = string.Empty;
            string resourceStringPostfix = string.Empty;

            // Reuse Same resource String for 3 Sales Reports Default(Resx) report-sales-*, report-sales-detail-*, report-sales-detail-group-
            // Detail/Group
            if (reportResourceString.StartsWith("report_sales_detail_group_"))
            {//be first
             // Remove extra chars from token to re use default resx
                resourceString = reportResourceString.Replace("report_sales_detail_group_", "report_sales_");
                resourceStringPostfix = string.Format(" {0}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_sales_detail_group_postfix"));
            }
            // Detail
            else if (reportResourceString.StartsWith("report_sales_detail_"))
            {
                // Remove extra chars from token to re use default resx
                resourceString = reportResourceString.Replace("report_sales_detail_", "report_sales_");
                resourceStringPostfix = string.Format(" {0}", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_sales_detail_postfix"));
            }
            // Default
            else
            {
                resourceString = reportResourceString;
                resourceStringPostfix = string.Empty;
            }


            // Get Resource Content for all modes
            if (!string.IsNullOrEmpty(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, resourceString)))
            {
                resourceString = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, resourceString);
            }
            else
            {
                resourceString = string.Format("Error: Can't find resourceString:[{0}]", resourceString);
                throw new Exception(resourceString);
            }

            return new Tuple<string, string>(resourceString, resourceStringPostfix);
        }

        public static string GenerateDocumentFinanceMasterPDFIfNotExists(fin_documentfinancemaster documentFinanceMaster)
        {
            string result = string.Empty;


            //Generate Documents Filename
            if (!string.IsNullOrEmpty(GeneralSettings.Settings["generatePdfDocuments"]))
            {
                bool generatePdfDocuments = false;

                generatePdfDocuments = Convert.ToBoolean(GeneralSettings.Settings["generatePdfDocuments"]);


                if (generatePdfDocuments)
                {
                    string entityName = !string.IsNullOrEmpty(documentFinanceMaster.EntityName)
                        ? string.Format("_{0}", PluginSettings.SoftwareVendor.Decrypt(documentFinanceMaster.EntityName).ToLower().Replace(' ', '_')) /* IN009075 */
                        : string.Empty;
                    string reportFilename = string.Format("{0}/{1}{2}.pdf",
                        PathsSettings.Paths["documents"],
                        documentFinanceMaster.DocumentNumber.Replace('/', '-').Replace(' ', '_'),
                        entityName
                    );
                    //Canceled documents with "Canceled" Text on PDF
                    if (!File.Exists(reportFilename) || documentFinanceMaster.DocumentStatusStatus == "A")
                    {
                        result = DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, documentFinanceMaster, reportFilename);
                    }
                    else
                    {
                        result = reportFilename;
                    }

                    return result;
                }
            }


            return result;
        }

        public static string GenerateDocumentFinancePaymentPDFIfNotExists(fin_documentfinancepayment documentFinancePayment)
        {
            string result = string.Empty;

            //Generate Documents Filename
            if (!string.IsNullOrEmpty(GeneralSettings.Settings["generatePdfDocuments"]))
            {
                bool generatePdfDocuments = false;

                generatePdfDocuments = Convert.ToBoolean(GeneralSettings.Settings["generatePdfDocuments"]);


                if (generatePdfDocuments)
                {
                    erp_customer customer = XPOUtility.GetEntityById<erp_customer>(documentFinancePayment.EntityOid);
                    string entityName = customer != null && !string.IsNullOrEmpty(customer.Name) ? string.Format("_{0}", customer.Name.ToLower().Replace(' ', '_')) : string.Empty;
                    string reportFilename = $"{PathsSettings.Paths["documents"]}/{documentFinancePayment.PaymentRefNo.Replace('/', '-').Replace(' ', '_')}{entityName}.pdf";

                    if (!File.Exists(reportFilename))
                    {
                        result = DocumentPaymentCreatePDF(CustomReportDisplayMode.ExportPDFSilent, documentFinancePayment, reportFilename);
                    }
                    else
                    {
                        result = reportFilename;
                    }

                    return result;
                }
            }


            return result;
        }
    }
}