using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Reflection;

namespace LogicPOS.UI.Components.InputFieds
{
    internal abstract class InputField<TEntity>
    {
        public Widget Widget { get; set; }
        public Label Label { get; set; }
        public TEntity Entity { get; set; }
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public string ValidationRule { get; set; }
        public bool Required { get; set; }
        public bool ValidationFunctionResult { get; set; } 
        public bool Validated { get; set; }
        public Type FieldType { get; set; }
        public PropertyInfo FieldProperty { get; set; }

        public InputField(Widget widget,
                          TEntity entity,
                          string name,
                          Label label = null,
                          string validationRule = "",
                          bool required = false)
        {
            Build(widget,
                       label,
                       entity,
                       name,
                       validationRule,
                       required);
        }

        public abstract object GetFieldValueFromEntity();

        public abstract void SetMemberValue(object value);

        public abstract void SetMembersProperties(TEntity entity);

        protected void Build(Widget widget,
                             Label label,
                             TEntity entity,
                             string fieldName,
                             string validationRule = "",
                             bool required = false)
        {
            Widget = widget;
            Label = label;
            Entity = entity;
            FieldName = fieldName;
            Required = required;
            ValidationRule = validationRule;

            if (Entity != null)
            {
                SetMembersProperties(entity);
                InitField();
                return;
            }

            Widget.Sensitive = false;
        }

