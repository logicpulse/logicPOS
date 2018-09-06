using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.shared;
using System;
using System.Drawing;
using System.Reflection;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    //T (XPGuidObject|DataRow)
    abstract class GenericCRUDWidget<T>
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Public Properties : Parameters
        protected Widget _widget;
        public Widget Widget
        {
            get { return _widget; }
            set { _widget = value; }
        }

        protected Label _label;
        public Label Label
        {
            get { return _label; }
            set { _label = value; }
        }

        protected T _dataSourceRow;
        public T DataSourceRow
        {
            get { return _dataSourceRow; }
            set { _dataSourceRow = value; }
        }

        protected string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        //Assigned in child GenericCRUDWidgetXPO.SetMembersProperties
        protected object _fieldValue;
        public object FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }

        protected string _validationRule;
        public string ValidationRule
        {
            get { return _validationRule; }
            set { _validationRule = value; }
        }

        protected bool _required;
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        //Stores result from Validation Function, always default to true, this way is true when we dont have a defined Func (OPTIONAL)
        protected bool _validatedFunc;
        public bool ValidatedFunc
        {
            get { return _validatedFunc; }
            set { _validatedFunc = value; }
        }

        protected bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set { _validated = value; }
        }

        protected Type _fieldType;
        public Type FieldType
        {
            get { return _fieldType; }
            set { _fieldType = value; }
        }

        protected PropertyInfo _fieldProperty;
        public PropertyInfo FieldProperty
        {
            get { return _fieldProperty; }
            set { _fieldProperty = value; }
        }

        public GenericCRUDWidget(Widget pWidget, T pDataSourceRow, string pFieldName, string pValidationRule = "", bool pRequired = false)
        {
            InitObject(pWidget, null, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public GenericCRUDWidget(Widget pWidget, Label pLabel, T pDataSourceRow, string pFieldName, string pValidationRule = "", bool pRequired = false)
        {
            InitObject(pWidget, pLabel, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Abstract Methods to be Implemented by SubClass/Child Classes

        //Must Be Overridden
        public abstract object GetMemberValue();

        //Must Be Overridden
        public abstract void SetMemberValue(object pValue);

        //Must Be Overridden
        public abstract void SetMembersProperties(T pDataSourceRow);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //InitObject

        protected void InitObject(Widget pWidget, Label pLabel, T pDataSourceRow, String pFieldName, String pValidationRule = "", bool pRequired = false)
        {
            //Parameters
            _widget = pWidget;
            _label = pLabel;
            _dataSourceRow = pDataSourceRow;
            _fieldName = pFieldName;
            _required = pRequired;
            _validationRule = pValidationRule;

            if (_dataSourceRow != null)
            {
                //Call SubClass Implementation
                SetMembersProperties(pDataSourceRow);
                InitField();
            }
            else
            {
                _widget.Sensitive = false;
            }
        }

        public void InitField()
        {
            //_log.Debug("Field:[{0}] Type:[{1}] FieldType:[{2}] ValidationRule:[{3}]", Field, FieldType, Widget.GetType().Name, ValidationRule);

            switch (_widget.GetType().Name)
            {
                case "Entry":
                    //InitialValue
                    //Require to replace NumberDecimalSeparator
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as Entry).Text = (String)Convert.ChangeType(GetMemberValue(), typeof(String), GlobalFramework.CurrentCultureNumberFormat);
                            //Decimal : Replace Visual Decimal Separator if is .
                            if (_fieldType == typeof(decimal) && GlobalFramework.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                                (_widget as Entry).Text = (_widget as Entry).Text.Replace('.', ',');
                            //DateTime
                            if (_fieldType == typeof(DateTime))
                            {
                                //Cast _fieldValue to DateTime
                                (_widget as Entry).Text = ((DateTime)_fieldValue).ToString(SettingsApp.DateTimeFormat);
                                //_log.Debug(string.Format("{0}:{1}:{2}:{3}:{4}", _dataSourceRow, _fieldName, _fieldValue, _fieldProperty, _fieldType));
                            }
                        }
                        catch
                        {
                            (_widget as Entry).Text = string.Empty;
                        }
                    }
                    else
                    {
                        (_widget as Entry).Text = string.Empty;
                    };
                    //Validation
                    if (_validationRule != string.Empty)
                    {
                        ValidateField();
                        (_widget as Entry).Changed += delegate { ValidateField(); };
                    }
                    break;

                case "TextView":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as TextView).Buffer.Text = (String)Convert.ChangeType(GetMemberValue(), typeof(String));
                        }
                        catch
                        {
                            (_widget as TextView).Buffer.Text = string.Empty;
                        };
                    }
                    else
                    {
                        (_widget as TextView).Buffer.Text = string.Empty;
                    };
                    //Validation
                    if (_validationRule != string.Empty)
                    {
                        ValidateField();
                        (_widget as TextView).Buffer.Changed += delegate { ValidateField(); };
                    }
                    break;

                case "EntryMultiline":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as EntryMultiline).Value.Text = (String)Convert.ChangeType(GetMemberValue(), typeof(String));
                        }
                        catch
                        {
                            (_widget as EntryMultiline).Value.Text = string.Empty;
                        };
                    }
                    else
                    {
                        (_widget as EntryMultiline).Value.Text = string.Empty;
                    };
                    //Validation
                    if (_validationRule != string.Empty)
                    {
                        ValidateField();
                        (_widget as EntryMultiline).Value.Changed += delegate { ValidateField(); };
                    }
                    break;

                case "CheckButton":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as CheckButton).Active = (bool)Convert.ChangeType(GetMemberValue(), typeof(bool));
                        }
                        catch
                        {
                            (_widget as CheckButton).Active = default(bool);
                        }
                    }
                    else
                    {
                        (_widget as CheckButton).Active = default(bool);
                    }
                    break;

                case "Label":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as Label).Text = (String)Convert.ChangeType(GetMemberValue(), typeof(String));
                        }
                        catch
                        {
                            (_widget as Label).Text = string.Empty;
                        }
                    }
                    else
                    {
                        (_widget as Label).Text = string.Empty;
                    }
                    break;

                case "FileChooserButton":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            (_widget as FileChooserButton).SetFilename((String)Convert.ChangeType(GetMemberValue(), typeof(String)));
                        }
                        catch
                        {
                            (_widget as FileChooserButton).SetFilename(default(String));
                        }
                    }
                    else
                    {
                        (_widget as FileChooserButton).SetFilename(default(String));
                    }
                    //Validation
                    if (_required)
                    {
                        ValidateField();
                        (_widget as FileChooserButton).FileSet += delegate { ValidateField(); };
                    }
                    break;

                case "XPOComboBox":
                    //InitialValue : Defined in XPOComboBox Object
                    //Validation
                    //Removed check Required to ValidateField, else wont work in no Required Fields
                    //if (_required)
                    //{
                    ValidateField();
                    (_widget as XPOComboBox).Changed += delegate { ValidateField(); };
                    //}
                    break;

                //Used outside BackOffice (Touch)
                case "EntryBoxValidation":
                    //Call Validate
                    (_widget as EntryBoxValidation).EntryValidation.Validate();
                    ValidateField();
                    (_widget as EntryBoxValidation).EntryValidation.Changed += delegate
                    {
                        (_widget as EntryBoxValidation).EntryValidation.Validate();
                        ValidateField();
                    };
                    break;

                /* OLD: Not Used Anymore : Not always use Unified component EntryBoxValidation
                case "EntryBox":
                    //InitialValue
                    if (GetMemberValue() != null)
                    {
                        try
                        {
                            if (_fieldType != typeof(decimal))
                            {
                                (_widget as EntryBoxValidation).EntryValidation.Text = (String)Convert.ChangeType(GetMemberValue(), typeof(String));
                            }
                            //Replace Visual Decimal Separator if is .
                            else
                            {
                                (_widget as EntryBoxValidation).EntryValidation.Text = FrameworkUtils.DecimalToString(Convert.ToDecimal(GetMemberValue()));

                                if (GlobalFramework.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                                {
                                    (_widget as EntryBoxValidation).EntryValidation.Text = (_widget as EntryBoxValidation).EntryValidation.Text.Replace('.', ',');
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.Message, ex);
                            (_widget as EntryBoxValidation).EntryValidation.Text = string.Empty;
                        };
                    }
                    else
                    {
                        (_widget as EntryBoxValidation).EntryValidation.Text = string.Empty;
                    };
                    //Validation
                    if (_required)
                    {
                        ValidateField();
                        (_widget as EntryBoxValidation).EntryValidation.Changed += delegate
                        {
                            ValidateField();
                        };
                    }
                    break;
                */

                case "XPOEntryBoxSelectRecord`2": //Note from `2 Generics <T1,T2>
                    //Required to use Dynamic, we cant cast without knowing generic types <T,T>
                    //WRONG CAST > (Widget as XPOEntryBoxSelectRecord<XPGuidObject,GenericTreeViewXPO>).Entry.Changed += delegate { ValidateField(); };
                    dynamic dynamicWiget = _widget;
                    //InitialValue : Defined in XPOComboBox Object
                    //Validation
                    if (_required)
                    {
                        ValidateField();
                        //Require a Know Type to use delegate
                        Entry entry = dynamicWiget.Entry;
                        entry.Changed += delegate { ValidateField(); };
                    }
                    break;
                //Default
                default:
                    break;
            };
        }

        //TODO: Check FileChooserButton|XPOComboBox|XPOEntryBoxSelectRecord Types with new pFunc Parameter
        public void ValidateField()
        {
            ValidateField(null);
        }

        //Use Custom Validate Function ex ValidateFiscalNumberFunc() in DialogCustomer
        public void ValidateField(Func<bool> pFunc)
        {
            Color colorEntryValidationValidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
            Color colorEntryValidationInvalidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidFont"]);

            //Always True, in case of null pFunc
            _validatedFunc = true;

            //Entry
            if (_widget.GetType() == typeof(Entry))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(pFunc, (_widget as Entry).Text);
                //Style Widget
                var currentWidget = (_widget as Entry);
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //TextView
            else if (_widget.GetType() == typeof(TextView))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(pFunc, (_widget as TextView).Buffer.Text);
                //Style Widget
                var currentWidget = (_widget as TextView);
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //EntryMultiline
            else if (_widget.GetType() == typeof(EntryMultiline))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(pFunc, (_widget as EntryMultiline).Value.Text);
                //Style Widget
                var currentWidget = (_widget as EntryMultiline);
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //XPOComboBox
            else if (_widget.GetType() == typeof(XPOComboBox))
            {
                //Required + Validate
                if ((_widget as XPOComboBox).Value != null)
                {
                    _validated = (FrameworkUtils.ValidateString((_widget as XPOComboBox).Value.Oid.ToString(), _validationRule));
                    //Call Validate Func Here
                    if (pFunc != null) _validatedFunc = pFunc();

                }
                else if (_required && (_widget as XPOComboBox).Value == null)
                {
                    _validated = false;
                }
                else if (!_required && (_widget as XPOComboBox).Value == null)
                {
                    _validated = true;
                };
                //Style Widget
                var currentWidget = (_widget as XPOComboBox);
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //TODO :WIP - Cant Change FileChooserButton BackGround
            //FileChooserButton
            else if (_widget.GetType() == typeof(FileChooserButton))
            {
                //Required
                if (_required && (_widget as FileChooserButton).Filename != null)
                {
                    _validated = true;
                    //Call Validate Func Here
                    if (pFunc != null) _validatedFunc = pFunc();
                }
                else
                {
                    _validated = false;
                }
                //Style Widget
                //var currentWidget = (_widget as FileChooserButton);
                //Call update Widget UI 
                //UpdateWidget(currentWidget, (_validated && validatedFunc));
            }

            //EntryBoxValidation
            else if (_widget.GetType() == typeof(EntryBoxValidation))
            {
                //Call Shared ValidateTextWidgets Method
                ValidateTextWidgets(pFunc, (_widget as EntryBoxValidation).EntryValidation.Text);
                //Style Widget
                var currentWidget = (_widget as EntryBoxValidation).EntryValidation;
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //XPOEntryBoxSelectRecord
            else if (_widget.GetType().IsGenericType && _widget.GetType().GetGenericTypeDefinition() == typeof(XPOEntryBoxSelectRecord<,>))
            {
                //Required to use dynamics to get Widget on Runtime, Because we can Cast it, Because we dont Know <T,T>, 
                //and We cant Cast Base (Widget as XPOEntryBoxSelectRecord<XPGuidObject,GenericTreeViewXPO>) Because it doesn works, 
                //Cast ony works if we use Know Types ex (Widget as XPOEntryBoxSelectRecordValidation<ConfigurationCountry, TreeViewConfigurationCountry>)
                //This way we Get Widget with Dynamics on RunTime with Base Knowed Types <XPGuidObject, GenericTreeViewXPO>
                dynamic dynamicWiget = _widget;

                //Required + Validate
                if (dynamicWiget.Value != null)
                {
                    _validated = (FrameworkUtils.ValidateString(dynamicWiget.Value.Oid.ToString(), _validationRule));
                    //Call Validate Func Here
                    if (pFunc != null) _validatedFunc = pFunc();
                }
                else if (_required && dynamicWiget.Value == null)
                {
                    _validated = false;
                }
                else if (
                  !_required && dynamicWiget.Value == null
                  )
                {
                    _validated = true;
                };
                //Style Widget
                var currentWidget = dynamicWiget.Entry;
                //Call update Widget UI 
                UpdateWidget(currentWidget, (_validated && _validatedFunc));
            }

            //Shared for all type of Components - Style the Widget Label
            if (_label != null)
            {
                if (_validated && _validatedFunc)
                {
                    _label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                }
                else
                {
                    _label.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                }
            }
        }

        //Shared CODE for all Text Widgets (Entry,TextView,EntryBox) 
        //only Changes (_widget as Object).[?.]Text ex (_widget as Entry).Text|(_widget as TextView).Buffer.Text|(_widget as EntryBox).Entry.Text
        public void ValidateTextWidgets(Func<bool> pFunc, string pText)
        {
            if (!_required && (_validationRule == string.Empty || pText == string.Empty))
            {
                _validated = true;
                //Call Validate Func Here
                if (pFunc != null && pText != string.Empty) _validatedFunc = pFunc();
            }
            else if (_required && pText == string.Empty)
            {
                _validated = false;
            }
            //Required + Validate + Validate Func
            else if (pText != string.Empty)
            {
                _validated = (FrameworkUtils.ValidateString(pText, _validationRule, _fieldType));
                //Call Validate Func Here
                if (pFunc != null) _validatedFunc = pFunc();
            };
        }

        //Shared CODE for Change Widgets Colors 
        public void UpdateWidget(dynamic pCurrentWidget, bool pIsValid)
        {
            Color colorEntryValidationValidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
            Color colorEntryValidationInvalidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidFont"]);
            Color colorEntryValidationValidBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidBackground"]);
            Color colorEntryValidationInvalidBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidBackground"]);

            //Override currentWidget reference, to access inner TextView
            if (pCurrentWidget.GetType() == typeof(EntryMultiline))
            {
                pCurrentWidget = pCurrentWidget.TextView;
            }

            if (pIsValid)
            {
                pCurrentWidget.ModifyText(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                pCurrentWidget.ModifyText(StateType.Active, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                pCurrentWidget.ModifyBase(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidBackground));
            }
            else
            {
                pCurrentWidget.ModifyText(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                pCurrentWidget.ModifyText(StateType.Active, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                pCurrentWidget.ModifyBase(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidBackground));
            }
        }
    }
}
