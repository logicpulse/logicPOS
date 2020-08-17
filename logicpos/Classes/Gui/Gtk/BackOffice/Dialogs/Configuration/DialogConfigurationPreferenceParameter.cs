using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.resources;
using logicpos.resources.Resources.Localization;
using System;
using System.Configuration;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class DialogConfigurationPreferenceParameter : BOBaseDialog
    {
        private int _windowWidth = 500;
        private int _windowHeightForTextComponent = 331;
        private int _windowHeight = 0;

        public static void SaveSettings(string fieldName)
        {
            try
            {              

                Configuration config =  ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["customCultureResourceDefinition"].Value = fieldName;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

        }
        public DialogConfigurationPreferenceParameter(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_edit_configurationpreferenceparameter"));
            cfg_configurationpreferenceparameter dataSourceRow = (cfg_configurationpreferenceparameter)_dataSourceRow;
            // Default windowHeight, InputTypes can Override this in Switch             
            if (Utils.IsLinux)
            {
                _windowHeight = 391;
            }
            else 
            {
                _windowHeight = _windowHeightForTextComponent;
            }
            InitUI();
            SetSizeRequest(_windowWidth, _windowHeight);
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            BOWidgetBox boxValue = null;
            BOWidgetBox ComboboxValue = null;

            try
            {
                cfg_configurationpreferenceparameter dataSourceRow = (cfg_configurationpreferenceparameter)_dataSourceRow;

                //Define Label for Value
                string valueLabel = (resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], dataSourceRow.ResourceString) != null)
                    ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], dataSourceRow.ResourceString)
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
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", SettingsApp.RegexIntegerGreaterThanZero, true));

                //Token
                Entry entryToken = new Entry();
                BOWidgetBox boxToken = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_token"), entryToken);
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
                            entryValue.TooltipText = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_prefparam_value_tip_format"), dataSourceRow.ValueTip);
                        }
                        break;
                    case PreferenceParameterInputType.Multiline:
                        EntryMultiline entryMultiline = new EntryMultiline();
                        entryMultiline.Value.Text = dataSourceRow.Value;
                        entryMultiline.ScrolledWindow.BorderWidth = 1;
                        entryMultiline.HeightRequest = 122;
                        Label labelMultiline = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"));
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
                    //Mudar a lingua da Aplicação - Não é genérico
                    //IN009296 BackOffice - Mudar a língua da aplicação
                    case PreferenceParameterInputType.ComboBox:
                        string getCultureFromDB;
                        try
                        {
                            string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                            getCultureFromDB = GlobalFramework.SessionXpo.ExecuteScalar(sql).ToString();
                        }
                        catch
                        {
                            getCultureFromDB = GlobalFramework.Settings["customCultureResourceDefinition"];
                        }

                        string[] getCulturesValues = new string[8];
                        getCulturesValues[0] = "pt-PT";
                        getCulturesValues[1] = "pt-AO";
                        getCulturesValues[2] = "pt-BR";
                        getCulturesValues[3] = "pt-MZ";
                        getCulturesValues[4] = "en-GB";
                        getCulturesValues[5] = "en-US";
                        getCulturesValues[6] = "fr-FR";
                        getCulturesValues[7] = "es-ES";

                        string[] getCulturesLabels = new string[8];
                        getCulturesLabels[0] = "Português(Portugal)";
                        getCulturesLabels[1] = "Português(Angola)";
                        getCulturesLabels[2] = "Português(Brasil)";
                        getCulturesLabels[3] = "Português(Moçambique)";
                        getCulturesLabels[4] = "English(GB)";
                        getCulturesLabels[5] = "English(USA)";
                        getCulturesLabels[6] = "Françes";
                        getCulturesLabels[7] = "Espanol";

                        TreeIter iter;
                        TreeStore store = new TreeStore(typeof(string), typeof(string));
                        for (int i = 0; i < getCulturesLabels.Length; i++)
                        {
                            iter = store.AppendValues(getCulturesValues.GetValue(i), getCulturesLabels.GetValue(i));
                        }

                        ComboBox xpoComboBoxInputType = new ComboBox(getCulturesLabels);
                        
                        xpoComboBoxInputType.Model.GetIterFirst(out iter);
                        int cbox = 0;
                        do
                        {
                            GLib.Value thisRow = new GLib.Value();
                            xpoComboBoxInputType.Model.GetValue(iter, 0, ref thisRow);
                            //if ((thisRow.Val as string).Equals(getCultureFromDB))
                            if (getCulturesValues[cbox] == getCultureFromDB)
                            {
                                xpoComboBoxInputType.SetActiveIter(iter);
                                break;
                            }
                            cbox++;  
                        } while (xpoComboBoxInputType.Model.IterNext(ref iter));

                        ComboboxValue = new BOWidgetBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_language"), xpoComboBoxInputType);
                        vboxTab1.PackStart(ComboboxValue, false, false, 0);

                        entryValue.Text = getCulturesValues[xpoComboBoxInputType.Active];
                        entryValue.Visibility = false;

                        xpoComboBoxInputType.Changed += delegate
                        {
                            entryValue.Text = getCulturesValues[xpoComboBoxInputType.Active];
                            if (Utils.IsLinux)
                            {
                                SaveSettings(getCulturesValues[xpoComboBoxInputType.Active].ToString());
                            }
                            //GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(getCulturesValues[xpoComboBoxInputType.Active]);
                            //GlobalFramework.Settings["customCultureResourceDefinition"] = getCulturesValues[xpoComboBoxInputType.Active];
                            //CustomResources.UpdateLanguage(getCulturesValues[xpoComboBoxInputType.Active]);
                            //_crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", string.Empty, false));
                        };
                        boxValue = new BOWidgetBox(valueLabel, entryValue);         
                        _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxValue, _dataSourceRow, "Value", string.Empty, false));             
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
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Disable Components
                entryToken.Sensitive = false;

                //Protect PreferenceParameterInputType : Disable if is COMPANY_FISCALNUMBER or Other Sensitive Data
                cfg_configurationpreferenceparameter parameter = (_dataSourceRow as cfg_configurationpreferenceparameter);
                if (entryValue != null) entryValue.Sensitive = (
                    parameter.Token != "COMPANY_NAME"
                    && parameter.Token != "COMPANY_BUSINESS_NAME"
                    && parameter.Token != "COMPANY_COUNTRY"
                    && parameter.Token != "COMPANY_COUNTRY"
                    && parameter.Token != "COMPANY_COUNTRY_CODE2"
                    && parameter.Token != "COMPANY_FISCALNUMBER"
                    && parameter.Token != "SYSTEM_CURRENCY"
                    && parameter.Token != "REPORT_FILENAME_LOGO"
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
