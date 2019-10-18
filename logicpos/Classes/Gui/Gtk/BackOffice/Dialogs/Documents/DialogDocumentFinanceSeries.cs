using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogDocumentFinanceSeries : BOBaseDialog
    {

        public DialogDocumentFinanceSeries(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_documentfinanceseries"));
            SetSizeRequest(500, 466);
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
                XPOComboBox xpoComboBoxFiscalYear = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinanceyears), (DataSourceRow as fin_documentfinanceseries).FiscalYear, "Designation", null);
                BOWidgetBox boxFiscalYear = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_series"), xpoComboBoxFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalYear, DataSourceRow, "FiscalYear", SettingsApp.RegexGuid, true));

                //DocumentType
                XPOComboBox xpoComboBoxDocumentType = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinancetype), (DataSourceRow as fin_documentfinanceseries).DocumentType, "Designation", null);
                BOWidgetBox boxDocumentType = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type"), xpoComboBoxDocumentType);
                vboxTab1.PackStart(boxDocumentType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentType, DataSourceRow, "DocumentType", SettingsApp.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", SettingsApp.RegexAlfaNumericExtended, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAcronym, _dataSourceRow, "Acronym", SettingsApp.RegexAlfaNumeric, false));

                //NextDocumentNumber
                Entry entryNextDocumentNumber = new Entry();
                BOWidgetBox boxNextDocumentNumber = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_NextDocumentNumber"), entryNextDocumentNumber);
                vboxTab1.PackStart(boxNextDocumentNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxNextDocumentNumber, _dataSourceRow, "NextDocumentNumber", SettingsApp.RegexIntegerGreaterThanZero, false));

                //DocumentNumberRangeBegin
                Entry entryDocumentNumberRangeBegin = new Entry();
                BOWidgetBox boxDocumentNumberRangeBegin = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_DocumentNumberRangeBegin"), entryDocumentNumberRangeBegin);
                vboxTab1.PackStart(boxDocumentNumberRangeBegin, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentNumberRangeBegin, _dataSourceRow, "DocumentNumberRangeBegin", SettingsApp.RegexIntegerGreaterThanZero, false));

                //DocumentNumberRangeEnd
                Entry entryDocumentNumberRangeEnd = new Entry();
                BOWidgetBox boxDocumentNumberRangeEnd = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_DocumentNumberRangeEnd"), entryDocumentNumberRangeEnd);
                vboxTab1.PackStart(boxDocumentNumberRangeEnd, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentNumberRangeEnd, _dataSourceRow, "DocumentNumberRangeEnd", SettingsApp.RegexIntegerGreaterThanZero, false));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components 
                xpoComboBoxFiscalYear.Sensitive = false;
                xpoComboBoxDocumentType.Sensitive = false;
                entryAcronym.Sensitive = false;
                entryNextDocumentNumber.Sensitive = false;
                entryDocumentNumberRangeBegin.Sensitive = false;
                entryDocumentNumberRangeEnd.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
