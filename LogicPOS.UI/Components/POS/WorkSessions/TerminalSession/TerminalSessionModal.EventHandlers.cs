using Gtk;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
            UpdateBtnPrint();
        }

        private void BtnOut_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerOut;
            UpdateBtnPrint();
        }

        private void BtnIn_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerIn;
            UpdateBtnPrint();
        }

        private void BtnClosing_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerClose;
            UpdateBtnPrint();
        }

        private void BtnOpening_Clicked(object sender, EventArgs e)
        {
            MovementType = WorkSessionMovementType.CashDrawerOpen;
            UpdateBtnPrint();
        }

        private void BtnPrint_Clicked(object sender, EventArgs e)
        {
            var reportData = WorkSessionsService.GetLastClosedDayReportData();

            if (reportData == null)
            {
                CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithMessage("Não foi possível obter os dados do relatório do último dia fechado.")
                            .ShowAlert();
                return;
            }

            ThermalPrintingService.PrintWorkSessionReport(reportData);
        }

        private void UpdateBtnPrint()
        {
            BtnPrint.Sensitive = WorkSessionsService.DayIsOpen() || WorkSessionsService.TerminalIsOpen();
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

            if (WorkSessionsService.AddCashDrawerOutMovement(amount, description))
            {
                CustomAlerts.ShowOperationSucceededAlert(this);

                var response = CustomAlerts.Question(this)
                                            .WithSize(new Size(620, 300))
                                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_print_document_confirmation"))
                                            .ShowAlert();

                if (response == ResponseType.Yes)
                {
                    var totalInCashDrawer = WorkSessionsService.GetTotalCashInCashDrawer();
                    ThermalPrintingService.PrintCashDrawerOutMovement(totalInCashDrawer, amount, description);
                }
                AuthenticationService.HardwareOpenDrawer();
            }
        }

        private void ProcessCashIn()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;

            if (WorkSessionsService.AddCashDrawerInMovement(amount, description))
            {
                CustomAlerts.ShowOperationSucceededAlert(this);
                var response = CustomAlerts.Question(this)
                                            .WithSize(new Size(620, 300))
                                            .WithMessage(GeneralUtils.GetResourceByName("dialog_message_request_print_document_confirmation"))
                                            .ShowAlert();

                if (response == ResponseType.Yes)
                {
                    var totalInCashDrawer = WorkSessionsService.GetTotalCashInCashDrawer();
                    ThermalPrintingService.PrintCashDrawerInMovement(totalInCashDrawer, amount, description);
                }

            }
        }

        private void CloseSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;
            var totalInCashDrawer = WorkSessionsService.GetTotalCashInCashDrawer();

            if (!WorkSessionsService.CloseTerminalSession(amount, description))
            {
                return;
            }

            ShowTerminalSessionEndInformation();
            HandleTerminalSessionReportPrinting();
            
            POSWindow.Instance.UpdateUI();
        }



        private void OpenSession()
        {
            var amount = decimal.Parse(TxtAmount.Text);
            var description = TxtDescription.Text;
            var totalInCashDrawer = WorkSessionsService.GetTotalCashInCashDrawer();

            if (!WorkSessionsService.OpenTerminalSession(amount, description))
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
                var newTotalInCashDrawer = WorkSessionsService.GetTotalCashInCashDrawer();
                if (newTotalInCashDrawer > totalInCashDrawer)
                {
                    ThermalPrintingService.PrintCashDrawerInMovement(newTotalInCashDrawer, amount - totalInCashDrawer, description);
                }
                if (newTotalInCashDrawer < totalInCashDrawer)
                {
                    ThermalPrintingService.PrintCashDrawerOutMovement(newTotalInCashDrawer, totalInCashDrawer - newTotalInCashDrawer, description);
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
                TxtAmount.Text = _totalCashInrawer.ToString("0.00");
            }
            else
            {
                TxtAmount.Text = string.Empty;
            }
        }

        private void UpdateButtonsColors()
        {
            _movementTypeButtons.ForEach(btn => btn.ModifyBg(StateType.Normal,
                                                 AppSettings.Instance.ColorBaseDialogDefaultButtonBackground.ToGdkColor()));

            switch (MovementType)
            {
                case WorkSessionMovementType.CashDrawerOpen:
                    BtnOpening.ModifyBg(StateType.Normal, AppSettings.Instance.ColorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerClose:
                    BtnClosing.ModifyBg(StateType.Normal, AppSettings.Instance.ColorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerIn:
                    BtnIn.ModifyBg(StateType.Normal, AppSettings.Instance.ColorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
                case WorkSessionMovementType.CashDrawerOut:
                    BtnOut.ModifyBg(StateType.Normal, AppSettings.Instance.ColorBaseDialogDefaultButtonBackground.Lighten(0.50f).ToGdkColor());
                    break;
            }
        }

        private void ShowTerminalSessionEndInformation()
        {
            DayReportData reportData = WorkSessionsService.GetLastClosedTerminalSessionReportData();

            var message = new StringBuilder($"Sessão de terminal fechada com sucesso!\n\n");


            if (reportData != null)
            {
                message.AppendLine($"Abertura: {reportData.Day.StartDate}");
                message.AppendLine($"Fecho: {reportData.Day.EndDate}");
                message.AppendLine($"Total abertura: {reportData.OpeningCashTotal:N2}");
                message.AppendLine($"Total em Caixa: {reportData.EndOfDayCashTotal:N2}");
                message.AppendLine($"Entrada de Numerário: {reportData.CashDrawerIn:N2}");
                message.AppendLine($"Saída de Numerário: {reportData.CashDrawerOut:N2}\n");

                message.AppendLine();
                message.AppendLine($"# Total por método de pagamento (Qnt.)");
                foreach (var payment in reportData.GetTotalPerPaymentMethod())
                {
                    message.AppendLine($"{payment.Method}: ({payment.Quantity:F2}) {payment.Total:F2}");
                }

                message.AppendLine();
                message.AppendLine($"# Total por família (Qnt.)");
                foreach (var family in reportData.GetTotalPerFamily())
                {
                    message.AppendLine($"{family.Family}: ({family.Quantity:F2}) {family.Total:N2}");
                }


                message.AppendLine();
                message.AppendLine($"# Total por subfamília (Qnt.)");
                foreach (var subfamily in reportData.GetTotalPerSubfamily())
                {
                    message.AppendLine($"{subfamily.Subfamily}: ({subfamily.Quantity:F2}) {subfamily.Total:N2}");
                }


                message.AppendLine();
                message.AppendLine($"# Total por artigo (Qnt.) ");
                foreach (var article in reportData.GetTotalPerArticle())
                {
                    message.AppendLine($"{article.Article}: ({article.Quantity:F2}) {article.Total:N2}");
                }


                message.AppendLine();
                message.AppendLine($"# Total por taxa (Qnt.)");
                foreach (var tax in reportData.GetTotalPerTax())
                {
                    message.AppendLine($"{tax.Tax}: ({tax.Quantity:F2}) {tax.Total:N2}");
                }

                message.AppendLine();
                message.AppendLine($"# Total por utilizador (Qnt.)");
                foreach (var user in reportData.GetTotalPerUser())
                {
                    message.AppendLine($"{user.User}: ({user.Quantity:F2}) {user.Total:N2}");
                }

                message.AppendLine();
                message.AppendLine("# Total por documento (Qnt.)");
                foreach (var document in reportData.GetTotalPerDocumentType())
                {
                    message.AppendLine($"{document.DocumentType}: ({document.Quantity:F2}) {document.Total:N2}");
                }

                message.AppendLine();
                message.AppendLine("# Total por hora (Qnt.)");
                foreach (var hour in reportData.GetTotalPerHour())
                {
                    message.AppendLine($"{hour.Hour}: ({hour.Quantity:F2}) {hour.Total:N2}");
                }

                message.AppendLine();
                message.AppendLine($"## Total Ilí.: {reportData.DocumentsTotal:N2}");
            }


            CustomAlerts.Information(this)
                        .WithMessage(message.ToString())
                        .ShowAlert();

        }

        private bool HandleTerminalSessionReportPrinting()
        {
            var printDialogResponse = CustomAlerts.Question(this)
                                                                .WithSize(new Size(500, 350))
                                                                .WithTitleResource("global_button_label_print")
                                                                .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                                .ShowAlert();

            if (printDialogResponse != ResponseType.Yes)
            {
                return false;
            }

            var reportData = WorkSessionsService.GetLastClosedTerminalSessionReportData();

            if (reportData == null)
            {
                CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithMessage("Não foi possível obter os dados do relatório da sessão.")
                            .ShowAlert();
                return false;
            }

            ThermalPrintingService.PrintWorkSessionReport(reportData);
            return true;
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