using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogDocumentFinanceType : EditDialog
    {
        public DialogDocumentFinanceType(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_template"));
            SetSizeRequest(400, 514);
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

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Acronym
                Entry entryAcronym = new Entry();
                BOWidgetBox boxAcronym = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_acronym"), entryAcronym);
                vboxTab1.PackStart(boxAcronym, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAcronym, Entity, "Acronym", LogicPOS.Utility.RegexUtils.RegexAlfa, true));

                //PrintCopies
                Entry entryPrintCopies = new Entry();
                BOWidgetBox boxPrintCopies = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_print_copies"), entryPrintCopies);
                vboxTab1.PackStart(boxPrintCopies, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrintCopies, Entity, "PrintCopies", LogicPOS.Utility.RegexUtils.RegexPrintCopies, true));

                //Printer
                XPOComboBox xpoComboBoxPrinter = new XPOComboBox(Entity.Session, typeof(sys_configurationprinters), (Entity as fin_documentfinancetype).Printer, "Designation", null);
                BOWidgetBox boxPrinter = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_device_printer"), xpoComboBoxPrinter);
                vboxTab1.PackStart(boxPrinter, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPrinter, Entity, "Printer", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //Template
                XPOComboBox xpoComboBoxTemplate = new XPOComboBox(Entity.Session, typeof(sys_configurationprinterstemplates), (Entity as fin_documentfinancetype).Template, "Designation", null);
                BOWidgetBox boxTemplate = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationprintersTemplate"), xpoComboBoxTemplate);
                vboxTab1.PackStart(boxTemplate, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxTemplate, Entity, "Template", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //PrintRequestConfirmation
                CheckButton checkButtonPrintRequestConfirmation = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_print_request_confirmation"));
                vboxTab1.PackStart(checkButtonPrintRequestConfirmation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonPrintRequestConfirmation, Entity, "PrintRequestConfirmation"));

                //PrintOpenDrawer
                CheckButton checkButtonPrintOpenDrawer = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_open_drawer"));
                vboxTab1.PackStart(checkButtonPrintOpenDrawer, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonPrintOpenDrawer, Entity, "PrintOpenDrawer"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryDesignation.Sensitive = (_dialogMode == DialogMode.Insert);
                entryAcronym.Sensitive = (_dialogMode == DialogMode.Insert);
                xpoComboBoxTemplate.Sensitive = (_dialogMode == DialogMode.Insert);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

