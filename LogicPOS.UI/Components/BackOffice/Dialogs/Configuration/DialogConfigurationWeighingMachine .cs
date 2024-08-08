using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationWeighingMachine : EditDialog
    {
        public DialogConfigurationWeighingMachine(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_configurationweighingmachine"));

            SetSizeRequest(500, 534);
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

                //PortName
                Entry entryPortName = new Entry();
                BOWidgetBox boxPortName = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_com_portname"), entryPortName);
                vboxTab1.PackStart(boxPortName, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPortName, Entity, "PortName", LogicPOS.Utility.RegexUtils.RegexHardwarePortName, true));

                //BaudRate
                Entry entryBaudRate = new Entry();
                BOWidgetBox boxBaudRate = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_com_baudrate"), entryBaudRate);
                vboxTab1.PackStart(boxBaudRate, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxBaudRate, Entity, "BaudRate", LogicPOS.Utility.RegexUtils.RegexHardwareBaudRate, true));

                //Parity
                Entry entryParity = new Entry();
                BOWidgetBox boxParity = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_com_parity"), entryParity);
                vboxTab1.PackStart(boxParity, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxParity, Entity, "Parity", LogicPOS.Utility.RegexUtils.RegexHardwareParity, true));

                //StopBits
                Entry entryStopBits = new Entry();
                BOWidgetBox boxStopBits = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_com_stopbits"), entryStopBits);
                vboxTab1.PackStart(boxStopBits, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxStopBits, Entity, "StopBits", LogicPOS.Utility.RegexUtils.RegexHardwareStopBits, true));

                //DataBits
                Entry entryDataBits = new Entry();
                BOWidgetBox boxDataBits = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_com_databits"), entryDataBits);
                vboxTab1.PackStart(boxDataBits, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDataBits, Entity, "DataBits", LogicPOS.Utility.RegexUtils.RegexHardwareDataBits, true));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
