using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using System.Drawing;
using System.IO;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using Image = Gtk.Image;
using logicpos.Classes.Gui.Gtk.Widgets;
using Pango;
using Color = System.Drawing.Color;
using Alignment = Gtk.Alignment;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.financial.library.Classes.Reports;
using Medsphere.Widgets;
using System.Collections;
using DevExpress.Xpo.DB;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class DashBoard : GenericTreeViewXPO
    {
        //private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Botões do DashBoard
        private TouchButtonIconWithText botao1;
        private TouchButtonIconWithText botao2;
        private TouchButtonIconWithText botao3;
        private TouchButtonIconWithText botao4;
        private TouchButtonIconWithText botao5;
        private TouchButtonIconWithText botao6;
        private TouchButtonIconWithText botao7;
        private TouchButtonIconWithText botao8;
        private TouchButtonIconWithText botao9;
        private TouchButtonIconWithText botao10;
        private TouchButtonIconWithText botao11;
        private TouchButtonIconWithText botao12;
        private TouchButtonIconWithText botao13;
        private TouchButtonIconWithText botao14;
        private TouchButtonIconWithText botao15;
        private TouchButtonIconWithText botao16;

        ICollection collectionDocuments = null;
        private readonly string creditNoteOid = SettingsApp.XpoOidDocumentFinanceTypeCreditNote.ToString();
        private readonly string invoiceOid = SettingsApp.XpoOidDocumentFinanceTypeInvoice.ToString();
        private readonly string simpleInvoiceOid = SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice.ToString();
        private readonly string invoiceAndPaymentOid = SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment.ToString();

        public string eventBackGround = "Backgrounds/Windows/LogicPOS_WorkFlow.png";

        //Cores usadas nos gráficos
        public PlotColor[] plotColors = new PlotColor[] {
            PlotColor.Red,
            PlotColor.Blue,
            PlotColor.Green,
            PlotColor.Orange,
            PlotColor.Purple,
            PlotColor.Brown,
            PlotColor.DarkRed,
            PlotColor.DarkBlue,
            PlotColor.DarkGreen,
            PlotColor.DarkOrange,
            PlotColor.DarkPurple,
            PlotColor.DarkYellow,
            PlotColor.DarkBrown
        };

        public ComboBox selAno;
        Graph newGraph = new Graph2D();

        //Public Parametless Constructor Required by Generics
        public DashBoard()
        {

        }

        public DashBoard(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //ScreenArea
        protected EventBox _eventboxDashboard;
        protected Color _colorBaseDialogDefaultButtonFont = FrameworkUtils.StringToColor("76, 72, 70");
        protected Color _colorBaseDialogDefaultButtonBackground = FrameworkUtils.StringToColor("156, 191, 42");
        protected Color _colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor("0, 0, 0");
        protected Color _colorBaseDialogActionAreaButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonBackground"]);
        //protected String _fontBaseDialogButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogButton"]);
        protected String _fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
        protected String _fileActionDefault = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");
        protected String _fileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
        protected String _fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
        //Colors
        Color colorBackOfficeContentBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeContentBackground"]);
        Color colorBackOfficeStatusBarBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBackground"]);
        Color colorBackOfficeAccordionFixBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeAccordionFixBackground"]);
        Color colorBackOfficeStatusBarFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarFont"]);
        Color colorBackOfficeStatusBarBottomBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBackOfficeStatusBarBottomBackground"]);
        public Color slateBlue = Color.FromName("White");
        //private Frame frame;
        private Label label;

        public DashBoard(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);


            Color screenBackgroundColor = FrameworkUtils.StringToColor(themeWindow.Globals.ScreenBackgroundColor);
            Color white = System.Drawing.Color.White;
            Color black = System.Drawing.Color.Black;


            //_log.Debug("Theme Background: " + eventBackGround);
            //Shared error Message
            string errorMessage = "Node: <Window ID=\"PosBaseWindow\">";

            Fixed fix = new Fixed();
            HBox hbox = new HBox();
            Frame frame = new Frame();

            VBox vbox = new VBox(false, 2);
            VBox vbox2 = new VBox(true, 0);
            VBox vbox3 = new VBox(false, 5);

            DateTime datenow = new DateTime();

            //Icons dos botões do dashboard
            String _fileFiscalYearIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_configurations.png");
            String _fileInsertFiscalYear = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_fiscal_year.png");
            String _fileInsertIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_printer.png");
            String _fileTerminalsIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_terminals.png");

            String _fileArticlesIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_articles.png");
            String _fileCostumersIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_costumers.png");
            String _fileEmployeesIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_employees.png");
            String _fileOtherTablesIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_other_tables.png");

            String _fileDocumentsIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_documents.png");
            String _fileNewDocumentIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_documents_new.png");
            String _filePayedDocumentsIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_documents_new.png");
            String _fileInsertMerchIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_documents_merch.png");

            String _fileReportsMenuIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_reports.png");
            String _fileReportsTotalIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_reports_sales_report.png");
            String _fileReportsClientsIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_reports_sales_client.png");
            String _fileReportsDayIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\BackOffice\icon_reports_sales_day.png");

            //Tamanho dos Icons e da Font do Texto dos botões
            Size sizeIcon = new Size(35, 35);
            string _fontBaseDialogButton = "8";
            //uint borderWidth = 5;
            //Cria o evento por trás da dashboard, tudo será carregado para aqui
            _eventboxDashboard = new EventBox();
            //_eventboxDashboard.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(screenBackgroundColor));
            _eventboxDashboard.WidthRequest = GlobalApp.boScreenSize.Width;
            _eventboxDashboard.HeightRequest = GlobalApp.boScreenSize.Height;
            Alignment _alignmentWindow = new Alignment(0.0f, 0.0f, 0.0f, 0.0f);
            _alignmentWindow.Add(_eventboxDashboard);
            Add(_alignmentWindow);
            try
            {
                //Imagem carregada aqui para o dashboard
                string fileImageBack = FrameworkUtils.OSSlash(string.Format("{0}Default/Backgrounds/Windows/LogicPOS_WorkFlow_{1}.png", GlobalFramework.Path["themes"], GlobalFramework.Settings["customCultureResourceDefinition"]));
                System.Drawing.Image pImage = System.Drawing.Image.FromFile(fileImageBack);
                Gdk.Pixbuf pixbuf = Utils.ImageToPixbuf(pImage);
                _eventboxDashboard.Style = Utils.GetImageBackgroundDashboard(pixbuf);
                //Buttons Configuração
                botao1 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_edit_ConfigurationPlaceTerminal_tab1_label"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileTerminalsIcon, sizeIcon, 105, 70);
                botao2 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_application_setup"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileFiscalYearIcon, sizeIcon, 105, 70);
                botao3 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_years_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertFiscalYear, sizeIcon, 105, 70);
                botao4 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_printers"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertIcon, sizeIcon, 105, 70);

                //Buttons Tabelas
                botao5 = new TouchButtonIconWithText("BACKOFFICE_MAN_ARTICLE_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_articles"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileArticlesIcon, sizeIcon, 105, 70);
                botao6 = new TouchButtonIconWithText("BACKOFFICE_MAN_CUSTOMER_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customers"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileCostumersIcon, sizeIcon, 105, 70);
                botao7 = new TouchButtonIconWithText("BACKOFFICE_MAN_USERDETAIL_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_users"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileEmployeesIcon, sizeIcon, 105, 70);
                botao8 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_other_tables"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileOtherTablesIcon, sizeIcon, 105, 70);

                //Buttons Documentos
                botao9 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSSHOW_MENU", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_resume_finance_documents"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileNewDocumentIcon, sizeIcon, 105, 70);
                botao10 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSNEW_MENU", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_new_document"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileDocumentsIcon, sizeIcon, 105, 70);
                botao11 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_button_label_select_payments"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _filePayedDocumentsIcon, sizeIcon, 105, 70);
                botao12 = new TouchButtonIconWithText("STOCK_MERCHANDISE_ENTRY_ACCESS", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentticket_type_title_cs_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertMerchIcon, sizeIcon, 105, 70);

                //Buttons Relatórios
                botao13 = new TouchButtonIconWithText("REPORT_ACCESS", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_reports"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsMenuIcon, sizeIcon, 105, 70);
                botao14 = new TouchButtonIconWithText("REPORT_COMPANY_BILLING", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_company_billing_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsTotalIcon, sizeIcon, 105, 70);
                botao15 = new TouchButtonIconWithText("REPORT_CUSTOMER_BALANCE_DETAILS", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_customer_balance_details_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsClientsIcon, sizeIcon, 105, 70);
                botao16 = new TouchButtonIconWithText("REPORT_SALES_DETAIL_PER_DATE", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "report_sales_per_date"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsDayIcon, sizeIcon, 105, 70);

                PosReportsDialog reportsClicked = new PosReportsDialog();

                //Permissões dos botões
                botao1.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
                botao2.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
                botao3.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
                botao4.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW");

                botao5.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_ARTICLE_VIEW");
                botao6.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_CUSTOMER_VIEW");
                botao7.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_USERDETAIL_VIEW");
                botao8.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");

                botao9.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
                botao10.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
                botao11.Sensitive = FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
                botao12.Sensitive = FrameworkUtils.HasPermissionTo("STOCK_MERCHANDISE_ENTRY_ACCESS");

                //Este fica comentado, porque o próprio menu dos reports tem controlo de previlégios
                //botao13.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_ACCESS");
                botao14.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_COMPANY_BILLING");
                botao15.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_CUSTOMER_BALANCE_DETAILS");
                botao16.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_SALES_DETAIL_PER_DATE");


                //Actions Configurações
                botao1.Clicked += delegate { botao1.Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTerminal>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao1, null); };
                botao2.Clicked += delegate { botao2.Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao2, null); };
                botao3.Clicked += delegate { botao3.Content = Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYears>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao3, null); };
                botao4.Clicked += delegate { botao4.Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrinters>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao4, null); };

                //Actions Tabelas
                botao5.Clicked += delegate { botao5.Content = Utils.GetGenericTreeViewXPO<TreeViewArticle>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao5, null); };
                botao6.Clicked += delegate { botao6.Content = Utils.GetGenericTreeViewXPO<TreeViewCustomer>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao6, null); };
                botao7.Clicked += delegate { botao7.Content = Utils.GetGenericTreeViewXPO<TreeViewUser>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao7, null); };
                botao8.Clicked += delegate { botao8.Content = Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTable>(pSourceWindow); GlobalApp.WindowBackOffice._dashboardButton_Clicked(botao8, null); };

                //Actions Documents
                botao9.Clicked += delegate { Utils.startDocumentsMenuFromBackOffice(pSourceWindow, 0); };
                botao10.Clicked += delegate { Utils.startNewDocumentFromBackOffice(pSourceWindow); };
                botao11.Clicked += delegate { Utils.startDocumentsMenuFromBackOffice(pSourceWindow, 3); };
                botao12.Clicked += delegate { Utils.startDocumentsMenuFromBackOffice(pSourceWindow, 6); };

                //Actions Reports
                botao13.Clicked += delegate { Utils.startReportsMenuFromBackOffice(pSourceWindow); };
                botao14.Clicked += delegate { reportsClicked.PrintReportRouter
                    (botao14, null); };
                botao15.Clicked += delegate { reportsClicked.PrintReportRouter(botao15, null); };
                botao16.Clicked += delegate { reportsClicked.PrintReportRouter(botao16, null); };

                //Posição dos botões na dashboard
                fix.Put(botao1, 55, 62);
                fix.Put(botao2, 55, 155);
                fix.Put(botao3, 55, 250);
                fix.Put(botao4, 55, 345);

                fix.Put(botao5, 245, 62);
                fix.Put(botao6, 245, 155);
                fix.Put(botao7, 245, 250);
                fix.Put(botao8, 245, 345);

                fix.Put(botao9, 440, 62);
                fix.Put(botao10, 440, 155);
                fix.Put(botao11, 440, 250);
                fix.Put(botao12, 440, 345);

                fix.Put(botao13, 635, 62);
                fix.Put(botao14, 635, 155);
                fix.Put(botao15, 635, 250);
                fix.Put(botao16, 635, 345);

                string currency = "Money";
                try
                {
                    string sqlCurrency = "SELECT Value FROM cfg_configurationpreferenceparameter where Token = 'SYSTEM_CURRENCY'";
                    currency = GlobalFramework.SessionXpo.ExecuteScalar(sqlCurrency).ToString();
                }
                catch
                {
                    currency = SettingsApp.SaftCurrencyCode;
                }

                decimal dailyTotal = 0;
                decimal MonthlyTotal = 0;
                decimal annualTotal = 0;
                ArrayList values = new ArrayList();
                values.Add(DateTime.Now.Year.ToString());
                try
                {
                    SortingCollection sortCollection = new SortingCollection();
                    sortCollection.Add(new SortProperty("Date", SortingDirection.Ascending));
                    CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL AND (DocumentType.Oid = '{0}' OR DocumentType.Oid = '{1}' OR DocumentType.Oid = '{2}' OR DocumentType.Oid = '{3}') AND DocumentStatusReason != 'A')", invoiceOid, invoiceAndPaymentOid, simpleInvoiceOid, creditNoteOid));
                    collectionDocuments = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(fin_documentfinancemaster)), criteria, sortCollection, int.MaxValue, false, true);

                    datenow = DateTime.Now;

                    foreach (fin_documentfinancemaster item in collectionDocuments)
                    {
                        //Faturação por Dia
                        if (item.Date.Day == datenow.Day && item.Date.Month == datenow.Month && item.Date.Year == datenow.Year)
                        {
                            if (item.DocumentType.Oid.ToString() == creditNoteOid && item.DocumentStatusStatus != "A")
                            {
                                dailyTotal -= Convert.ToDecimal(item.TotalFinal);
                            }
                            else if (item.DocumentStatusStatus != "A" && (item.DocumentType.Oid.ToString() == invoiceOid || item.DocumentType.Oid.ToString() == invoiceAndPaymentOid || item.DocumentType.Oid.ToString() == simpleInvoiceOid))
                                dailyTotal += Convert.ToDecimal(item.TotalFinal);
                        }
                        //Faturação por Mês
                        if (item.Date.Month == datenow.Month && item.Date.Year == datenow.Year)
                        {
                            if (item.DocumentType.Oid.ToString() == creditNoteOid && item.DocumentStatusStatus != "A")
                            {
                                MonthlyTotal -= Convert.ToDecimal(item.TotalFinal);
                            }
                            else if (item.DocumentStatusStatus != "A" && (item.DocumentType.Oid.ToString() == invoiceOid || item.DocumentType.Oid.ToString() == invoiceAndPaymentOid || item.DocumentType.Oid.ToString() == simpleInvoiceOid))
                                MonthlyTotal += Convert.ToDecimal(item.TotalFinal);
                        }
                        //Faturação por Ano
                        if (item.Date.Year == datenow.Year)
                        {
                            if (item.DocumentType.Oid.ToString() == creditNoteOid && item.DocumentStatusStatus != "A")
                            {
                                annualTotal -= Convert.ToDecimal(item.TotalFinal);
                            }
                            else if (item.DocumentStatusStatus != "A" && (item.DocumentType.Oid.ToString() == invoiceOid || item.DocumentType.Oid.ToString() == invoiceAndPaymentOid || item.DocumentType.Oid.ToString() == simpleInvoiceOid))
                                annualTotal += Convert.ToDecimal(item.TotalFinal);
                        }
                        //grava anos que existe faturação 
                        if (!values.Contains(item.Date.Year.ToString()))
                        {
                            values.Add(item.Date.Year.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }

                label = new Label();
                frame.ShadowType = (ShadowType)0;

                label.Text = string.Format("{0} {3}\n\n{1} {3}\n\n{2} {3}",
                    Convert.ToInt64(Math.Round(dailyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(MonthlyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(annualTotal, 0)).ToString(),
                    currency.ToString());

                label.ModifyFont(FontDescription.FromString("Trebuchet MS 16"));
                label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(white));
                label.Justify = Justification.Right;
                frame.Add(label);
                hbox.PackStart(frame, false, false, 0);
                vbox.PackStart(hbox, false, false, 0);
                fix.Put(vbox, 628, 515);

                //COMBO BOX selecionar os anos do gráfico
                int w = 1;
                string[] getYears = new string[values.Count];
                getYears[0] = (string)values[0];
                for (int i = values.Count - 1; i > 0; i--)
                {
                    getYears[i] = (string)values[w];
                    w++;
                }
                //w = 1;
                selAno = new ComboBox(getYears);
                selAno.ModifyFg(StateType.Selected, Utils.ColorToGdkColor(black));

                TreeIter iter;
                selAno.Model.GetIterFirst(out iter);
                do
                {
                    GLib.Value thisRow = new GLib.Value();
                    selAno.Model.GetValue(iter, 0, ref thisRow);
                    if ((thisRow.Val as string).Equals(getYears[0]))
                    {
                        selAno.SetActiveIter(iter);
                        break;
                    }

                } while (selAno.Model.IterNext(ref iter));
                selAno.Changed += delegate
               {
                   annualTotal = 0;
                   foreach (fin_documentfinancemaster item in collectionDocuments)
                   {
                       if (item.Date.Year.ToString() == selAno.ActiveText.ToString())
                       {
                           if (item.DocumentType.Oid.ToString() == creditNoteOid && item.DocumentStatusStatus != "A")
                           {
                               annualTotal -= Convert.ToDecimal(item.TotalFinal);
                           }
                           else if (item.DocumentStatusStatus != "A" && (item.DocumentType.Oid.ToString() == invoiceOid || item.DocumentType.Oid.ToString() == invoiceAndPaymentOid || item.DocumentType.Oid.ToString() == simpleInvoiceOid))
                               annualTotal += Convert.ToDecimal(item.TotalFinal);
                       }
                   }
                   label.Text = string.Format("{0} {3}\n\n{1} {3}\n\n{2} {3}",
                    Convert.ToInt64(Math.Round(dailyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(MonthlyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(annualTotal, 0)).ToString(),
                    currency.ToString());

                   label.ModifyFont(FontDescription.FromString("Trebuchet MS 16"));
                   label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(white));
                   label.Justify = Justification.Right;
                   frame.Add(label);

                   hbox.PackStart(frame, false, false, 0);
                   vbox.PackStart(hbox, false, false, 0);
                   string selectedDate = string.Format("01/01/{0}", (selAno.ActiveText.ToString()));
                   fix.Put(vbox, 640, 515);
                   fix.Put(drawSalesGraphic(DateTime.Parse(selectedDate), true), 55, 485);

               };
                if (Utils.IsLinux)
                {
                    fix.Put(selAno, 220, 650);
                }
                else fix.Put(selAno, 220, 665);

                //GRÁFICO         
                fix.Put(drawSalesGraphic(datenow, false), 55, 485);

                //Adiciona tudo ao evento principal
                _eventboxDashboard.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                _eventboxDashboard.Add(fix);
                fix.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                Utils.ShowMessageTouchErrorRenderTheme(pSourceWindow, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
            }
        }
        /// <summary>
        /// Inicialização do Gráfico GTK Widget
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cleanGraph"></param>
        /// <returns></returns>
        private Widget drawSalesGraphic(DateTime date, bool cleanGraph)
        {
            if (cleanGraph) newGraph.Clear();
            HBox hboxGraph = new HBox(false, 0);
            DateTimeAxis dtA = new DateTimeAxis(0, AxisLocation.Bottom);
            dtA.Padding = 5;
            dtA.ShowGridLines = false;
            dtA.ShowTicks = true;
            dtA.ShowTickLabels = true;

            newGraph.AppendAxis(dtA);
            newGraph.AppendAxis(new LinearAxis(1, AxisLocation.Left));
            HistogramPlot(newGraph, date);
            newGraph.CreatePangoContext();
            newGraph.ModifyBg(StateType.Normal, new Gdk.Color(218, 218, 218));
            newGraph.ModifyFg(StateType.Normal, new Gdk.Color(100, 100, 100));
            newGraph.WidthRequest = 515;
            newGraph.HeightRequest = 170;
            hboxGraph.PackStart(newGraph, false, false, 0);
            return hboxGraph;
        }

        /// <summary>
        /// Constroi o Gráfico do tipo HistogramPlot
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="sql"></param>
        public void HistogramPlot(Graph graph, DateTime date)
        {

            PlotColor plotColors = new PlotColor();
            plotColors = PlotColor.Green;
            HistogramPlot plot = new HistogramPlot(
                CreateModel(date),
                plotColors);

            plot.Name = "Vendas por Mês";
            plot.ShowValues = true;

            plot.SetValueDataColumn(0, 0);
            plot.SetValueDataColumn(1, 1);
            graph.AddPlot(plot, graph.Axes);
        }

        /// <summary>
        /// Criar o modelo de dados do gráfico
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public TreeStore CreateModel(DateTime year)
        {
            TreeStore store = new TreeStore(typeof(DateTime), typeof(double));

            decimal totalMes = 0;
            string date;
            DateTime parsedDate = new DateTime();
            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    totalMes = 0;
                    foreach (fin_documentfinancemaster item in collectionDocuments)
                    {
                        //Faturação por Mes/Ano
                        if (item.Date.Year == year.Year)
                        {
                            //Faturação por mês
                            if (item.Date.Month == i)
                            {
                                if (item.DocumentType.Oid.ToString() == creditNoteOid && item.DocumentStatusStatus != "A")
                                {
                                    totalMes -= Convert.ToDecimal(item.TotalFinal);
                                }
                                else if (item.DocumentStatusStatus != "A" && (item.DocumentType.Oid.ToString() == invoiceOid || item.DocumentType.Oid.ToString() == invoiceAndPaymentOid || item.DocumentType.Oid.ToString() == simpleInvoiceOid))
                                    totalMes += Convert.ToDecimal(item.TotalFinal);
                            }
                        }
                    }
                    totalMes = Math.Round(totalMes, 0);
                    date = string.Format("01/{0}/{1}", i, year.Year.ToString());
                    parsedDate = DateTime.Parse(date);
                    store.AppendValues(parsedDate, Convert.ToDouble(totalMes));
                }
                return store;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                store = null;
                return store;
            }
        }
    }
}
