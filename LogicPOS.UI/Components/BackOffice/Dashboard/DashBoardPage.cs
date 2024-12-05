using Gtk;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using Medsphere.Widgets;
using Pango;
using System;
using System.Collections;
using System.Drawing;
using Alignment = Gtk.Alignment;
using Color = System.Drawing.Color;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage : Box
    {
        private readonly Window _parentWindow;
        private IconButtonWithText BtnTerminals { get; set; }
        private readonly IconButtonWithText BtnPreferenceParameters;
        private readonly IconButtonWithText BtnFiscalYears;
        private readonly IconButtonWithText BtnPrinters;
        private readonly IconButtonWithText BtnArticles;
        private readonly IconButtonWithText BtnCustomers;
        private readonly IconButtonWithText BtnUsers;
        private readonly IconButtonWithText BtnTables;
        private readonly IconButtonWithText BtnDocuments;
        private readonly IconButtonWithText BtnNewDocument;
        private readonly IconButtonWithText botao11;
        private readonly IconButtonWithText BtnArticleStock;
        private readonly IconButtonWithText BtnReportsMenu;
        private readonly IconButtonWithText BtnPrintReportRouter;
        private readonly IconButtonWithText botao15;
        private readonly IconButtonWithText botao16;
        private readonly ICollection collectionDocuments = null;
        private readonly string creditNoteOid = "Tchialo";
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
            var themeWindow = LogicPOSAppContext.Theme.Theme.Frontoffice.Window.Find(predicate);
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
            _eventBox.WidthRequest = BackOfficeWindow.ScreenSize.Width;
            _eventBox.HeightRequest = BackOfficeWindow.ScreenSize.Height;
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
                BtnTerminals = new IconButtonWithText(
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

                BtnPreferenceParameters = new IconButtonWithText(
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


                BtnFiscalYears = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_documentfinance_years_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertFiscalYear, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnPrinters = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_printers"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Tabelas
                BtnArticles = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_ARTICLE_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_articles"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileArticlesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnCustomers = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CUSTOMER_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_customers"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileCostumersIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnUsers = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_USERDETAIL_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_users"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileEmployeesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnTables = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_other_tables"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileOtherTablesIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Documentos
                BtnDocuments = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSSHOW_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_worksession_resume_finance_documents"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileNewDocumentIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnNewDocument = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSNEW_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("window_title_dialog_new_document"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileDocumentsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao11 = new IconButtonWithText(new ButtonSettings { Name = "BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_payments"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _filePayedDocumentsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnArticleStock = new IconButtonWithText(new ButtonSettings { Name = "STOCK_MERCHANDISE_ENTRY_ACCESS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_documentticket_type_title_cs_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileInsertMerchIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                //Buttons Relatórios
                BtnReportsMenu = new IconButtonWithText(new ButtonSettings { Name = "REPORT_ACCESS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("global_reports"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsMenuIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                BtnPrintReportRouter = new IconButtonWithText(new ButtonSettings { Name = "REPORT_COMPANY_BILLING", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_company_billing_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsTotalIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao15 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_CUSTOMER_BALANCE_DETAILS", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_customer_balance_details_short"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsClientsIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });
                botao16 = new IconButtonWithText(new ButtonSettings { Name = "REPORT_SALES_DETAIL_PER_DATE", BackgroundColor = _colorBaseDialogDefaultButtonBackground, Text = GeneralUtils.GetResourceByName("report_sales_per_date"), Font = _fontBaseDialogButton, FontColor = _colorBaseDialogDefaultButtonFont, Icon = _fileReportsDayIcon, IconSize = sizeIcon, ButtonSize = new Size(105, 70) });

                BtnTerminals.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU");
                BtnPreferenceParameters.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETER_VIEW");
                BtnFiscalYears.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_CREATE");
                BtnPrinters.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPRINTERS_VIEW");

                BtnArticles.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLE_VIEW");
                BtnCustomers.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CUSTOMER_VIEW");
                BtnUsers.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERDETAIL_VIEW");
                BtnTables.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_VIEW");

                BtnDocuments.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU");
                BtnNewDocument.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCETYPE_CREATE");
                botao11.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_VIEW");
                BtnArticleStock.Sensitive = AuthenticationService.UserHasPermission("STOCK_MERCHANDISE_ENTRY_ACCESS");

                BtnReportsMenu.Sensitive = AuthenticationService.UserHasPermission("REPORT_ACCESS");
                BtnPrintReportRouter.Sensitive = AuthenticationService.UserHasPermission("REPORT_COMPANY_BILLING");
                botao15.Sensitive = AuthenticationService.UserHasPermission("REPORT_CUSTOMER_BALANCE_DETAILS");
                botao16.Sensitive = AuthenticationService.UserHasPermission("REPORT_SALES_DETAIL_PER_DATE");

                BtnTerminals.Clicked += delegate { BtnTerminals.Page = TerminalsPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnTerminals, null); };
                BtnPreferenceParameters.Clicked += delegate { BtnPreferenceParameters.Page = PreferenceParametersPage.CompanyPageInstance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnPreferenceParameters, null); };
                BtnFiscalYears.Clicked += delegate { BtnFiscalYears.Page = FiscalYearsPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnFiscalYears, null); };
                BtnPrinters.Clicked += delegate { BtnPrinters.Page = PrintersPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnPrinters, null); };

                BtnArticles.Clicked += delegate { BtnArticles.Page = ArticlesPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnArticles, null); };
                BtnCustomers.Clicked += delegate { BtnCustomers.Page = CustomersPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnCustomers, null); };
                BtnUsers.Clicked += delegate { BtnUsers.Page = UsersPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnUsers, null); };
                BtnTables.Clicked += delegate { BtnTables.Page = TablesPage.Instance; BackOfficeWindow.Instance.MenuBtn_Clicked(BtnTables, null); };

                BtnDocuments.Clicked += BtnDocuments_Clicked;
                BtnNewDocument.Clicked += BtnNewDocument_Clicked;
                botao11.Clicked += delegate { };
                BtnArticleStock.Clicked += delegate { logicpos.Utils.OpenArticleStockDialog(_parentWindow); };

                BtnReportsMenu.Clicked += delegate { logicpos.Utils.StartReportsMenuFromBackOffice(parentWindow); };
                BtnPrintReportRouter.Clicked += delegate
                {
                    //tchial0
                    //reportsClicked.PrintReportRouter
                    //(BtnPrintReportRouter, null);
                };

                botao15.Clicked += delegate { /*reportsClicked.PrintReportRouter(botao15, null);*/ };
                botao16.Clicked += delegate {/* reportsClicked.PrintReportRouter(botao16, null);*/ };

                fix.Put(BtnTerminals, 55, 62);
                fix.Put(BtnPreferenceParameters, 55, 155);
                fix.Put(BtnFiscalYears, 55, 250);
                fix.Put(BtnPrinters, 55, 345);

                fix.Put(BtnArticles, 245, 62);
                fix.Put(BtnCustomers, 245, 155);
                fix.Put(BtnUsers, 245, 250);
                fix.Put(BtnTables, 245, 345);

                fix.Put(BtnDocuments, 440, 62);
                fix.Put(BtnNewDocument, 440, 155);
                fix.Put(botao11, 440, 250);
                fix.Put(BtnArticleStock, 440, 345);

                fix.Put(BtnReportsMenu, 635, 62);
                fix.Put(BtnPrintReportRouter, 635, 155);
                fix.Put(botao15, 635, 250);
                fix.Put(botao16, 635, 345);

                string currency = PreferenceParametersService.SystemCurrency;

                decimal dailyTotal = 0;
                decimal MonthlyTotal = 0;
                decimal annualTotal = 0;
                ArrayList values = new ArrayList
                {
                    DateTime.Now.Year.ToString()
                };

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
                   //fix.Put(drawSalesGraphic(DateTime.Parse(selectedDate), true), 55, 485);

               };

                fix.Put(selAno, 220, 665);

                //GRÁFICO         
                //fix.Put(drawSalesGraphic(datenow, false), 55, 485);

                //Adiciona tudo ao evento principal
                _eventBox.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                _eventBox.Add(fix);
                fix.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowThemeRenderingErrorAlert($"{errorMessage}\n\n{ex.Message}", parentWindow);
            }

            ShowAll();
        }



    }
}
