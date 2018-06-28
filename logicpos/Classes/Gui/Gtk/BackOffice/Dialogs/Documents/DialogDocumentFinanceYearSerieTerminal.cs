using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogDocumentFinanceYearSerieTerminal : BOBaseDialog
    {
        public DialogDocumentFinanceYearSerieTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_documentfinanceseries);
            SetSizeRequest(500, 550);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //FiscalYear
                XPOComboBox xpoComboBoxFiscalYear = new XPOComboBox(DataSourceRow.Session, typeof(FIN_DocumentFinanceYears), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).FiscalYear, "Designation");
                BOWidgetBox boxFiscalYear = new BOWidgetBox(Resx.global_fiscal_year, xpoComboBoxFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalYear, DataSourceRow, "FiscalYear", SettingsApp.RegexGuid, true));

                //DocumentType
                XPOComboBox xpoComboBoxDocumentType = new XPOComboBox(DataSourceRow.Session, typeof(FIN_DocumentFinanceType), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).DocumentType, "Designation");
                BOWidgetBox boxDocumentType = new BOWidgetBox(Resx.global_documentfinance_type, xpoComboBoxDocumentType);
                vboxTab1.PackStart(boxDocumentType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentType, DataSourceRow, "DocumentType", SettingsApp.RegexGuid, true));

                //Serie
                XPOComboBox xpoComboBoxSerie = new XPOComboBox(DataSourceRow.Session, typeof(FIN_DocumentFinanceSeries), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).Serie, "Designation");
                BOWidgetBox boxSerie = new BOWidgetBox(Resx.global_documentfinance_series, xpoComboBoxSerie);
                vboxTab1.PackStart(boxSerie, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxSerie, DataSourceRow, "Serie", SettingsApp.RegexGuid, true));

                //Terminal
                XPOComboBox xpoComboBoxTerminal = new XPOComboBox(DataSourceRow.Session, typeof(POS_ConfigurationPlaceTerminal), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).Terminal, "Designation");
                BOWidgetBox boxTerminal = new BOWidgetBox(Resx.global_configurationplaceterminal, xpoComboBoxTerminal);
                vboxTab1.PackStart(boxTerminal, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTerminal, DataSourceRow, "Terminal", SettingsApp.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(Resx.global_designation, entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrinters), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).Printer, "Designation");
                BOWidgetBox boxPrinter = new BOWidgetBox(Resx.global_configurationprinter, xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", SettingsApp.RegexGuid, true));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(SYS_ConfigurationPrintersTemplates), (DataSourceRow as FIN_DocumentFinanceYearSerieTerminal).Template, "Designation", "FinancialTemplate = 1");
                BOWidgetBox boxTemplate = new BOWidgetBox(Resx.global_configurationprintersTemplate, xpoComboBoxTemplate);
                vboxTab1.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", SettingsApp.RegexGuid, true));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components 
                xpoComboBoxFiscalYear.Sensitive = false;
                xpoComboBoxDocumentType.Sensitive = false;
                xpoComboBoxSerie.Sensitive = false;
                xpoComboBoxTerminal.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}