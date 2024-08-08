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
    internal class DialogConfigurationPoleDisplay : EditDialog
    {
        public DialogConfigurationPoleDisplay(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_configurationpoledisplay"));

            SetSizeRequest(500, 490);
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

                // HBox For Vid and Pid
                HBox hbox1 = new HBox(true, _boxSpacing);
                // HBox For Vid and Pid
                HBox hbox2 = new HBox(true, _boxSpacing);

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

                //VID
                Entry entryVID = new Entry();
                BOWidgetBox boxVID = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_vid"), entryVID);
                hbox1.PackStart(boxVID, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxVID, Entity, "VID", LogicPOS.Utility.RegexUtils.RegexHardwareVidAndPid, true));

                //PID
                Entry entryPID = new Entry();
                BOWidgetBox boxPID = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_pid"), entryPID);
                hbox1.PackStart(boxPID, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPID, Entity, "PID", LogicPOS.Utility.RegexUtils.RegexHardwareVidAndPid, true));

                //EndPoint
                Entry entryEndPoint = new Entry();
                BOWidgetBox boxEndPoint = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_endpoint"), entryEndPoint);
                hbox1.PackStart(boxEndPoint, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxEndPoint, Entity, "EndPoint", LogicPOS.Utility.RegexUtils.RegexHardwareEndpoint, true));

                //COM
                Entry entryCOM = new Entry();
                BOWidgetBox boxCOM = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_com_port"), entryCOM);
                hbox1.PackStart(boxCOM, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCOM, Entity, "COM", LogicPOS.Utility.RegexUtils.RegexHardwarePortName, true));

                // Pack hboxVIDAndPid
                vboxTab1.PackStart(hbox1, false, false, 0);

                //CodeTable
                Entry entryCodeTable = new Entry();
                BOWidgetBox boxCodeTable = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_codetable"), entryCodeTable);
                hbox2.PackStart(boxCodeTable, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCodeTable, Entity, "CodeTable", LogicPOS.Utility.RegexUtils.RegexHardwareCodeTable, true));

                //DisplayCharactersPerLine
                Entry entryDisplayCharactersPerLine = new Entry();
                BOWidgetBox boxDisplayCharactersPerLine = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_number_of_characters_per_line"), entryDisplayCharactersPerLine);
                hbox2.PackStart(boxDisplayCharactersPerLine, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDisplayCharactersPerLine, Entity, "DisplayCharactersPerLine", LogicPOS.Utility.RegexUtils.RegexInteger, true));


                //GoToStandByInSeconds
                Entry entryGoToStandByInSeconds = new Entry();
                BOWidgetBox boxGoToStandByInSeconds = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_goto_stand_by_in_seconds"), entryGoToStandByInSeconds);
                hbox2.PackStart(boxGoToStandByInSeconds, true, true, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxGoToStandByInSeconds, Entity, "GoToStandByInSeconds", LogicPOS.Utility.RegexUtils.RegexInteger, true));

                // Pack hboxEndpointAndCodeTableAndGoToStandByInSeconds
                vboxTab1.PackStart(hbox2, false, false, 0);

                //StandByLine1
                Entry entryStandByLine1 = new Entry();
                BOWidgetBox boxStandByLine1 = new BOWidgetBox(string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_stand_by_line_no"), 1), entryStandByLine1);
                vboxTab1.PackStart(boxStandByLine1, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxStandByLine1, Entity, "StandByLine1", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //StandByLine2
                Entry entryStandByLine2 = new Entry();
                BOWidgetBox boxStandByLine2 = new BOWidgetBox(string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_pole_display_stand_by_line_no"), 2), entryStandByLine2);
                vboxTab1.PackStart(boxStandByLine2, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxStandByLine2, Entity, "StandByLine2", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

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