        public void InitField()
        {
            switch (Widget.GetType().Name)
            {
                case "Entry":
                    var textField = Widget as Entry;
                    if (GetFieldValueFromEntity() != null)
                    {     
                        textField.Text = (string)Convert.ChangeType(GetFieldValueFromEntity(), typeof(string), Settings.CultureSettings.CurrentCultureNumberFormat);

                        if (FieldType == typeof(decimal) && Settings.CultureSettings.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                        {
                            textField.Text = textField.Text.Replace('.', ',');
                        }

                        if (FieldType == typeof(DateTime))
                        {
                            textField.Text = ((DateTime)FieldValue).ToString(Settings.CultureSettings.DateTimeFormat);
                        }
                    }
                    else
                    {
                        textField.Text = string.Empty;
                    }
                    

                    if (ValidationRule != string.Empty)
                    {
                        ValidateField();
                        (Widget as Entry).Changed += delegate { ValidateField(); };
                    }
                    break;

                case "TextView":
                    //InitialValue
                    if (GetFieldValueFromEntity() != null)
                    {
                        try
                        {
                            (Widget as TextView).Buffer.Text = (string)Convert.ChangeType(GetFieldValueFromEntity(), typeof(string));
                        }
                        catch
                        {
                            (Widget as TextView).Buffer.Text = string.Empty;
                        };
                    }
                    else
                    {
                        (Widget as TextView).Buffer.Text = string.Empty;
                    };
                    //Validation
                    if (ValidationRule != string.Empty)
                    {
                        ValidateField();
                        (Widget as TextView).Buffer.Changed += delegate { ValidateField(); };
                    }
                    break;

                case "EntryMultiline":
                    //InitialValue
                    if (GetFieldValueFromEntity() != null)
                    {
                        try
                        {
                            (Widget as MultilineTextBox).Value.Text = (string)Convert.ChangeType(GetFieldValueFromEntity(), typeof(string));
                        }
                        catch
                        {
                            (Widget as MultilineTextBox).Value.Text = string.Empty;
                        };
                    }
                    else
                    {
                        (Widget as MultilineTextBox).Value.Text = string.Empty;
                    };
                    //Validation
                    if (ValidationRule != string.Empty)
                    {
                        ValidateField();
                        (Widget as MultilineTextBox).Value.Changed += delegate { ValidateField(); };
                    }
                    break;

                case "CheckButton":
                    //InitialValue
                    if (GetFieldValueFromEntity() != null)
                    {
                        try
                        {
                            (Widget as CheckButton).Active = (bool)Convert.ChangeType(GetFieldValueFromEntity(), typeof(bool));
                        }
                        catch
                        {
                            (Widget as CheckButton).Active = default;
                        }
                    }
                    else
                    {
                        (Widget as CheckButton).Active = default;
                    }
                    break;

                case "Label":
                    //InitialValue
                    if (GetFieldValueFromEntity() != null)
                    {
                        try
                        {
                            (Widget as Label).Text = (string)Convert.ChangeType(GetFieldValueFromEntity(), typeof(string));
                        }
                        catch
                        {
                            (Widget as Label).Text = string.Empty;
                        }
                    }
                    else
                    {
                        (Widget as Label).Text = string.Empty;
                    }
                    break;

                case "FileChooserButton":
                    //InitialValue
                    if (GetFieldValueFromEntity() != null)
                    {
                        try
                        {
                            (Widget as FileChooserButton).SetFilename((string)Convert.ChangeType(GetFieldValueFromEntity(), typeof(string)));
                        }
                        catch
                        {
                            (Widget as FileChooserButton).SetFilename(default);
                        }
                    }
                    else
                    {
                        (Widget as FileChooserButton).SetFilename(default);
                    }
                    //Validation
                    if (Required)
                    {
                        ValidateField();
                        (Widget as FileChooserButton).FileSet += delegate { ValidateField(); };
                    }
                    break;

                case "XPOComboBox":
                    //InitialValue : Defined in XPOComboBox Object
                    //Validation
                    //Removed check Required to ValidateField, else wont work in no Required Fields
                    //if (_required)
                    //{
                    ValidateField();
                    (Widget as XPOComboBox).Changed += delegate { ValidateField(); };
                    //}
                    break;

                //Used outside BackOffice (Touch)
                case "EntryBoxValidation":
                    //Call Validate
                    (Widget as EntryBoxValidation).EntryValidation.Validate();
                    ValidateField();
                    (Widget as EntryBoxValidation).EntryValidation.Changed += delegate
                    {
                        (Widget as EntryBoxValidation).EntryValidation.Validate();
                        ValidateField();
                    };
                    break;

                case "XPOEntryBoxSelectRecord`2":
                    dynamic dynamicWiget = Widget;

                    if (Required)
                    {
                        ValidateField();
                        Entry entry = dynamicWiget.Entry;
                        entry.Changed += delegate { ValidateField(); };
                    }
                    break;
                //Default
                default:
                    break;
            };
        }

        public void ValidateField()
        {
            ValidateField(null);
        }

        public void ValidateField(Func<bool> function)
        {
            Color validFontColor = Settings.AppSettings.Instance.colorEntryValidationValidFont;
            Color invalidFontColor = Settings.AppSettings.Instance.colorEntryValidationInvalidFont;

            ValidationFunctionResult = true;

            //Entry
            if (Widget.GetType() == typeof(Entry))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(function, (Widget as Entry).Text);
                //Style Widget
                var currentWidget = Widget as Entry;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //TextView
            else if (Widget.GetType() == typeof(TextView))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(function, (Widget as TextView).Buffer.Text);
                //Style Widget
                var currentWidget = Widget as TextView;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //EntryMultiline
            else if (Widget.GetType() == typeof(MultilineTextBox))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(function, (Widget as MultilineTextBox).Value.Text);
                //Style Widget
                var currentWidget = Widget as MultilineTextBox;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //XPOComboBox
            else if (Widget.GetType() == typeof(XPOComboBox))
            {
                //Required + Validate
                if ((Widget as XPOComboBox).Value != null)
                {
                    Validated = GeneralUtils.ValidateString((Widget as XPOComboBox).Value.Oid.ToString(), ValidationRule);
                    //Call Validate Func Here
                    if (function != null) ValidationFunctionResult = function();

                }
                else if (Required && (Widget as XPOComboBox).Value == null)
                {
                    Validated = false;
                }
                else if (!Required && (Widget as XPOComboBox).Value == null)
                {
                    Validated = true;
                };
                //Style Widget
                var currentWidget = Widget as XPOComboBox;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //TODO :WIP - Cant Change FileChooserButton BackGround
            //FileChooserButton
            else if (Widget.GetType() == typeof(FileChooserButton))
            {
                //Required
                if (Required && (Widget as FileChooserButton).Filename != null)
                {
                    Validated = true;
                    //Call Validate Func Here
                    if (function != null) ValidationFunctionResult = function();
                }
                else
                {
                    Validated = false;
                }
                //Style Widget
                //var currentWidget = (_widget as FileChooserButton);
                //Call update Widget UI 
                //UpdateWidget(currentWidget, (_validated && validatedFunc));
            }

            //EntryBoxValidation
            else if (Widget.GetType() == typeof(EntryBoxValidation))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(function, (Widget as EntryBoxValidation).EntryValidation.Text);
                //Style Widget
                var currentWidget = (Widget as EntryBoxValidation).EntryValidation;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //XPOEntryBoxSelectRecord
            else if (Widget.GetType().IsGenericType && Widget.GetType().GetGenericTypeDefinition() == typeof(XPOEntryBoxSelectRecord<,>))
            {
                //Required to use dynamics to get Widget on Runtime, Because we can Cast it, Because we dont Know <T,T>, 
                //and We cant Cast Base (Widget as XPOEntryBoxSelectRecord<XPGuidObject,GenericTreeViewXPO>) Because it doesn works, 
                //Cast ony works if we use Know Types ex (Widget as XPOEntryBoxSelectRecordValidation<ConfigurationCountry, TreeViewConfigurationCountry>)
                //This way we Get Widget with Dynamics on RunTime with Base Knowed Types <XPGuidObject, GenericTreeViewXPO>
                dynamic dynamicWiget = Widget;

                //Required + Validate
                if (dynamicWiget.Value != null)
                {
                    Validated = GeneralUtils.ValidateString(dynamicWiget.Value.Oid.ToString(), ValidationRule);
                    //Call Validate Func Here
                    if (function != null) ValidationFunctionResult = function();
                }
                else if (Required && dynamicWiget.Value == null)
                {
                    Validated = false;
                }
                else if (
                  !Required && dynamicWiget.Value == null
                  )
                {
                    Validated = true;
                };
                //Style Widget
                var currentWidget = dynamicWiget.Entry;
                //Call update Widget UI 
                UpdateWidget(currentWidget, Validated && ValidationFunctionResult);
            }

            //Shared for all type of Components - Style the Widget Label
            if (Label != null)
            {
                if (Validated && ValidationFunctionResult)
                {
                    Label.ModifyFg(StateType.Normal, validFontColor.ToGdkColor());
                }
                else
                {
                    Label.ModifyFg(StateType.Normal, invalidFontColor.ToGdkColor());
                }
            }
        }

        public void ValidateTextWidgets(Func<bool> function, string text)
        {
            if (Required == false && (ValidationRule == string.Empty || text == string.Empty))
            {
                Validated = true;
                if (function != null && text != string.Empty)
                {
                    ValidationFunctionResult = function();
                }
            }
            else if (Required && text == string.Empty)
            {
                Validated = false;
            }
            else if (text != string.Empty)
            {
                Validated = GeneralUtils.ValidateString(text, ValidationRule, FieldType);
                //Call Validate Func Here
                if (function != null) ValidationFunctionResult = function();
            };
        }

        public void UpdateWidget(dynamic pCurrentWidget, bool pIsValid)
        {
            Color colorEntryValidationValidFont = Settings.AppSettings.Instance.colorEntryValidationValidFont;
            Color colorEntryValidationInvalidFont = Settings.AppSettings.Instance.colorEntryValidationInvalidFont;
            Color colorEntryValidationValidBackground = Settings.AppSettings.Instance.colorEntryValidationValidBackground;
            Color colorEntryValidationInvalidBackground = Settings.AppSettings.Instance.colorEntryValidationInvalidBackground;

            //Override currentWidget reference, to access inner TextView
            if (pCurrentWidget.GetType() == typeof(MultilineTextBox))
            {
                pCurrentWidget = pCurrentWidget.TextView;
            }

            if (pIsValid)
            {
                pCurrentWidget.ModifyText(StateType.Normal, colorEntryValidationValidFont.ToGdkColor());
                pCurrentWidget.ModifyText(StateType.Active, colorEntryValidationValidFont.ToGdkColor());
                pCurrentWidget.ModifyBase(StateType.Normal, colorEntryValidationValidBackground.ToGdkColor());
            }
            else
            {
                pCurrentWidget.ModifyText(StateType.Normal, colorEntryValidationInvalidFont.ToGdkColor());
                pCurrentWidget.ModifyText(StateType.Active, colorEntryValidationInvalidFont.ToGdkColor());
                pCurrentWidget.ModifyBase(StateType.Normal, colorEntryValidationInvalidBackground.ToGdkColor());
            }
        }
    }
}
