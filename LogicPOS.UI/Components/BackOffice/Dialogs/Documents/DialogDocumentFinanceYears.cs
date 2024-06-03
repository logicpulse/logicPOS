using Gtk;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogDocumentFinanceYears : BOBaseDialog
    {

        public DialogDocumentFinanceYears(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_documentfinanceseries"));
            SetSizeRequest(500, 288);
            InitUI();
            //InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAcronym, _dataSourceRow, "Acronym", LogicPOS.Utility.RegexUtils.RegexDocumentSeriesYearAcronym, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //FiscalYear
                Entry entryFiscalYear = new Entry();
                BOWidgetBox boxFiscalYear = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_year"), entryFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalYear, _dataSourceRow, "FiscalYear", LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero, true));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components 
                entryFiscalYear.Sensitive = false;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
