using Gtk;
using LogicPOS.Api.Features.WorkSessions.GetLastClosedDay;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Linq;

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

            HandleDayReportPrinting();

            UpdateUI();
            POSWindow.Instance.UpdateUI();
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

            if(reportData == null)
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
