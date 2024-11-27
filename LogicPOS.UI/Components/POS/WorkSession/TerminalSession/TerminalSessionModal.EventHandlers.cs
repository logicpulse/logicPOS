using Gtk;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
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

            if(_totalCashInrawer < amount)
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
            }
        }

        private void ProcessCashIn()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (WorkSessionService.AddCashDrawerInMovement(amount, description))
            {
                CustomAlerts.ShowOperationSucceededAlert(this);
            }
        }

        private void CloseSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (WorkSessionService.CloseTerminalSession(amount, description))
            {
                CustomAlerts.Information(this)
                         .WithMessageResource("dialog_message_worksession_terminal_close_successfully")
                         .ShowAlert();
            }
        }

        private void OpenSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (WorkSessionService.OpenTerminalSession(amount, description))
            {
                CustomAlerts.Information(this)
                             .WithMessageResource("dialog_message_cashdrawer_open_successfully")
                             .ShowAlert();
            }
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

            if(MovementType == WorkSessionMovementType.CashDrawerOpen || MovementType == WorkSessionMovementType.CashDrawerClose)
            {
                TxtAmount.Text = _totalCashInrawer.ToString();
            } else
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