using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using Medsphere.Widgets;
using Pango;
using System;
using System.Collections;
using System.Drawing;
using Alignment = Gtk.Alignment;
using Color = System.Drawing.Color;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class DashBoardPage : Box
    {
        private readonly Window _parentWindow;
        private readonly IconButtonWithText botao1;
        private readonly IconButtonWithText botao2;
        private readonly IconButtonWithText botao3;
        private readonly IconButtonWithText botao4;
        private readonly IconButtonWithText botao5;
        private readonly IconButtonWithText botao6;
        private readonly IconButtonWithText botao7;
        private readonly IconButtonWithText botao8;
        private readonly IconButtonWithText botao9;
        private readonly IconButtonWithText botao10;
        private readonly IconButtonWithText botao11;
        private readonly IconButtonWithText botao12;
        private readonly IconButtonWithText botao13;
        private readonly IconButtonWithText botao14;
        private readonly IconButtonWithText botao15;
        private readonly IconButtonWithText botao16;
        private readonly ICollection collectionDocuments = null;
        private readonly string creditNoteOid = CustomDocumentSettings.CreditNoteId.ToString();
        private readonly string invoiceOid = InvoiceSettings.InvoiceId.ToString();
        private readonly string simpleInvoiceOid = DocumentSettings.SimplifiedInvoiceId.ToString();
        private readonly string invoiceAndPaymentOid = DocumentSettings.InvoiceAndPaymentId.ToString();


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


        //ScreenArea
        protected EventBox _eventBox;
        protected Color _colorBaseDialogDefaultButtonFont = ("76, 72, 70").StringToColor();
        protected Color _colorBaseDialogDefaultButtonBackground = ("156, 191, 42").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonFont = ("0, 0, 0").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
        //protected String _fontBaseDialogButton = SharedUtils.OSSlash(LogicPOS.Settings.AppSettings.Instance.fontBaseDialogButton"]);
        protected string _fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
        protected string _fileActionDefault = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_default.png";
        protected string _fileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        protected string _fileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";

        //Colors
        private readonly Color colorBackOfficeContentBackground = AppSettings.Instance.colorBackOfficeContentBackground;
        private readonly Color colorBackOfficeStatusBarBackground = AppSettings.Instance.colorBackOfficeStatusBarBackground;
        private readonly Color colorBackOfficeAccordionFixBackground = AppSettings.Instance.colorBackOfficeAccordionFixBackground;
        private readonly Color colorBackOfficeStatusBarFont = AppSettings.Instance.colorBackOfficeStatusBarFont;
        private readonly Color colorBackOfficeStatusBarBottomBackground = AppSettings.Instance.colorBackOfficeStatusBarBottomBackground;
        public Color slateBlue = Color.FromName("White");
        //private Frame frame;
        private readonly Label label;

        public DashBoardPage(Window parentWindow)
        {
            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(AppSettings.Instance.fontGenericTreeViewColumn);
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
            _parentWindow = parentWindow;

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
            _eventBox = new EventBox();
            //_eventboxDashboard.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(screenBackgroundColor));
            _eventBox.WidthRequest = GlobalApp.BackOfficeScreenSize.Width;
            _eventBox.HeightRequest = GlobalApp.BackOfficeScreenSize.Height;
            Alignment _alignmentWindow = new Alignment(0.0f, 0.0f, 0.0f, 0.0f)
            {
                _eventBox
            };
            Add(_alignmentWindow);
            try
            {
                //Imagem carregada aqui para o dashboard
                string fileImageBack = $"{PathsSettings.Paths["themes"]}Default/Backgrounds/Windows/LogicPOS_WorkFlow_{CultureSettings.CurrentCultureName}.png";
                System.Drawing.Image pImage = System.Drawing.Image.FromFile(fileImageBack);
                Gdk.Pixbuf pixbuf = logicpos.Utils.ImageToPixbuf(pImage);
                _eventBox.Style = logicpos.Utils.GetImageBackgroundDashboard(pixbuf);
                //Buttons Configuração
                botao1 = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU",
                        BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                        Text = GeneralUtils.GetResourceByName("dialog_edit_ConfigurationPlaceTerminal_tab1_label"),
                        Font = _fontBaseDialogButton,
                        FontColor = _colorBaseDialogDefaultButtonFont,
                        Icon = _fileTerminalsIcon,
                        IconSize = sizeIcon,
                        ButtonSize = new Size(105, 70)
                    });

                botao2 = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW",
                        BackgroundColor = _colorBaseDialogDefaultButtonBackground,
                        Text = GeneralUtils.GetResourceByName("global_application_setup"),
                        Font = _fontBaseDialogButton,
                        FontColor = _colorBaseDialogDefaultButtonFont,
                        Icon = _fileFiscalYearIcon,
                        IconSize = sizeIcon,
                        ButtonSize = new Size(105, 70)
                    });


                botao3 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_documentfinance_years_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertFiscalYear, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao4 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_printers"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Tabelas
                botao5 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_ARTICLE_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_articles"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileArticlesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao6 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CUSTOMER_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_customers"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileCostumersIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao7 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_USERDETAIL_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_users"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileEmployeesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao8 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_other_tables"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileOtherTablesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Documentos
                botao9 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSSHOW_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileNewDocumentIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao10 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSNEW_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("window_title_dialog_new_document"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileDocumentsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao11 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_payments"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _filePayedDocumentsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao12 = new IconButtonWithText(new ButtonSettings { Name = "STOCK_MERCHANDISE_ENTRY_ACCESS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_documentticket_type_title_cs_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertMerchIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Relatórios
                botao13 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_ACCESS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_reports"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsMenuIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao14 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_COMPANY_BILLING", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_company_billing_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsTotalIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao15 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_CUSTOMER_BALANCE_DETAILS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_customer_balance_details_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsClientsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao16 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_SALES_DETAIL_PER_DATE", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_sales_per_date"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsDayIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                PosReportsDialog reportsClicked = new PosReportsDialog();

                //Permissões dos botões
                botao1.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
                botao2.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
                botao3.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
                botao4.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW");

                botao5.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_ARTICLE_VIEW");
                botao6.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_CUSTOMER_VIEW");
                botao7.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_USERDETAIL_VIEW");
                botao8.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");

                botao9.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
                botao10.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
                botao11.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
                botao12.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("STOCK_MERCHANDISE_ENTRY_ACCESS");

                //Este fica comentado, porque o próprio menu dos reports tem controlo de previlégios
                //botao13.Sensitive = FrameworkUtils.HasPermissionTo("REPORT_ACCESS");
                botao14.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("REPORT_COMPANY_BILLING");
                botao15.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("REPORT_CUSTOMER_BALANCE_DETAILS");
                botao16.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("REPORT_SALES_DETAIL_PER_DATE");


                //Actions Configurações
                botao1.Clicked += delegate { botao1.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTerminal>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao1, null); };
                botao2.Clicked += delegate { botao2.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPreferenceParameter>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao2, null); };
                botao3.Clicked += delegate { botao3.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewDocumentFinanceYears>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao3, null); };
                botao4.Clicked += delegate { botao4.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPrinters>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao4, null); };

                //Actions Tabelas
                botao5.Clicked += delegate { botao5.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewArticle>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao5, null); };
                botao6.Clicked += delegate { botao6.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewCustomer>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao6, null); };
                botao7.Clicked += delegate { botao7.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewUser>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao7, null); };
                botao8.Clicked += delegate { botao8.Page = logicpos.Utils.GetGenericTreeViewXPO<TreeViewConfigurationPlaceTable>(parentWindow); GlobalApp.BackOfficeMainWindow.Button_Click(botao8, null); };

                //Actions Documents
                botao9.Clicked += delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(parentWindow, 0); };
                botao10.Clicked += delegate { logicpos.Utils.StartNewDocumentFromBackOffice(parentWindow); };
                botao11.Clicked += delegate { logicpos.Utils.StartDocumentsMenuFromBackOffice(parentWindow, 3); };
                botao12.Clicked += delegate { logicpos.Utils.OpenArticleStockDialog(_parentWindow); };

                //Actions Reports
                botao13.Clicked += delegate { logicpos.Utils.StartReportsMenuFromBackOffice(parentWindow); };
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
                _eventBox.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                _eventBox.Add(fix);
                fix.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                logicpos.Utils.ShowMessageTouchErrorRenderTheme(parentWindow, string.Format("{1}{0}{0}{2}", Environment.NewLine, errorMessage, ex.Message));
            }

            ShowAll();
        }

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
                store = null;
                return store;
            }
        }
    }
}
