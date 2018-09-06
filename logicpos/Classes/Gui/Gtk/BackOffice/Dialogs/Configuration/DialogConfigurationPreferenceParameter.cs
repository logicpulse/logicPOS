using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPreferenceParameter : BOBaseDialog
    {
        private int _windowWidth = 500;
        private int _windowHeightForTextComponent = 331;
        private int _windowHeight = 0;

        public DialogConfigurationPreferenceParameter(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(Resx.window_title_edit_configurationpreferenceparameter);
            CFG_ConfigurationPreferenceParameter dataSourceRow = (CFG_ConfigurationPreferenceParameter)_dataSourceRow;
            // Default windowHeight, InputTypes can Override this in Switch 
            _windowHeight = _windowHeightForTextComponent;
            InitUI();
            SetSizeRequest(_windowWidth, _windowHeight);
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            BOWidgetBox boxValue = null;

            try
            {
                CFG_ConfigurationPreferenceParameter dataSourceRow = (CFG_ConfigurationPreferenceParameter)_dataSourceRow;

                //Define Label for Value
                string valueLabel = (Resx.ResourceManager.GetString(dataSourceRow.ResourceString) != null)
                    ? Resx.ResourceManager.GetString(dataSourceRow.ResourceString)
                    : "LABEL NOT DEFINED IN Field  [ResourceString]";

                //Define RegEx for Value
                string valueRegEx = "REGULAR EXPRESSION NOT DEFINED IN Field [RegEx]";
                if (dataSourceRow.RegEx != null)
                {
                    //Try to get Value
                    object objectValueRegEx = FrameworkUtils.GetFieldValueFromType(typeof(SettingsApp), dataSourceRow.RegEx);
                    if (objectValueRegEx != null) valueRegEx = objectValueRegEx.ToString();
                }

                //Define Label for Value
                bool valueRequired = (dataSourceRow.Required)
                    ? dataSourceRow.Required
                    : false;

                //Override Db Regex with ConfigurationSystemCountry RegExZipCode and RegExFiscalNumber
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

                //Get InputType
                PreferenceParameterInputType inputType = (PreferenceParameterInputType)Enum.Parse(typeof(PreferenceParameterInputType), dataSourceRow.InputType.ToString(), true);

                Entry entryValue = new Entry();

                switch (inputType)
                {
                    case PreferenceParameterInputType.Text:
                    case PreferenceParameterInputType.TextPassword:
                        boxValue = new BOWidgetBox(valueLabel, entryValue);
                        vboxTab1.PackStart(boxValue, false, false, 0);
                        // Turn entry into a TextPassword, curently we not use it, until we can turn Visibility = false into TreeView Cell
                        if (inputType.Equals(PreferenceParameterInputType.TextPassword)) entryValue.Visibility = false;

                        _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", valueRegEx, valueRequired));
                        // ValueTip
                        if (!string.IsNullOrEmpty(dataSourceRow.ValueTip))
                        {
                            entryValue.TooltipText = string.Format(Resx.global_prefparam_value_tip_format, dataSourceRow.ValueTip);
                        }
                        break;
                    case PreferenceParameterInputType.Multiline:
                        EntryMultiline entryMultiline = new EntryMultiline();
                        entryMultiline.Value.Text = dataSourceRow.Value;
                        entryMultiline.ScrolledWindow.BorderWidth = 1;
                        entryMultiline.HeightRequest = 122;
                        Label labelMultiline = new Label(Resx.global_notes);
                        boxValue = new BOWidgetBox(valueLabel, entryMultiline);
                        vboxTab1.PackStart(boxValue, false, false, 0);
                        _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", valueRegEx, valueRequired));
                        // Override Default Window Height
                        _windowHeight = _windowHeightForTextComponent + 100;
                        break;
                    case PreferenceParameterInputType.CheckButton:
                        CheckButton checkButtonValue = new CheckButton(valueLabel);
                        vboxTab1.PackStart(checkButtonValue, false, false, 0);
                        _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonValue, _dataSourceRow, "Value"));
                        // Override Default Window Height
                        _windowHeight = _windowHeightForTextComponent - 20;
                        break;
                    case PreferenceParameterInputType.ComboBox:
                        break;
                    case PreferenceParameterInputType.FilePicker:
                    case PreferenceParameterInputType.DirPicker:
                        //FilePicker
                        FileChooserAction fileChooserAction = (inputType.Equals(PreferenceParameterInputType.FilePicker)) ? FileChooserAction.Open : FileChooserAction.SelectFolder;
                        FileChooserButton fileChooser = new FileChooserButton(string.Empty, fileChooserAction) { HeightRequest = 23 };
                        if (inputType.Equals(PreferenceParameterInputType.FilePicker))
                        {
                            fileChooser.SetFilename(dataSourceRow.Value);
                            fileChooser.Filter = Utils.GetFileFilterImages();
                        }
                        else
                        {
                            fileChooser.SetCurrentFolder(dataSourceRow.Value);
                        }
                        boxValue = new BOWidgetBox(valueLabel, fileChooser);
                        vboxTab1.PackStart(boxValue, false, false, 0);
                        _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", string.Empty, false));
                        break;
                    default:
                        break;
                }

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(Resx.global_record_main_detail));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryToken.Sensitive = false;

                //Protect PreferenceParameterInputType : Disable if is COMPANY_FISCALNUMBER or Other Sensitive Data
                CFG_ConfigurationPreferenceParameter parameter = (_dataSourceRow as CFG_ConfigurationPreferenceParameter);
                if (entryValue != null) entryValue.Sensitive = (
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
