using Gtk;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.Api.Features.WorkSessions.GetLastClosedDay;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Printing.Tickets;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LogicPOS.UI.Components.POS
{
    public partial class SessionOpeningModal
    {
        private void BtnDayOpening_Clicked(object sender, EventArgs e)
        {
            if (WorkSessionsService.DayIsOpen())
            {
                CloseDay();
                return;
            }

            OpenDay();
        }

        private bool HasOpenTables()
        {
            var openTables = TablesService.GetOpenTables();

            if (openTables.Any())
            {
                string tablesNames = string.Join(" ", openTables.Select(x => x.Designation));

                CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_worksession_period_warning_open_orders_tables"),
                                          openTables.Count(),
                                          $"\n{tablesNames}"))
                            .ShowAlert();
                return true;
            }

            return false;
        }

        private bool CheckOpenTerminals()
        {
            var openTerminalSessions = WorkSessionsService.GetOpenTerminalSessions();

            if (openTerminalSessions.Any())
            {
                string openTerminalsNames = string.Join(" ", openTerminalSessions.Select(x => x.Designation));

                ResponseType alertResponse = CustomAlerts.Question(this)
                                                         .WithSize(new Size(600, 400))
                                                         .WithTitleResource("global_information")
                                                         .WithMessage(string.Format(LocalizedString.Instance["dialog_message_worksession_period_warning_open_terminals"],
                                                                                    openTerminalSessions.Count(),
                                                                                    $"\n{openTerminalsNames}"))
                                                         .ShowAlert();

                if (alertResponse != ResponseType.Yes)
                {
                    return false;
                }

                WorkSessionsService.CloseAllTerminalSessions();
            }

            return true;
        }

        private void HandleBackup()
        {
            var responseType = new CustomAlert(this)
                .WithMessageResource("dialog_message_request_backup")
                .WithMessageType(MessageType.Question)
                .WithButtonsType(ButtonsType.YesNo)
                .WithTitleResource("global_information")
                .ShowAlert();

        }

        private void CloseDay()
        {
            if (AuthenticationService.User.Profile.Order != 40)
            {
                SimpleAlerts.Information()
                            .WithMessage("Usuário sem permissão para Fechar o Dia")
                            .ShowAlert();
                return;
            }
            if (HasOpenTables())
            {
                return;
            }

            if (CheckOpenTerminals() == false)
            {
                return;
            }

            HandleBackup();

            if (!WorkSessionsService.CloseDay())
            {
                return;
            }

            ShowDayEndInformation();

            HandleDayReportPrinting();

            UpdateUI();
            POSWindow.Instance.UpdateUI();
        }

        private void ShowDayEndInformation()
        {
            DayReportData reportData = WorkSessionsService.GetLastClosedDayReportData();

            var message = new StringBuilder($"Dia de trabalho  fechado com sucesso!\n\n");


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

        private void HandleDayReportPrinting()
        {
            var printDialogResponse = CustomAlerts.Question(this)
                                                                .WithSize(new Size(500, 350))
                                                                .WithTitleResource("global_button_label_print")
                                                                .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                                .ShowAlert();

            if (printDialogResponse != ResponseType.Yes)
            {
                return;
            }

            var reportData = WorkSessionsService.GetLastClosedDayReportData();

            if (reportData == null)
            {
                CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithMessage("Não foi possível obter os dados do relatório do dia.")
                            .ShowAlert();
                return;
            }

            ThermalPrintingService.PrintWorkSessionReport(reportData);
        }

        private void AddEventHandlers()
        {
            BtnDayOpening.Clicked += BtnDayOpening_Clicked;
            BtnSessionOpening.Clicked += BtnSessionOpening_Clicked;
        }

        private void OpenDay()
        {
            if (WorkSessionsService.OpenDay() == false)
            {
                CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithMessage("Não foi possível abrir o dia.")
                            .ShowAlert();
            }

            UpdateUI();
        }

        private void BtnSessionOpening_Clicked(object sender, EventArgs e)
        {
            TerminalSessionModal terminalSessionModal = new TerminalSessionModal(this);
            terminalSessionModal.Run();
            terminalSessionModal.Destroy();

            UpdateUI();
        }

        public void UpdateUI()
        {
            if (WorkSessionsService.DayIsOpen())
            {
                BtnDayOpening.ButtonLabel.Text = LocalizedString.Instance["global_worksession_close_day"];
                BtnSessionOpening.Sensitive = true;
            }
            else
            {
                BtnDayOpening.ButtonLabel.Text = LocalizedString.Instance["global_worksession_open_day"];
                BtnSessionOpening.Sensitive = false;
            }
        }

    }
}
