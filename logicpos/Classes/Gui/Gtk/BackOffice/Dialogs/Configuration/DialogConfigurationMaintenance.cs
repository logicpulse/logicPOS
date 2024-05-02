using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Enums.Dialogs;
using logicpos.datalayer.App;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogConfigurationMaintenance : BOBaseDialog
    {
        public DialogConfigurationMaintenance(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_edit_configurationmaintenance"));
            
            SetSizeRequest(500, 500);
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
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Date
                Entry entryDate = new Entry();
                BOWidgetBox boxDate = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationMaintenance_Date"), entryDate);
                vboxTab1.PackStart(boxDate, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDate, _dataSourceRow, "Date", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));
                
                //Time
                Entry entryTime = new Entry();
                BOWidgetBox boxTime = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationMaintenance_Time"), entryTime);
                vboxTab1.PackStart(boxTime, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxTime, _dataSourceRow, "Time", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //PasswordAccess
                Entry entryPasswordAccess = new Entry();
                BOWidgetBox boxPasswordAccess = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationMaintenance_PasswordAccess"), entryPasswordAccess);
                vboxTab1.PackStart(boxPasswordAccess, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPasswordAccess, _dataSourceRow, "PasswordAccess", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Remarks
                Entry entryRemarks = new Entry();
                BOWidgetBox boxRemarks = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationMaintenance_Remarks"), entryRemarks);
                vboxTab1.PackStart(boxRemarks, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxRemarks, _dataSourceRow, "Remarks", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
