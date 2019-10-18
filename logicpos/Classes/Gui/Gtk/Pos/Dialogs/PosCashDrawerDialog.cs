using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosCashDrawerDialog : PosBaseDialog
    {
        //Settings
        private int _decimalRoundTo = SettingsApp.DecimalRoundTo;
        //Private Properties
        //ResponseType (Above 10)
        private ResponseType _responseTypePrint = (ResponseType)11;
        //Buttons
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private TouchButtonIconWithText _buttonPrint;
        private TouchButtonBase _selectedCashDrawerButton;
        //UI
        private EntryBoxValidation _entryBoxMovementAmountMoney;
        private EntryBoxValidation _entryBoxMovementDescription;
        //private EntryBoxValidation _entryBoxMovementAmountOtherPayments;
        //Public
        private decimal _totalAmountInCashDrawer;
        public decimal TotalAmountInCashDrawer
        {
            get { return _totalAmountInCashDrawer; }
            set { _totalAmountInCashDrawer = value; }
        }
        private decimal _movementAmountMoney;
        public decimal MovementAmountMoney
        {
            get { return _movementAmountMoney; }
            set { _movementAmountMoney = value; }
        }
        private decimal _movementAmountOtherPayments;
        public decimal MovementAmountOtherPayments
        {
            get { return _movementAmountOtherPayments; }
            set { _movementAmountOtherPayments = value; }
        }
        private string _movementDescription;
        public string MovementDescription
        {
            get { return _movementDescription; }
            set { _movementDescription = value; }
        }
        private pos_worksessionmovementtype _selectedMovementType;
        public pos_worksessionmovementtype MovementType
        {
            get { return _selectedMovementType; }
            set { _selectedMovementType = value; }
        }

        public PosCashDrawerDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            //Disable WindowTitleCloseButton
            : base(pSourceWindow, pDialogFlags, true, false)
        {
            try
            {
                //Parameters
                _sourceWindow = pSourceWindow;

                //If has a valid open session
                if (GlobalFramework.WorkSessionPeriodTerminal != null)
                {
                    //Get From MoneyInCashDrawer, Includes CASHDRAWER_START and Money Movements
                    _totalAmountInCashDrawer = ProcessWorkSessionPeriod.GetSessionPeriodMovementTotal(GlobalFramework.WorkSessionPeriodTerminal, MovementTypeTotal.MoneyInCashDrawer);
                }
                //Dont have Open Terminal Session YET, use from last Closed CashDrawer
                else
                {
                    //Default Last Closed Cash Value
                    _totalAmountInCashDrawer = ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_CLOSE");
                }

                //Init Local Vars
                String windowTitle = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_cashdrawer"), FrameworkUtils.DecimalToStringCurrency(_totalAmountInCashDrawer));
                Size windowSize = new Size(462, 310);//400 With Other Payments
                String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_cash_drawer.png");
                String fileActionPrint = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_print.png");

                //Get SeletedData from WorkSessionMovementType Buttons
                string executeSql = @"SELECT Oid, Token, ResourceString, ButtonIcon, Disabled FROM pos_worksessionmovementtype WHERE (Token LIKE 'CASHDRAWER_%') AND (Disabled IS NULL or Disabled  <> 1) ORDER BY Ord;";
                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(executeSql);
                //Init Dictionary
                string buttonBagKey;
                bool buttonDisabled;
                Dictionary<string, TouchButtonIconWithText> buttonBag = new Dictionary<string, TouchButtonIconWithText>();
                TouchButtonIconWithText touchButtonIconWithText;
                HBox hboxCashDrawerButtons = new HBox(true, 5);
                bool buttonOkSensitive;

                //Generate Buttons
                foreach (SelectStatementResultRow row in xPSelectData.Data)
                {
                    buttonBagKey = row.Values[xPSelectData.GetFieldIndex("Token")].ToString();
                    buttonDisabled = Convert.ToBoolean(row.Values[xPSelectData.GetFieldIndex("Disabled")]);

                    touchButtonIconWithText = new TouchButtonIconWithText(
                      string.Format("touchButton{0}_Green", buttonBagKey),
                      Color.Transparent/*_colorBaseDialogDefaultButtonBackground*/,
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], row.Values[xPSelectData.GetFieldIndex("ResourceString")].ToString()),
                      _fontBaseDialogButton,
                      _colorBaseDialogDefaultButtonFont,
                     FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], row.Values[xPSelectData.GetFieldIndex("ButtonIcon")].ToString())),
                      _sizeBaseDialogDefaultButtonIcon,
                      _sizeBaseDialogDefaultButton.Width,
                      _sizeBaseDialogDefaultButton.Height
                     )
                    {
                        CurrentButtonOid = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString()),
                        Sensitive = !buttonDisabled
                    };
                    //Add to Dictionary
                    buttonBag.Add(buttonBagKey, touchButtonIconWithText);
                    //pack to VBhox
                    hboxCashDrawerButtons.PackStart(touchButtonIconWithText, true, true, 0);
                    //Events
                    buttonBag[buttonBagKey].Clicked += PosCashDrawerDialog_Clicked;
                }

                //Initial Button Status, Based on Open/Close Terminal Session
                string initialButtonToken;
                if (GlobalFramework.WorkSessionPeriodTerminal != null && GlobalFramework.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    buttonBag["CASHDRAWER_OPEN"].Sensitive = false;
                    buttonBag["CASHDRAWER_CLOSE"].Sensitive = true;
                    buttonBag["CASHDRAWER_IN"].Sensitive = true;
                    buttonBag["CASHDRAWER_OUT"].Sensitive = true;
                    initialButtonToken = "CASHDRAWER_CLOSE";
                    buttonOkSensitive = true;
                }
                else
                {
                    buttonBag["CASHDRAWER_OPEN"].Sensitive = true;
                    buttonBag["CASHDRAWER_CLOSE"].Sensitive = false;
                    buttonBag["CASHDRAWER_IN"].Sensitive = false;
                    buttonBag["CASHDRAWER_OUT"].Sensitive = false;
                    initialButtonToken = "CASHDRAWER_OPEN";
                    buttonOkSensitive = false;
                }
                //Initial Dialog Values
                _selectedCashDrawerButton = buttonBag[initialButtonToken];
                _selectedCashDrawerButton.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Utils.Lighten(_colorBaseDialogDefaultButtonBackground, 0.50f)));
                _selectedMovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), _selectedCashDrawerButton.CurrentButtonOid);
                _selectedMovementType.Token = initialButtonToken;

                //EntryAmountMoney
                _entryBoxMovementAmountMoney = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_money"), KeyboardMode.Money, SettingsApp.RegexDecimalGreaterEqualThanZero, true);
                _entryBoxMovementAmountMoney.EntryValidation.Changed += delegate { ValidateDialog(); };

                //TODO: Enable Other Payments
                //EntryAmountOtherPayments
                //_entryBoxMovementAmountOtherPayments = new EntryBox(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_other_payments, KeyboardModes.Money, regexDecimalGreaterThanZero, false);
                //_entryBoxMovementAmountOtherPayments.EntryValidation.Changed += delegate { ValidateDialog(); };

                //EntryDescription
                _entryBoxMovementDescription = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_description"), KeyboardMode.AlfaNumeric, SettingsApp.RegexAlfaNumericExtended, false);
                _entryBoxMovementDescription.EntryValidation.Changed += delegate { ValidateDialog(); };

                //VBox
                VBox vbox = new VBox(false, 0);
                vbox.PackStart(hboxCashDrawerButtons, false, false, 0);
                vbox.PackStart(_entryBoxMovementAmountMoney, false, false, 0);
                //vbox.PackStart(_entryBoxMovementAmountOtherPayments, false, false, 0);
                vbox.PackStart(_entryBoxMovementDescription, false, false, 0);

                //Init Content
                Fixed fixedContent = new Fixed();
                fixedContent.Put(vbox, 0, 0);

                //ActionArea Buttons
                _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
                _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
                _buttonPrint = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Print);
                _buttonOk.Sensitive = false;
                _buttonPrint.Sensitive = buttonOkSensitive;

                //ActionArea
                ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
                actionAreaButtons.Add(new ActionAreaButton(_buttonPrint, _responseTypePrint));
                actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
                actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

                //Call Activate Button Helper Method
                ActivateButton(buttonBag[initialButtonToken]);

                //Init Object
                this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
