using Gtk;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosCashDrawerDialog
    {
        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse == ResponseType.Ok)
            {
                MovementAmountMoney = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxMovementAmountMoney.EntryValidation.Text);
                MovementDescription = _entryBoxMovementDescription.EntryValidation.Text;

                decimal cashLastMovementTypeAmount;
                if (MovementType.Token == "CASHDRAWER_OPEN")
                {
                    cashLastMovementTypeAmount = WorkSessionProcessor.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_CLOSE");
                }
                else if (MovementType.Token == "CASHDRAWER_CLOSE")
                {
                    //Alteração no funcionamento do Inicio/fecho Sessão [IN:014330]
                    cashLastMovementTypeAmount = WorkSessionProcessor.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_OPEN");
                    //Keep Running            
                    //if (!IsCashDrawerAmountValid(cashLastMovementTypeAmount))
                    //{
                    //    this.Run();
                    //}
                    //else
                    //{

                    //}
                }
                else if (MovementType.Token == "CASHDRAWER_OUT")
                {
                    //Check if Value is Small than AmountInCashDrawer
                    if (MovementAmountMoney > TotalAmountInCashDrawer)
                    {
                        string movementAmountMoney = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(MovementAmountMoney, XPOSettings.ConfigurationSystemCurrency.Acronym);
                        string totalAmountInCashDrawer = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalAmountInCashDrawer, XPOSettings.ConfigurationSystemCurrency.Acronym);

                        CustomAlerts.Error(this)
                                    .WithSize(new Size(500, 350))
                                    .WithTitleResource("global_error")
                                    .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_cashdrawer_money_out_error"), movementAmountMoney, totalAmountInCashDrawer))
                                    .ShowAlert();


                        this.Run();
                    }
                }
            }
            else if (pResponse == _responseTypePrint)
            {
                var workSessionDto = MappingUtils.GetPrintWorkSessionDto(XPOSettings.WorkSessionPeriodTerminal);
                //tchial0: FrameworkCalls.PrintWorkSessionMovement(this, TerminalService.Terminal.ThermalPrinter,workSessionDto, TerminalService.Terminal.Designation);

                this.Run();
            }
        }

        private void PosCashDrawerDialog_Clicked(object sender, EventArgs e)
        {
            IconButtonWithText button = (IconButtonWithText)sender;
            ActivateButton(button);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void ActivateButton(IconButtonWithText pButton)
        {
            //If Changed Button, Clean old Values
            if (_selectedCashDrawerButton != pButton)
            {
                _entryBoxMovementAmountMoney.EntryValidation.Text = string.Empty;
                _entryBoxMovementDescription.EntryValidation.Text = string.Empty;
            }

            //Assign _selectedMovementType
            MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(pButton.CurrentButtonId);

            //Detect Cash open
            if (MovementType.Token == "CASHDRAWER_OPEN")
            {
                _entryBoxMovementAmountMoney.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(TotalAmountInCashDrawer);
                _entryBoxMovementAmountMoney.EntryValidation.Sensitive = true;
                //Required to disable keyboard button
                _entryBoxMovementAmountMoney.ButtonKeyBoard.Sensitive = true;
                _entryBoxMovementAmountMoney.EntryValidation.Required = false;
                _entryBoxMovementAmountMoney.EntryValidation.Rule = null;
            }
            else
            {
                _entryBoxMovementAmountMoney.EntryValidation.Required = true;
                _entryBoxMovementAmountMoney.EntryValidation.Rule = RegularExpressions.DecimalGreaterEqualThanZero;
                _entryBoxMovementAmountMoney.EntryValidation.Sensitive = true;
                //Required to enable keyboard button
                _entryBoxMovementAmountMoney.ButtonKeyBoard.Sensitive = true;
            }

            //Apply Requires Description for MONEY_IN and MONEY_OUT
            _entryBoxMovementDescription.EntryValidation.Required = (MovementType.Token == "CASHDRAWER_IN" || MovementType.Token == "CASHDRAWER_OUT");
            _entryBoxMovementDescription.EntryValidation.Validate();

            //Now we can UnToggle Old Selected Button
            if (_selectedCashDrawerButton != null)
            {
                //Toggle Button Off
                _selectedCashDrawerButton.ModifyBg(
                    StateType.Normal,
                    ColorSettings.DefaultButtonBackground.ToGdkColor());
            }

            //In the End Change reference to new Seleted Button
            _selectedCashDrawerButton = pButton;
            //Toggle Button On
            _selectedCashDrawerButton.ModifyBg(
                StateType.Normal,
                ColorSettings.DefaultButtonBackground.Lighten(0.50f).ToGdkColor());

            //Validate
            ValidateDialog();
        }

        private void ValidateDialog()
        {
            decimal entryValidation;
            if (_entryBoxMovementAmountMoney.EntryValidation.Text != string.Empty)
                entryValidation = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxMovementAmountMoney.EntryValidation.Text);

            //Validate Selected Entities and Change Value
            if (_buttonOk != null)
                _buttonOk.Sensitive =
                  (
                    _selectedCashDrawerButton != null &&
                    (
                      //Validated or Zero
                      (_entryBoxMovementAmountMoney.EntryValidation.Validated)
                      &&
                      //TODO: Enable Other Payments
                      //_entryBoxMovementAmountOtherPayments.EntryValidation.Validated &&
                      _entryBoxMovementDescription.EntryValidation.Validated
                    )
                    //One of them must be filled
                    &&
                    (
                      _entryBoxMovementAmountMoney.EntryValidation.Text != string.Empty //||
                                                                                        //TODO: Enable Other Payments
                                                                                        //_entryBoxMovementAmountOtherPayments.EntryValidation.Text != string.Empty
                    )
                  );
        }
    }
}