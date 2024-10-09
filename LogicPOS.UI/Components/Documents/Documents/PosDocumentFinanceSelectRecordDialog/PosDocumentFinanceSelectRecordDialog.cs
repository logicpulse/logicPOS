using Gtk;
using System;
using System.Drawing;
using LogicPOS.Settings;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceSelectRecordDialog : BaseDialog
    {
        private  IconButtonWithText BtnDocuments { get; set; }
        private IconButtonWithText BtnReceiptsEmission { get; set; }
        private IconButtonWithText BtnReceipts { get; set; }
        private IconButtonWithText BtnCurrentAccount { get; set; }
        private IconButtonWithText BtnWorkSessionPeriods { get; set; }
        private IconButtonWithText BtnStockEntry { get; set; }

        public PosDocumentFinanceSelectRecordDialog(Window parentWindow, DialogFlags pDialogFlags, int docChoice)
            : base(parentWindow, pDialogFlags)
        {
            //Parameters
            WindowSettings.Source = parentWindow;            

            //Settings
            string _fileIconListFinanceDocuments = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png";
            string _fileIconListCurrentAccountDocuments = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_reports.png";
            string _fileIconListWorksessionPeriods = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_cashdrawer.png";
            string _fileIconListMerchandiseEntry = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_merchandise_entry.png";

            //Sizes
            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 10;
            int windowSizeWidth = (buttonWidth + Convert.ToInt16(tablePadding)) * 3 + 65;
            int windowSizeHeight = (buttonHeight + Convert.ToInt16(tablePadding)) * 2 + 90;

            //Init Local Vars
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_document_finance");
            Size windowSize = new Size(windowSizeWidth, windowSizeHeight);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_documents.png";

            //Buttons
             BtnDocuments = new IconButtonWithText(new ButtonSettings { Name = "touchButtonPosToolbarFinanceDocuments_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_record_finance_documents"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListFinanceDocuments, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) }) { Token = "ALL" };
            BtnReceiptsEmission = new IconButtonWithText(new ButtonSettings { Name = "touchButtonPosToolbarFinanceDocumentsInvoicesForPayment_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_ft_unpaid"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListFinanceDocuments, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) }) { Token = "FT_UNPAYED" };
            BtnReceipts = new IconButtonWithText(new ButtonSettings { Name = "touchButtonPosToolbarFinanceDocumentsPayments_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_payments"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListFinanceDocuments, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) });
            BtnCurrentAccount = new IconButtonWithText(new ButtonSettings { Name = "REPORT_CUSTOMER_BALANCE_DETAILS", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_finance_documents_cc"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListCurrentAccountDocuments, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) }) { Token = "CC" };
            BtnWorkSessionPeriods = new IconButtonWithText(new ButtonSettings { Name = "touchButtonPosToolbarWorkSessionPeriods_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_worksession_period"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListWorksessionPeriods, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) });
            BtnStockEntry = new IconButtonWithText(new ButtonSettings { Name = "touchButtonPosToolbarMerchandiseEntry_Green", BackgroundColor = ColorSettings.DefaultButtonBackground, Text = GeneralUtils.GetResourceByName("dialog_button_label_select_merchandise_entry"), Font = FontSettings.Button, FontColor = ColorSettings.DefaultButtonFont, Icon = _fileIconListMerchandiseEntry, IconSize = sizeIcon, ButtonSize = new Size(buttonWidth, buttonHeight) });
            //Permission
            BtnStockEntry.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("STOCK_MERCHANDISE_ENTRY_ACCESS");

            //Table
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(BtnDocuments, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnReceiptsEmission, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnReceipts, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 2
            table.Attach(BtnCurrentAccount, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnWorkSessionPeriods, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(BtnStockEntry, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            PosReportsDialog reportsClicked = new PosReportsDialog();

            //TK016235 BackOffice - Mode
            //numero da escolha vem do accordion do BackOfficeMainWindow, e passa por Utils
            switch (docChoice)
            {
                case 1:
                    BtnDocuments_Clicked(BtnDocuments, null);
                    break;
                case 2:
                    BtnDocuments_Clicked(BtnReceiptsEmission, null);
                    break;
                case 3:
                    _toolbarFinanceDocumentsPayments_Clicked(BtnReceipts, null);
                    break;
                case 4:
                    BtnCurrentAccount.Clicked += delegate { reportsClicked.PrintReportRouter(BtnCurrentAccount, null); };                    
                    break;
                case 5:
                    _touchButtonPosToolbarWorkSessionPeriods_Clicked(BtnWorkSessionPeriods, null);
                    break;
                case 6:
                    _touchButtonPosToolbarMerchandiseEntry_Clicked(BtnStockEntry, null);
                    break;
                case 0:

                    //Init Object
                    this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

                    //Shared Events 
                    BtnDocuments.Clicked += BtnDocuments_Clicked;
                    BtnReceiptsEmission.Clicked += BtnDocuments_Clicked;
                    BtnCurrentAccount.Clicked += delegate { reportsClicked.PrintReportRouter(BtnCurrentAccount, null); };
                    //Non Shared Events
                    BtnReceipts.Clicked += _toolbarFinanceDocumentsPayments_Clicked;
                    BtnWorkSessionPeriods.Clicked += _touchButtonPosToolbarWorkSessionPeriods_Clicked;
                    BtnStockEntry.Clicked += _touchButtonPosToolbarMerchandiseEntry_Clicked;

                    //Reference Objects
                    _printerGeneric = (sys_configurationprinters)XPOSettings.Session.GetObjectByKey(typeof(sys_configurationprinters), PrintingSettings.GenericPrinterId);
                    break;
            }
        }
    }
}
