﻿using Gtk;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
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

                        logicpos.Utils.ShowMessageBox(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"),
                            string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_cashdrawer_money_out_error"), movementAmountMoney, totalAmountInCashDrawer)
                        );
                        //Keep Running            
                        this.Run();
                    }
                }
            }
            else if (pResponse == _responseTypePrint)
            {
                //Uncomment to Pront Session Day
                //PrintTicket.PrintWorkSessionMovementInit(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);

                //PrintWorkSessionMovement
                //PrintRouter.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                var workSessionDto = MappingUtils.GetPrintWorkSessionDto(XPOSettings.WorkSessionPeriodTerminal);
                FrameworkCalls.PrintWorkSessionMovement(this, TerminalSettings.LoggedTerminal.ThermalPrinter,workSessionDto);

                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                //(_sourceWindow as PosCashDialog).ShowClosePeriodMessage(GlobalFramework.WorkSessionPeriodDay);
                //(_sourceWindow as PosCashDialog).ShowClosePeriodMessage(GlobalFramework.WorkSessionPeriodTerminal);

                //TEST FROM PERSISTED GUID, PAST RESUMES - DEBUG ONLY
                //WorkSessionPeriod workSessionPeriodDay;
                //WorkSessionPeriod workSessionPeriodTerminal;

                //#0 - Day 1
                //workSessionPeriodDay = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("90a187b3-c91b-4c5b-907a-d54a6ee1dcb6"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodDay);
                //#8 - Day 2
                //workSessionPeriodDay = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("5ce65097-55a2-4a6c-9406-aabe9f3f0124"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodDay);

                //#1 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("12d74d99-9734-4adb-b322-82f337e24d3e"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#2 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("67758bb2-c52a-4c05-8e10-37f63f729ce4"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#3 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("f43ae288-3615-44c0-b876-4fcac01efd1e"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#4 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("8b261a90-c15d-4e54-a013-c85467338224"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#5 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("13816f1f-4dd5-4351-afe4-c492f61cacb1"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#6 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("e5698d06-5740-4317-b7c7-d3eb92063b37"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#7 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("734c8ed3-34f9-4096-8c20-de9110a24817"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#9 - Day2 - Terminal #10
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("f445c36c-3ebd-46f1-bcbd-d158e497eda9"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#10 - Day2 - Terminal #20
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("74fd498a-c1a7-46e6-a117-14eea795e93d"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#11 - Day2 - Terminal #30
                //workSessionPeriodTerminal = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("14631cda-f31a-4e7a-8a75-a3ba2955ccf8"));
                //PrintTicket.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, workSessionPeriodTerminal);     

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
                _entryBoxMovementAmountMoney.EntryValidation.Rule = LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero;
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