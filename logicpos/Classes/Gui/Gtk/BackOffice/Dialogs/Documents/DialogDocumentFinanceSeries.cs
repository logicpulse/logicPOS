using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Globalization;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogDocumentFinanceSeries : BOBaseDialog
    {

        public DialogDocumentFinanceSeries(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_documentfinanceseries"));
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
                BOWidgetBox boxFiscalYear = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_series"), xpoComboBoxFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalYear, DataSourceRow, "FiscalYear", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //DocumentType
                XPOComboBox xpoComboBoxDocumentType = new XPOComboBox(DataSourceRow.Session, typeof(fin_documentfinancetype), (DataSourceRow as fin_documentfinanceseries).DocumentType, "Designation", null);
                BOWidgetBox boxDocumentType = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_type"), xpoComboBoxDocumentType);
                vboxTab1.PackStart(boxDocumentType, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentType, DataSourceRow, "DocumentType", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAcronym, _dataSourceRow, "Acronym", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //NextDocumentNumber
                Entry entryNextDocumentNumber = new Entry();
                BOWidgetBox boxNextDocumentNumber = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinanceseries_NextDocumentNumber"), entryNextDocumentNumber);
                vboxTab1.PackStart(boxNextDocumentNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxNextDocumentNumber, _dataSourceRow, "NextDocumentNumber", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //DocumentNumberRangeBegin
                Entry entryDocumentNumberRangeBegin = new Entry();
                BOWidgetBox boxDocumentNumberRangeBegin = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinanceseries_DocumentNumberRangeBegin"), entryDocumentNumberRangeBegin);
                vboxTab1.PackStart(boxDocumentNumberRangeBegin, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentNumberRangeBegin, _dataSourceRow, "DocumentNumberRangeBegin", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //DocumentNumberRangeEnd
                Entry entryDocumentNumberRangeEnd = new Entry();
                BOWidgetBox boxDocumentNumberRangeEnd = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinanceseries_DocumentNumberRangeEnd"), entryDocumentNumberRangeEnd);
                vboxTab1.PackStart(boxDocumentNumberRangeEnd, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDocumentNumberRangeEnd, _dataSourceRow, "DocumentNumberRangeEnd", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

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
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
