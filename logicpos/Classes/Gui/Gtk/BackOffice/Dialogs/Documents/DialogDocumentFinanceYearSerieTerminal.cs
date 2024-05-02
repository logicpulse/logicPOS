using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogDocumentFinanceYearSerieTerminal : BOBaseDialog
    {
        public DialogDocumentFinanceYearSerieTerminal(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_edit_documentfinanceseries"));
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
                XPOComboBox xpoComboBoxFiscalYear = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinanceyears), (DataSourceRow as fin_documentfinanceyearserieterminal).FiscalYear, "Designation", null);
                BOWidgetBox boxFiscalYear = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_fiscal_year"), xpoComboBoxFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalYear, DataSourceRow, "FiscalYear", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //DocumentType
                XPOComboBox xpoComboBoxDocumentType = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinancetype), (DataSourceRow as fin_documentfinanceyearserieterminal).DocumentType, "Designation", null);
                BOWidgetBox boxDocumentType = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type"), xpoComboBoxDocumentType);
                vboxTab1.PackStart(boxDocumentType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentType, DataSourceRow, "DocumentType", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Serie
                XPOComboBox xpoComboBoxSerie = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinanceseries), (DataSourceRow as fin_documentfinanceyearserieterminal).Serie, "Designation", null);
                BOWidgetBox boxSerie = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_series"), xpoComboBoxSerie);
                vboxTab1.PackStart(boxSerie, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxSerie, DataSourceRow, "Serie", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Terminal
                XPOComboBox xpoComboBoxTerminal = new XPOComboBox(DataSourceRow.Session, typeof(pos_configurationplaceterminal), (DataSourceRow as fin_documentfinanceyearserieterminal).Terminal, "Designation", null);
                BOWidgetBox boxTerminal = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_configurationplaceterminal"), xpoComboBoxTerminal);
                vboxTab1.PackStart(boxTerminal, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTerminal, DataSourceRow, "Terminal", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinters), (DataSourceRow as fin_documentfinanceyearserieterminal).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_configurationprinter"), xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPrinter, DataSourceRow, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(DataSourceRow.Session, typeof(sys_configurationprinterstemplates), (DataSourceRow as fin_documentfinanceyearserieterminal).Template, "Designation", "FinancialTemplate = 1");
                BOWidgetBox boxTemplate = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_configurationprintersTemplate"), xpoComboBoxTemplate);
                vboxTab1.PackStart(boxTemplate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTemplate, DataSourceRow, "Template", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components 
                xpoComboBoxFiscalYear.Sensitive = false;
                xpoComboBoxDocumentType.Sensitive = false;
                xpoComboBoxSerie.Sensitive = false;
                xpoComboBoxTerminal.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}