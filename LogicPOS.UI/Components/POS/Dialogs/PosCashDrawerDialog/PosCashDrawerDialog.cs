using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosCashDrawerDialog : BaseDialog
    {
        //Settings
        private readonly int _decimalRoundTo = CultureSettings.DecimalRoundTo;
        //Private Properties
        //ResponseType (Above 10)
        private readonly ResponseType _responseTypePrint = (ResponseType)11;
        //Buttons
        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly IconButtonWithText _buttonPrint;
        private CustomButton _selectedCashDrawerButton;
        //UI
        private readonly EntryBoxValidation _entryBoxMovementAmountMoney;
        private readonly EntryBoxValidation _entryBoxMovementDescription;

        public decimal TotalAmountInCashDrawer { get; set; }
        public decimal MovementAmountMoney { get; set; }

        public decimal MovementAmountOtherPayments { get; set; }
        public string MovementDescription { get; set; }

        public pos_worksessionmovementtype MovementType { get; set; }

        public PosCashDrawerDialog(Window parentWindow, DialogFlags pDialogFlags)
            //Disable WindowTitleCloseButton
            : base(parentWindow, pDialogFlags, true, false)
        {
            //Parameters
            WindowSettings.Source = parentWindow;

            //If has a valid open session
            if (XPOSettings.WorkSessionPeriodTerminal != null)
            {
                //Alteração no funcionamento do Inicio/fecho Sessão [IN:014330]
                //Get From MoneyInCashDrawer, Includes CASHDRAWER_START and Money Movements
                TotalAmountInCashDrawer = WorkSessionProcessor.GetSessionPeriodMovementTotal(XPOSettings.WorkSessionPeriodTerminal, MovementTypeTotal.MoneyInCashDrawer);
                if (TotalAmountInCashDrawer < 0) TotalAmountInCashDrawer = TotalAmountInCashDrawer * (-1);
            }
            //Dont have Open Terminal Session YET, use from last Closed CashDrawer
            else
            {
                //Default Last Closed Cash Value
                TotalAmountInCashDrawer = WorkSessionProcessor.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_CLOSE");
                if (TotalAmountInCashDrawer < 0) TotalAmountInCashDrawer = TotalAmountInCashDrawer * (-1);
            }

            //Init Local Vars
            string windowTitle = string.Format(GeneralUtils.GetResourceByName("window_title_dialog_cashdrawer"), DataConversionUtils.DecimalToStringCurrency(TotalAmountInCashDrawer, XPOSettings.ConfigurationSystemCurrency.Acronym));
            Size windowSize = new Size(462, 310);//400 With Other Payments
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_cash_drawer.png";
            string fileActionPrint = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_print.png";

            //Get SeletedData from WorkSessionMovementType Buttons
            string executeSql = @"SELECT Oid, Token, ResourceString, ButtonIcon, Disabled FROM pos_worksessionmovementtype WHERE (Token LIKE 'CASHDRAWER_%') AND (Disabled IS NULL or Disabled  <> 1) ORDER BY Ord;";
            SQLSelectResultData xPSelectData = XPOUtility.GetSelectedDataFromQuery(executeSql);
            //Init Dictionary
            string buttonBagKey;
            bool buttonDisabled;
            Dictionary<string, IconButtonWithText> buttonBag = new Dictionary<string, IconButtonWithText>();
            IconButtonWithText touchButtonIconWithText;
            HBox hboxCashDrawerButtons = new HBox(true, 5);
            bool buttonOkSensitive;

            //Generate Buttons
            foreach (SelectStatementResultRow row in xPSelectData.DataRows)
            {
                buttonBagKey = row.Values[xPSelectData.GetFieldIndexFromName("Token")].ToString();
                buttonDisabled = Convert.ToBoolean(row.Values[xPSelectData.GetFieldIndexFromName("Disabled")]);

                touchButtonIconWithText = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = string.Format("touchButton{0}_Green", buttonBagKey),
                        Text = LocalizedString.Instance[row.Values[xPSelectData.GetFieldIndexFromName("ResourceString")].ToString()],
                        Font = FontSettings.Button,
                        FontColor = ColorSettings.DefaultButtonFont,
                        Icon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, row.Values[xPSelectData.GetFieldIndexFromName("ButtonIcon")].ToString()),
                        IconSize = SizeSettings.DefaultButtonIcon,
                        ButtonSize = SizeSettings.DefaultButton
                    })
                {
                    CurrentButtonId = new Guid(row.Values[xPSelectData.GetFieldIndexFromName("Oid")].ToString()),
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
            if (XPOSettings.WorkSessionPeriodTerminal != null && XPOSettings.WorkSessionPeriodTerminal.SessionStatus == WorkSessionPeriodStatus.Open)
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
            _selectedCashDrawerButton.ModifyBg(StateType.Normal, ColorSettings.DefaultButtonBackground.Lighten(0.50f).ToGdkColor());
            MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(_selectedCashDrawerButton.CurrentButtonId);
            MovementType.Token = initialButtonToken;

            //EntryAmountMoney
            _entryBoxMovementAmountMoney = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_money"), KeyboardMode.Money, RegularExpressions.DecimalGreaterEqualThanZero, true);
            _entryBoxMovementAmountMoney.EntryValidation.Changed += delegate { ValidateDialog(); };
            //Alteração no funcionamento do Inicio/fecho Sessão [IN:014330]
            _entryBoxMovementAmountMoney.EntryValidation.Text = DataConversionUtils.DecimalToString(TotalAmountInCashDrawer);

            //TODO: Enable Other Payments
            //EntryAmountOtherPayments
            //_entryBoxMovementAmountOtherPayments = new EntryBox(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_other_payments, KeyboardModes.Money, regexDecimalGreaterThanZero, false);
            //_entryBoxMovementAmountOtherPayments.EntryValidation.Changed += delegate { ValidateDialog(); };

            //EntryDescription
            _entryBoxMovementDescription = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_description"), KeyboardMode.AlfaNumeric, RegularExpressions.AlfaNumericExtended, false);
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
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            _buttonPrint = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Print);
            _buttonOk.Sensitive = false;
            _buttonPrint.Sensitive = buttonOkSensitive;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
                {
                    new ActionAreaButton(_buttonPrint, _responseTypePrint),
                    new ActionAreaButton(_buttonOk, ResponseType.Ok),
                    new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
                };

            //Call Activate Button Helper Method
            ActivateButton(buttonBag[initialButtonToken]);

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);

        }
    }
}
