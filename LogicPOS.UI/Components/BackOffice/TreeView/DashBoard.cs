using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using LogicPOS.Settings;
using Medsphere.Widgets;
using Pango;
using System;
using System.Collections;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using Alignment = Gtk.Alignment;
using Color = System.Drawing.Color;
using LogicPOS.Globalization;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Data.XPO.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class DashBoard : GenericTreeViewXPO
    {
        //private log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Botões do DashBoard
        private readonly TouchButtonIconWithText botao1;
        private readonly TouchButtonIconWithText botao2;
        private readonly TouchButtonIconWithText botao3;
        private readonly TouchButtonIconWithText botao4;
        private readonly TouchButtonIconWithText botao5;
        private readonly TouchButtonIconWithText botao6;
        private readonly TouchButtonIconWithText botao7;
        private readonly TouchButtonIconWithText botao8;
        private readonly TouchButtonIconWithText botao9;
        private readonly TouchButtonIconWithText botao10;
        private readonly TouchButtonIconWithText botao11;
        private readonly TouchButtonIconWithText botao12;
        private readonly TouchButtonIconWithText botao13;
        private readonly TouchButtonIconWithText botao14;
        private readonly TouchButtonIconWithText botao15;
        private readonly TouchButtonIconWithText botao16;
        private readonly ICollection collectionDocuments = null;
        private readonly string creditNoteOid = CustomDocumentSettings.CreditNoteDocumentTypeId.ToString();
        private readonly string invoiceOid = InvoiceSettings.XpoOidDocumentFinanceTypeInvoice.ToString();
        private readonly string simpleInvoiceOid = DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice.ToString();
        private readonly string invoiceAndPaymentOid = DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment.ToString();

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
        private readonly Graph newGraph = new Graph2D();

        //Public Parametless Constructor Required by Generics
        public DashBoard()
        {

        }

        public DashBoard(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { _sourceWindow = pSourceWindow; }


        //ScreenArea
        protected EventBox _eventboxDashboard;
        protected Color _colorBaseDialogDefaultButtonFont = ("76, 72, 70").StringToColor();
        protected Color _colorBaseDialogDefaultButtonBackground = ("156, 191, 42").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonFont = ("0, 0, 0").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonBackground = GeneralSettings.Settings["colorBaseDialogActionAreaButtonBackground"].StringToColor();
        //protected String _fontBaseDialogButton = SharedUtils.OSSlash(LogicPOS.Settings.GeneralSettings.Settings["fontBaseDialogButton"]);
        protected string _fontBaseDialogActionAreaButton = GeneralSettings.Settings["fontBaseDialogActionAreaButton"];
        protected string _fileActionDefault = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_default.png";
        protected string _fileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        protected string _fileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";

        //Colors
        private readonly Color colorBackOfficeContentBackground = GeneralSettings.Settings["colorBackOfficeContentBackground"].StringToColor();
        private readonly Color colorBackOfficeStatusBarBackground = GeneralSettings.Settings["colorBackOfficeStatusBarBackground"].StringToColor();
        private readonly Color colorBackOfficeAccordionFixBackground = GeneralSettings.Settings["colorBackOfficeAccordionFixBackground"].StringToColor();
        private readonly Color colorBackOfficeStatusBarFont = GeneralSettings.Settings["colorBackOfficeStatusBarFont"].StringToColor();
        private readonly Color colorBackOfficeStatusBarBottomBackground = GeneralSettings.Settings["colorBackOfficeStatusBarBottomBackground"].StringToColor();
        public Color slateBlue = Color.FromName("White");
        //private Frame frame;
        private readonly Label label;

        public DashBoard(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GeneralSettings.Settings["fontGenericTreeViewColumn"]);
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            _sourceWindow = pSourceWindow;

            Color screenBackgroundColor = (themeWindow.Globals.ScreenBackgroundColor as string).StringToColor();
            Color white = Color.White;
            Color black = Color.Black;


            //_logger.Debug("Theme Background: " + eventBackGround);
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
            string _fileFiscalYearIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_configurations.png";
            string _fileInsertFiscalYear = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_fiscal_year.png";
            string _fileInsertIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_printer.png";
            string _fileTerminalsIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_terminals.png";

            string _fileArticlesIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_articles.png";
            string _fileCostumersIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_costumers.png";
            string _fileEmployeesIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_employees.png";
            string _fileOtherTablesIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_other_tables.png";

            string _fileDocumentsIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_documents.png";
            string _fileNewDocumentIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_documents_new.png";
            string _filePayedDocumentsIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_documents_new.png";
            string _fileInsertMerchIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_documents_merch.png";

            string _fileReportsMenuIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_reports.png";
            string _fileReportsTotalIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_reports_sales_report.png";
            string _fileReportsClientsIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_reports_sales_client.png";
            string _fileReportsDayIcon = PathsSettings.ImagesFolderLocation + @"Icons\BackOffice\icon_reports_sales_day.png";

            //Tamanho dos Icons e da Font do Texto dos botões
            Size sizeIcon = new Size(35, 35);
            string _fontBaseDialogButton = "8";
            //uint borderWidth = 5;
            //Cria o evento por trás da dashboard, tudo será carregado para aqui
            _eventboxDashboard = new EventBox();
            //_eventboxDashboard.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(screenBackgroundColor));
            _eventboxDashboard.WidthRequest = GlobalApp.BoScreenSize.Width;
            _eventboxDashboard.HeightRequest = GlobalApp.BoScreenSize.Height;
            Alignment _alignmentWindow = new Alignment(0.0f, 0.0f, 0.0f, 0.0f)
            {
                _eventboxDashboard
            };
            Add(_alignmentWindow);
            try
            {
                //Imagem carregada aqui para o dashboard
                string fileImageBack = string.Format("{0}Default/Backgrounds/Windows/LogicPOS_WorkFlow_{1}.png", PathsSettings.Paths["themes"], CultureSettings.CurrentCultureName);
                System.Drawing.Image pImage = System.Drawing.Image.FromFile(fileImageBack);
                Gdk.Pixbuf pixbuf = logicpos.Utils.ImageToPixbuf(pImage);
                _eventboxDashboard.Style = logicpos.Utils.GetImageBackgroundDashboard(pixbuf);
                //Buttons Configuração
                botao1 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_edit_ConfigurationPlaceTerminal_tab1_label"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileTerminalsIcon, sizeIcon, 105, 70);
                botao2 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_application_setup"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileFiscalYearIcon, sizeIcon, 105, 70);
                botao3 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentfinance_years_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertFiscalYear, sizeIcon, 105, 70);
                botao4 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_printers"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertIcon, sizeIcon, 105, 70);

                //Buttons Tabelas
                botao5 = new TouchButtonIconWithText("BACKOFFICE_MAN_ARTICLE_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_articles"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileArticlesIcon, sizeIcon, 105, 70);
                botao6 = new TouchButtonIconWithText("BACKOFFICE_MAN_CUSTOMER_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_customers"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileCostumersIcon, sizeIcon, 105, 70);
                botao7 = new TouchButtonIconWithText("BACKOFFICE_MAN_USERDETAIL_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_users"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileEmployeesIcon, sizeIcon, 105, 70);
                botao8 = new TouchButtonIconWithText("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_other_tables"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileOtherTablesIcon, sizeIcon, 105, 70);

                //Buttons Documentos
                botao9 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSSHOW_MENU", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_worksession_resume_finance_documents"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileNewDocumentIcon, sizeIcon, 105, 70);
                botao10 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSNEW_MENU", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_new_document"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileDocumentsIcon, sizeIcon, 105, 70);
                botao11 = new TouchButtonIconWithText("BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_button_label_select_payments"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _filePayedDocumentsIcon, sizeIcon, 105, 70);
                botao12 = new TouchButtonIconWithText("STOCK_MERCHANDISE_ENTRY_ACCESS", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documentticket_type_title_cs_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileInsertMerchIcon, sizeIcon, 105, 70);

                //Buttons Relatórios
                botao13 = new TouchButtonIconWithText("REPORT_ACCESS", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_reports"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsMenuIcon, sizeIcon, 105, 70);
                botao14 = new TouchButtonIconWithText("REPORT_COMPANY_BILLING", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_company_billing_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsTotalIcon, sizeIcon, 105, 70);
                botao15 = new TouchButtonIconWithText("REPORT_CUSTOMER_BALANCE_DETAILS", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_customer_balance_details_short"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsClientsIcon, sizeIcon, 105, 70);
                botao16 = new TouchButtonIconWithText("REPORT_SALES_DETAIL_PER_DATE", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_sales_per_date"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileReportsDayIcon, sizeIcon, 105, 70);

                PosReportsDialog reportsClicked = new PosReportsDialog();

                //Permissões dos botões
                botao1.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
                botao2.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
                botao3.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
                botao4.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW");

                botao5.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_ARTICLE_VIEW");
                botao6.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_CUSTOMER_VIEW");
                botao7.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_USERDETAIL_VIEW");
                botao8.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");

                botao9.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
                botao10.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
                botao11.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
                botao12.Sensitive = GeneralSettings.HasPermissionTo("STOCK_MERCHANDISE_ENTRY_ACCESS");

                //Este fica comentado, porque o próprio menu dos reports tem controlo de previlégios
                //botao13.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_ACCESS");
                botao14.Sensitive = GeneralSettings.HasPermissionTo("REPORT_COMPANY_BILLING");
                botao15.Sensitive = GeneralSettings.HasPermissionTo("REPORT_CUSTOMER_BALANCE_DETAILS");
                botao16.Sensitive = GeneralSettings.HasPermissionTo("REPORT_SALES_DETAIL_PER_DATE");


                //Actions Configurações
                botao1.Clicked += delegate { botao1.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTerminal>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao1, null); };
                botao2.Clicked += delegate { botao2.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao2, null); };
                botao3.Clicked += delegate { botao3.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYears>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao3, null); };
                botao4.Clicked += delegate { botao4.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrinters>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao4, null); };

                //Actions Tabelas
                botao5.Clicked += delegate { botao5.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticle>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao5, null); };
                botao6.Clicked += delegate { botao6.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewCustomer>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao6, null); };
                botao7.Clicked += delegate { botao7.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewUser>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao7, null); };
                botao8.Clicked += delegate { botao8.Content = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTable>(pSourceWindow); GlobalApp.BackOfficeMainWindow._dashboardButton_Clicked(botao8, null); };

                //Actions Documents
                botao9.Clicked += delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(pSourceWindow, 0); };
                botao10.Clicked += delegate { logicpos.Utils.StartNewDocumentFromBackOffice(pSourceWindow); };
                botao11.Clicked += delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(pSourceWindow, 3); };
                botao12.Clicked += delegate { logicpos.Utils.OpenArticleStockDialog(_sourceWindow); };

                //Actions Reports
                botao13.Clicked += delegate { logicpos.Utils.StartReportsMenuFromBackOffice(pSourceWindow); };
                botao14.Clicked += delegate
                {
                    reportsClicked.PrintReportRouter
                    (botao14, null);
                };
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
                    currency = XPOSettings.Session.ExecuteScalar(sqlCurrency).ToString();
                }
                catch
                {
                    currency = CultureSettings.SaftCurrencyCode;
                }

                decimal dailyTotal = 0;
                decimal MonthlyTotal = 0;
                decimal annualTotal = 0;
                ArrayList values = new ArrayList
                {
                    DateTime.Now.Year.ToString()
                };
                try
                {
                    SortingCollection sortCollection = new SortingCollection
                    {
                        new SortProperty("Date", SortingDirection.Ascending)
                    };
                    CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL AND (DocumentType.Oid = '{0}' OR DocumentType.Oid = '{1}' OR DocumentType.Oid = '{2}' OR DocumentType.Oid = '{3}') AND DocumentStatusReason != 'A')", invoiceOid, invoiceAndPaymentOid, simpleInvoiceOid, creditNoteOid));
                    collectionDocuments = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(fin_documentfinancemaster)), criteria, sortCollection, int.MaxValue, false, true);

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
                    _logger.Error(ex.Message, ex);
                }

                label = new Label();
                frame.ShadowType = (ShadowType)0;

                label.Text = string.Format("{0} {3}\n\n{1} {3}\n\n{2} {3}",
                    Convert.ToInt64(Math.Round(dailyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(MonthlyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(annualTotal, 0)).ToString(),
                    currency.ToString());

                label.ModifyFont(FontDescription.FromString("Trebuchet MS 16"));
                label.ModifyFg(StateType.Normal, white.ToGdkColor());
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
                selAno.ModifyFg(StateType.Selected, black.ToGdkColor());

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
                   label.ModifyFg(StateType.Normal, white.ToGdkColor());
                   label.Justify = Justification.Right;
                   frame.Add(label);

                   hbox.PackStart(frame, false, false, 0);
                   vbox.PackStart(hbox, false, false, 0);
                   string selectedDate = string.Format("01/01/{0}", (selAno.ActiveText.ToString()));
                   fix.Put(vbox, 640, 515);
                   fix.Put(drawSalesGraphic(DateTime.Parse(selectedDate), true), 55, 485);

               };

                fix.Put(selAno, 220, 665);

                //GRÁFICO         
                fix.Put(drawSalesGraphic(datenow, false), 55, 485);

                //Adiciona tudo ao evento principal
                _eventboxDashboard.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                _eventboxDashboard.Add(fix);
                fix.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                logicpos.Utils.ShowMessageTouchErrorRenderTheme(pSourceWindow, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
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
            PlotColor plotColors = PlotColor.Green;
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
            string date;
            DateTime parsedDate = new DateTime();
            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    decimal totalMes = 0;
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
                _logger.Error(ex.Message, ex);
                store = null;
                return store;
            }
        }
    }
}
