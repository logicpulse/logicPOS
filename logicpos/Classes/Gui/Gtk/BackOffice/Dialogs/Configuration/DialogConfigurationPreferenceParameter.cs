using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPreferenceParameter : BOBaseDialog
    {
        public DialogConfigurationPreferenceParameter(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_configurationpreferenceparameter);
            SetSizeRequest(500, 331);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                CFG_ConfigurationPreferenceParameter dataSourceRow = (CFG_ConfigurationPreferenceParameter) _dataSourceRow;

                //Define Label for Value
                string valueLabel = (Resx.ResourceManager.GetString(dataSourceRow.ResourceString) != null) 
                    ? Resx.ResourceManager.GetString(dataSourceRow.ResourceString)
                    : "LABEL NOT DEFINED IN Field  [ResourceString]";

                //Define RegEx for Value
                string valueRegEx = "REGULAR EXPRESSION NOT DEFINED IN Field [RegEx]";
                if (dataSourceRow.RegEx != null) {
                    //Try to get Value
                    object objectValueRegEx = FrameworkUtils.GetFieldValueFromType(typeof(SettingsApp), dataSourceRow.RegEx);
                    if (objectValueRegEx != null) valueRegEx = objectValueRegEx.ToString();
                }

                //Define Label for Value
                bool valueRequired = (dataSourceRow.Required) 
                    ? dataSourceRow.Required
                    : false;

                //Override Db Regex
                if (dataSourceRow.Token == "COMPANY_POSTALCODE") valueRegEx = SettingsApp.ConfigurationSystemCountry.RegExZipCode;
                if (dataSourceRow.Token == "COMPANY_FISCALNUMBER") valueRegEx = SettingsApp.ConfigurationSystemCountry.RegExFiscalNumber;

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(Resx.global_record_order, entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(Resx.global_record_code, entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Token
                Entry entryToken = new Entry();
                BOWidgetBox boxToken = new BOWidgetBox(Resx.global_token, entryToken);
                vboxTab1.PackStart(boxToken, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxToken, _dataSourceRow, "Token", SettingsApp.RegexAlfaNumericExtended, true));

                //Value
                Entry entryValue = new Entry();
                BOWidgetBox boxValue = new BOWidgetBox(valueLabel, entryValue);
                vboxTab1.PackStart(boxValue, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", valueRegEx, valueRequired));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryToken.Sensitive = false;
            
                //Disable if is COMPANY_FISCALNUMBER
                CFG_ConfigurationPreferenceParameter parameter = (_dataSourceRow as CFG_ConfigurationPreferenceParameter);
                entryValue.Sensitive = (
                    parameter.Token != "COMPANY_NAME"
                    && parameter.Token != "COMPANY_BUSINESS_NAME" 
                    && parameter.Token != "COMPANY_COUNTRY" 
                    && parameter.Token != "COMPANY_COUNTRY" 
                    && parameter.Token != "COMPANY_COUNTRY_CODE2" 
                    && parameter.Token != "COMPANY_FISCALNUMBER" 
                    && parameter.Token != "SYSTEM_CURRENCY" 
                    //&& parameter.Token != "COMPANY_CIVIL_REGISTRATION" 
                    //&& parameter.Token != "COMPANY_CIVIL_REGISTRATION_ID"
                );
            }
            catch (System.Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
