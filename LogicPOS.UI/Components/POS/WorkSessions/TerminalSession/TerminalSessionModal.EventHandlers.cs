using Gtk;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class TerminalSessionModal
    {
        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Cancel && response != ResponseType.Ok)
            {
                Run();
                return;
            }

            base.OnResponse(response);
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnPrint.Clicked += BtnPrint_Clicked;
            BtnOpening.Clicked += BtnOpening_Clicked;
            BtnClosing.Clicked += BtnClosing_Clicked;
            BtnIn.Clicked += BtnIn_Clicked;
            BtnOut.Clicked += BtnOut_Clicked;
            _movementTypeButtons.ForEach(btn => btn.Clicked += MovementTypeButton_Clicked);
        }

        private void BtnOut_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerOut;
        }

        private void BtnIn_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerIn;
        }

        private void BtnClosing_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerClose;
        }

        private void BtnOpening_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerOpen;

        }

        private void BtnPrint_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }

            switch (MovementType)
            {
                case WorkSessionMovementType.CashDrawerOpen:
                    OpenSession();
                    break;
                case WorkSessionMovementType.CashDrawerClose:
                    CloseSession();
                    break;
                case WorkSessionMovementType.CashDrawerIn:
                    ProcessCashIn();
                    break;
                case WorkSessionMovementType.CashDrawerOut:
                    ProcessCashOut();
                    break;
            }
        }

        private void ProcessCashOut()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (_totalCashInrawer < amount)
            {
                CustomAlerts.Error(this)
                             .WithMessage(string.Format(LocalizedString.Instance["dialog_message_cashdrawer_money_out_error"],
                                                        amount.ToString("C"),
                                                        _totalCashInrawer.ToString("C")))
                             .ShowAlert();
                Run();
                return;
            }

            if (WorkSessionService.AddCashDrawerOutMovement(amount, description))
            {
                CustomAlerts.ShowOperationSucceededAlert(this);

                var response= CustomAlerts.Question(this)
                                            .WithSize(new Size(620, 300))
                                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_print_document_confirmation"))
                                            .ShowAlert();

                if (response == ResponseType.Yes)
                {
                    var totalInCashDrawer = WorkSessionService.GetTotalCashInCashDrawer();
                    ThermalPrintingService.PrintCashDrawerOutMovement(totalInCashDrawer, amount, description);
                }
            }
        }

        private void ProcessCashIn()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (WorkSessionService.AddCashDrawerInMovement(amount, description))
            {
                CustomAlerts.ShowOperationSucceededAlert(this);
                var response = CustomAlerts.Question(this)
                                            .WithSize(new Size(620, 300))
                                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_print_document_confirmation"))
                                            .ShowAlert();

                if (response == ResponseType.Yes)
                {
                    var totalInCashDrawer = WorkSessionService.GetTotalCashInCashDrawer();
                    ThermalPrintingService.PrintCashDrawerInMovement(totalInCashDrawer, amount, description);
                }
            }
        }

        private void CloseSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (!WorkSessionService.CloseTerminalSession(amount, description))
            {
                return;
            }

            CustomAlerts.Information(this)
                     .WithMessageResource("dialog_message_worksession_terminal_close_successfully")
                     .ShowAlert();

            POSWindow.Instance.UpdateUI();
        }

        private void OpenSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;
            var totalInCashDrawer = WorkSessionService.GetTotalCashInCashDrawer();

            if (!WorkSessionService.OpenTerminalSession(amount, description))
            {
                return;
            }

            CustomAlerts.Information(this)
                         .WithMessageResource("dialog_message_cashdrawer_open_successfully")
                         .ShowAlert();

            var response = CustomAlerts.Question(this)
                                       .WithSize(new Size(620, 300))
                                       .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_print_document_confirmation"))
                                       .ShowAlert();

            if (response == ResponseType.Yes)
            {
                ThermalPrintingService.PrintCashDrawerOpen(totalInCashDrawer, amount, description);
                var newTotalInCashDrawer = WorkSessionService.GetTotalCashInCashDrawer();
                if (newTotalInCashDrawer > totalInCashDrawer)
                {
                    ThermalPrintingService.PrintCashDrawerInMovement(newTotalInCashDrawer, amount - totalInCashDrawer, description);
                }
                if (newTotalInCashDrawer<totalInCashDrawer)
                {
                    ThermalPrintingService.PrintCashDrawerOutMovement(newTotalInCashDrawer, totalInCashDrawer-newTotalInCashDrawer , description);
                }
            }

            POSWindow.Instance.UpdateUI();
        }

        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            yield return TxtAmount;
            if (MovementType == WorkSessionMovementType.CashDrawerIn || MovementType == WorkSessionMovementType.CashDrawerOut)
            {
                yield return TxtDescription;
            }
        }

        private void UpdateValidatableFields()
        {
            TxtDescription.IsRequired = MovementType == WorkSessionMovementType.CashDrawerIn || MovementType == WorkSessionMovementType.CashDrawerOut;
            TxtDescription.UpdateValidationColors();
        }

        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void MovementTypeButton_Clicked(object sender, EventArgs e)
        {
            UpdateButtonsColors();
            UpdateValidatableFields();

            if (MovementType == WorkSessionMovementType.CashDrawerOpen || MovementType == WorkSessionMovementType.CashDrawerClose)
            {
                TxtAmount.Text = _totalCashInrawer.ToString();
            }
            else
            {
                TxtAmount.Text = string.Empty;
            }
        }

        private void UpdateButtonsColors()
        {
            _movementTypeButtons.ForEach(btn => btn.ModifyBg(StateType.Normal,
                                                 AppSettings.Instance.colorBaseDialogDefaultButtonBackground.ToGdkColor()));

            switch (MovementType)
            {
                case WorkSessionMovementType.CashDrawerOpen:
                    BtnOpening.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerClose:
                    BtnClosing.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerIn:
                    BtnIn.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerOut:
                    BtnOut.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
            }
        }

        private void UpdateButtonsSensitivity()
        {
            BtnOpening.Sensitive = !_cashDrawerIsOpen;
            BtnClosing.Sensitive = _cashDrawerIsOpen;
            BtnIn.Sensitive = _cashDrawerIsOpen;
            BtnOut.Sensitive = _cashDrawerIsOpen;
        }
    }
}