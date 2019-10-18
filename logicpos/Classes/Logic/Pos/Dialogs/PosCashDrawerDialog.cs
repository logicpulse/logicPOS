using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosCashDrawerDialog
    {
        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse == ResponseType.Ok)
            {
                _movementAmountMoney = FrameworkUtils.StringToDecimal(_entryBoxMovementAmountMoney.EntryValidation.Text);
                _movementDescription = _entryBoxMovementDescription.EntryValidation.Text;

                decimal cashLastMovementTypeAmount = 0.0m;

                if (_selectedMovementType.Token == "CASHDRAWER_OPEN")
                {
                    cashLastMovementTypeAmount = ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_CLOSE");
                }
                else if (_selectedMovementType.Token == "CASHDRAWER_CLOSE")
                {
                    cashLastMovementTypeAmount = ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount("CASHDRAWER_OPEN");
                    //Keep Running            
                    if (!IsCashDrawerAmountValid(cashLastMovementTypeAmount))
                    {
                        this.Run();
                    }
                    else
                    {
                        pResponse = Utils.ShowMessageTouch(
                      this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print"),
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_request_print_document_confirmation"));

                        if (pResponse == ResponseType.Yes)
                        {
                            FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, GlobalFramework.WorkSessionPeriodTerminal);
                        }
                    }
                }
                else if (_selectedMovementType.Token == "CASHDRAWER_OUT")
                {
                    //Check if Value is Small than AmountInCashDrawer
                    if (_movementAmountMoney > _totalAmountInCashDrawer)
                    {
                        string movementAmountMoney = FrameworkUtils.DecimalToStringCurrency(_movementAmountMoney);
                        string totalAmountInCashDrawer = FrameworkUtils.DecimalToStringCurrency(_totalAmountInCashDrawer);
                        
                        Utils.ShowMessageTouch(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"),
                            string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_cashdrawer_money_out_error"), movementAmountMoney, totalAmountInCashDrawer)
                        );
                        //Keep Running            
                        this.Run();
                    }
                }
            }
            else if (pResponse == _responseTypePrint)
            {
                //Uncomment to Pront Session Day
                //PrintTicket.PrintWorkSessionMovementInit(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);

                //PrintWorkSessionMovement
                //PrintRouter.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, GlobalFramework.WorkSessionPeriodTerminal);

                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                //(_sourceWindow as PosCashDialog).ShowClosePeriodMessage(GlobalFramework.WorkSessionPeriodDay);
                //(_sourceWindow as PosCashDialog).ShowClosePeriodMessage(GlobalFramework.WorkSessionPeriodTerminal);

                //TEST FROM PERSISTED GUID, PAST RESUMES - DEBUG ONLY
                //WorkSessionPeriod workSessionPeriodDay;
                //WorkSessionPeriod workSessionPeriodTerminal;

                //#0 - Day 1
                //workSessionPeriodDay = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("90a187b3-c91b-4c5b-907a-d54a6ee1dcb6"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodDay);
                //#8 - Day 2
                //workSessionPeriodDay = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("5ce65097-55a2-4a6c-9406-aabe9f3f0124"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodDay);

                //#1 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("12d74d99-9734-4adb-b322-82f337e24d3e"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#2 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("67758bb2-c52a-4c05-8e10-37f63f729ce4"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#3 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("f43ae288-3615-44c0-b876-4fcac01efd1e"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#4 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("8b261a90-c15d-4e54-a013-c85467338224"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#5 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("13816f1f-4dd5-4351-afe4-c492f61cacb1"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#6 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("e5698d06-5740-4317-b7c7-d3eb92063b37"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);
                //#7 - Day1
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("734c8ed3-34f9-4096-8c20-de9110a24817"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#9 - Day2 - Terminal #10
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("f445c36c-3ebd-46f1-bcbd-d158e497eda9"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#10 - Day2 - Terminal #20
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("74fd498a-c1a7-46e6-a117-14eea795e93d"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);     
                //#11 - Day2 - Terminal #30
                //workSessionPeriodTerminal = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(typeof(WorkSessionPeriod), new Guid("14631cda-f31a-4e7a-8a75-a3ba2955ccf8"));
                //PrintTicket.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, workSessionPeriodTerminal);     

                this.Run();
            }
        }

        void PosCashDrawerDialog_Clicked(object sender, EventArgs e)
        {
            TouchButtonIconWithText button = (TouchButtonIconWithText)sender;
            ActivateButton(button);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void ActivateButton(TouchButtonIconWithText pButton)
        {
            //If Changed Button, Clean old Values
            if (_selectedCashDrawerButton != pButton)
            {
                _entryBoxMovementAmountMoney.EntryValidation.Text = string.Empty;
                _entryBoxMovementDescription.EntryValidation.Text = string.Empty;
            }

            //Assign _selectedMovementType
            _selectedMovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), pButton.CurrentButtonOid);

            //Detect Cash open
            if (_selectedMovementType.Token == "CASHDRAWER_OPEN")
            {
                _entryBoxMovementAmountMoney.EntryValidation.Text = FrameworkUtils.DecimalToString(_totalAmountInCashDrawer);
                _entryBoxMovementAmountMoney.EntryValidation.Sensitive = false;
                //Required to disable keyboard button
                _entryBoxMovementAmountMoney.ButtonKeyBoard.Sensitive = false;
                _entryBoxMovementAmountMoney.EntryValidation.Required = false;
                _entryBoxMovementAmountMoney.EntryValidation.Rule = null;
            }
            else
            {
                _entryBoxMovementAmountMoney.EntryValidation.Required = true;
                _entryBoxMovementAmountMoney.EntryValidation.Rule = SettingsApp.RegexDecimalGreaterEqualThanZero;
                _entryBoxMovementAmountMoney.EntryValidation.Sensitive = true;
                //Required to enable keyboard button
                _entryBoxMovementAmountMoney.ButtonKeyBoard.Sensitive = true;
            }

            //Apply Requires Description for MONEY_IN and MONEY_OUT
            _entryBoxMovementDescription.EntryValidation.Required = (_selectedMovementType.Token == "CASHDRAWER_IN" || _selectedMovementType.Token == "CASHDRAWER_OUT");
            _entryBoxMovementDescription.EntryValidation.Validate();

            //Now we can UnToggle Old Selected Button
            if (_selectedCashDrawerButton != null)
            {
                //Toggle Button Off
                _selectedCashDrawerButton.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(_colorBaseDialogDefaultButtonBackground));
            }

            //In the End Change reference to new Seleted Button
            _selectedCashDrawerButton = pButton;
            //Toggle Button On
            _selectedCashDrawerButton.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Utils.Lighten(_colorBaseDialogDefaultButtonBackground, 0.50f)));

            //Validate
            ValidateDialog();
        }

        private void ValidateDialog()
        {
            decimal entryValidation = -1;
            if (_entryBoxMovementAmountMoney.EntryValidation.Text != string.Empty)
                entryValidation = FrameworkUtils.StringToDecimal(_entryBoxMovementAmountMoney.EntryValidation.Text);

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

        /// <summary>
        /// Check if Open and Close CashDrawer Amount is Valid, Totals are Equal
        /// </summary>
        /// <param name="pLastMovementTypeAmount"></param>
        /// <returns></returns>
        private Boolean IsCashDrawerAmountValid(decimal pLastMovementTypeAmount)
        {
            decimal totalInCashDrawer;

            //With Drawer Opened
            if (GlobalFramework.WorkSessionPeriodTerminal != null)
            {
                decimal moneyCashTotalMovements = ProcessWorkSessionPeriod.GetSessionPeriodMovementTotal(GlobalFramework.WorkSessionPeriodTerminal, MovementTypeTotal.MoneyInCashDrawer);
                totalInCashDrawer = Math.Round(moneyCashTotalMovements, _decimalRoundTo);
            }
            //With Drawer Closed
            else
            {
                totalInCashDrawer = Math.Round(pLastMovementTypeAmount, _decimalRoundTo);
            }

            decimal diference = Math.Round(_movementAmountMoney - totalInCashDrawer, _decimalRoundTo);
            string message = string.Empty;
            //_log.Debug(string.Format("_movementAmountMoney: [{0}], pLastMovementTypeAmount:[{1}], totalInCashDrawer: [{2}], diference: [{3}]", _movementAmountMoney, pLastMovementTypeAmount, totalInCashDrawer, diference));

            if (diference != 0)
            {
                message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_cashdrawer_open_close_total_enter_diferent_from_total_in_cashdrawer")
                  , FrameworkUtils.DecimalToStringCurrency(totalInCashDrawer)
                  , FrameworkUtils.DecimalToStringCurrency(_movementAmountMoney)
                  , FrameworkUtils.DecimalToStringCurrency(diference)
                );
                Utils.ShowMessageTouch(this, DialogFlags.Modal, new Size(600, 450), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), message);

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}