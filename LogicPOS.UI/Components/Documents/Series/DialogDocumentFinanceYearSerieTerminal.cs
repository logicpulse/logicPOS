using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogDocumentFinanceYearSerieTerminal : EditDialog
    {
        [System.Obsolete]
        public DialogDocumentFinanceYearSerieTerminal(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_documentfinanceseries"));
            SetSizeRequest(500, 550);
            InitUI();
            InitNotes();
            ShowAll();
        }

        [System.Obsolete]
        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //FiscalYear
                XPOComboBox xpoComboBoxFiscalYear = new XPOComboBox(Entity.Session, typeof(fin_documentfinanceyears), (Entity as fin_documentfinanceyearserieterminal).FiscalYear, "Designation", null);
                BOWidgetBox boxFiscalYear = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_year"), xpoComboBoxFiscalYear);
                vboxTab1.PackStart(boxFiscalYear, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxFiscalYear, Entity, "FiscalYear", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //DocumentType
                XPOComboBox xpoComboBoxDocumentType = new XPOComboBox(Entity.Session, typeof(fin_documentfinancetype), (Entity as fin_documentfinanceyearserieterminal).DocumentType, "Designation", null);
                BOWidgetBox boxDocumentType = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_type"), xpoComboBoxDocumentType);
                vboxTab1.PackStart(boxDocumentType, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDocumentType, Entity, "DocumentType", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Serie
                XPOComboBox xpoComboBoxSerie = new XPOComboBox(Entity.Session, typeof(fin_documentfinanceseries), (Entity as fin_documentfinanceyearserieterminal).Serie, "Designation", null);
                BOWidgetBox boxSerie = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_series"), xpoComboBoxSerie);
                vboxTab1.PackStart(boxSerie, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxSerie, Entity, "Serie", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Terminal
                XPOComboBox xpoComboBoxTerminal = new XPOComboBox(Entity.Session, typeof(pos_configurationplaceterminal), (Entity as fin_documentfinanceyearserieterminal).Terminal, "Designation", null);
                BOWidgetBox boxTerminal = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationplaceterminal"), xpoComboBoxTerminal);
                vboxTab1.PackStart(boxTerminal, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTerminal, Entity, "Terminal", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(Entity.Session, typeof(sys_configurationprinters), (Entity as fin_documentfinanceyearserieterminal).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationprinter"), xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrinter, Entity, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(Entity.Session, typeof(sys_configurationprinterstemplates), (Entity as fin_documentfinanceyearserieterminal).Template, "Designation", "FinancialTemplate = 1");
                BOWidgetBox boxTemplate = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationprintersTemplate"), xpoComboBoxTemplate);
                vboxTab1.PackStart(boxTemplate, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTemplate, Entity, "Template", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

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