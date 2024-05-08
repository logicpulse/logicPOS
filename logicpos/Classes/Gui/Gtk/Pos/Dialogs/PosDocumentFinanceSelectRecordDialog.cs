using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.shared.App;
using System;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceSelectRecordDialog : PosBaseDialog
    {
        private readonly TouchButtonIconWithText _touchButtonPosToolbarFinanceDocuments;
        private readonly TouchButtonIconWithText _toolbarFinanceDocumentsInvoicesUnpayed;
        private readonly TouchButtonIconWithText _toolbarFinanceDocumentsPayments;
        private readonly TouchButtonIconWithText _touchButtonPosToolbarCurrentAccountDocuments;
        private readonly TouchButtonIconWithText _touchButtonPosToolbarWorkSessionPeriods;
        private readonly TouchButtonIconWithText _touchButtonPosToolbarMerchandiseEntry;

        public PosDocumentFinanceSelectRecordDialog(Window pSourceWindow, DialogFlags pDialogFlags, int docChoice)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;            

            //Settings
            string _fileIconListFinanceDocuments = GeneralSettings.Path["images"] + @"Icons\icon_pos_toolbar_finance_document.png";
            string _fileIconListCurrentAccountDocuments = GeneralSettings.Path["images"] + @"Icons\icon_pos_toolbar_reports.png";
            string _fileIconListWorksessionPeriods = GeneralSettings.Path["images"] + @"Icons\icon_pos_toolbar_cashdrawer.png";
            string _fileIconListMerchandiseEntry = GeneralSettings.Path["images"] + @"Icons\icon_pos_toolbar_merchandise_entry.png";

            //Sizes
            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 10;
            int windowSizeWidth = (buttonWidth + Convert.ToInt16(tablePadding)) * 3 + 65;
            int windowSizeHeight = (buttonHeight + Convert.ToInt16(tablePadding)) * 2 + 90;

            //Init Local Vars
            string windowTitle = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_dialog_document_finance");
            Size windowSize = new Size(windowSizeWidth, windowSizeHeight);
            string fileDefaultWindowIcon = GeneralSettings.Path["images"] + @"Icons\Windows\icon_window_documents.png";

            //Buttons
            _touchButtonPosToolbarFinanceDocuments = new TouchButtonIconWithText("touchButtonPosToolbarFinanceDocuments_Green", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_record_finance_documents"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListFinanceDocuments, sizeIcon, buttonWidth, buttonHeight) { Token = "ALL" };
            _toolbarFinanceDocumentsInvoicesUnpayed = new TouchButtonIconWithText("touchButtonPosToolbarFinanceDocumentsInvoicesForPayment_Green", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_finance_documents_ft_unpaid"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListFinanceDocuments, sizeIcon, buttonWidth, buttonHeight) { Token = "FT_UNPAYED" };
            _toolbarFinanceDocumentsPayments = new TouchButtonIconWithText("touchButtonPosToolbarFinanceDocumentsPayments_Green", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_payments"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListFinanceDocuments, sizeIcon, buttonWidth, buttonHeight);
            _touchButtonPosToolbarCurrentAccountDocuments = new TouchButtonIconWithText("REPORT_CUSTOMER_BALANCE_DETAILS", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_finance_documents_cc"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListCurrentAccountDocuments, sizeIcon, buttonWidth, buttonHeight) { Token = "CC" };
            _touchButtonPosToolbarWorkSessionPeriods = new TouchButtonIconWithText("touchButtonPosToolbarWorkSessionPeriods_Green", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_worksession_period"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListWorksessionPeriods, sizeIcon, buttonWidth, buttonHeight);
            _touchButtonPosToolbarMerchandiseEntry = new TouchButtonIconWithText("touchButtonPosToolbarMerchandiseEntry_Green", _colorBaseDialogDefaultButtonBackground, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_button_label_select_merchandise_entry"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, _fileIconListMerchandiseEntry, sizeIcon, buttonWidth, buttonHeight);
            //Permission
            _touchButtonPosToolbarMerchandiseEntry.Sensitive = SharedUtils.HasPermissionTo("STOCK_MERCHANDISE_ENTRY_ACCESS");

            //Table
            Table table = new Table(1, 1, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(_touchButtonPosToolbarFinanceDocuments, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_toolbarFinanceDocumentsInvoicesUnpayed, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_toolbarFinanceDocumentsPayments, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //Row 2
            table.Attach(_touchButtonPosToolbarCurrentAccountDocuments, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_touchButtonPosToolbarWorkSessionPeriods, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(_touchButtonPosToolbarMerchandiseEntry, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            PosReportsDialog reportsClicked = new PosReportsDialog();

            //TK016235 BackOffice - Mode
            //numero da escolha vem do accordion do BackOfficeMainWindow, e passa por Utils
            switch (docChoice)
            {
                case 1:
                    touchButtonPosToolbarFinanceDocuments_Clicked(_touchButtonPosToolbarFinanceDocuments, null);
                    break;
                case 2:
                    touchButtonPosToolbarFinanceDocuments_Clicked(_toolbarFinanceDocumentsInvoicesUnpayed, null);
                    break;
                case 3:
                    _toolbarFinanceDocumentsPayments_Clicked(_toolbarFinanceDocumentsPayments, null);
                    break;
                case 4:
                    _touchButtonPosToolbarCurrentAccountDocuments.Clicked += delegate { reportsClicked.PrintReportRouter(_touchButtonPosToolbarCurrentAccountDocuments, null); };                    
                    break;
                case 5:
                    _touchButtonPosToolbarWorkSessionPeriods_Clicked(_touchButtonPosToolbarWorkSessionPeriods, null);
                    break;
                case 6:
                    _touchButtonPosToolbarMerchandiseEntry_Clicked(_touchButtonPosToolbarMerchandiseEntry, null);
                    break;
                case 0:

                    //Init Object
                    this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

                    //Shared Events 
                    _touchButtonPosToolbarFinanceDocuments.Clicked += touchButtonPosToolbarFinanceDocuments_Clicked;
                    _toolbarFinanceDocumentsInvoicesUnpayed.Clicked += touchButtonPosToolbarFinanceDocuments_Clicked;
                    _touchButtonPosToolbarCurrentAccountDocuments.Clicked += delegate { reportsClicked.PrintReportRouter(_touchButtonPosToolbarCurrentAccountDocuments, null); };
                    //Non Shared Events
                    _toolbarFinanceDocumentsPayments.Clicked += _toolbarFinanceDocumentsPayments_Clicked;
                    _touchButtonPosToolbarWorkSessionPeriods.Clicked += _touchButtonPosToolbarWorkSessionPeriods_Clicked;
                    _touchButtonPosToolbarMerchandiseEntry.Clicked += _touchButtonPosToolbarMerchandiseEntry_Clicked;

                    //Reference Objects
                    _printerGeneric = (sys_configurationprinters)XPOSettings.Session.GetObjectByKey(typeof(sys_configurationprinters), PrintingSettings.XpoOidConfigurationPrinterGeneric);
                    break;
            }
        }
    }
}
