using DevExpress.Xpo;
using Gtk;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.POS.PrintingContext;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public partial class SessionOpeningModal
    {
        private void BtnDayOpening_Clicked(object sender, EventArgs e)
        {
            if (WorkSessionService.DayIsOpen())
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
            var openTerminalSessions = WorkSessionService.GetOpenTerminalSessions();

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

                WorkSessionService.CloseAllTerminalSessions();
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

            if (!WorkSessionService.CloseDay())
            {
                return;
            }

            //tchial0: ShowClosePeriodMessage(this, XPOSettings.WorkSessionPeriodDay);

            HandleDayReportPrinting();
            UpdateUI();
            POSWindow.Instance.UpdateUI();
        }

        private void HandleDayReportPrinting()
        {
            var printWorkSessionDayReportResponse = CustomAlerts.Question(this)
                                                                .WithSize(new Size(500, 350))
                                                                .WithTitleResource("global_button_label_print")
                                                                .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                                .ShowAlert();

            if (printWorkSessionDayReportResponse == ResponseType.Yes)
            {
                PrintingServices.PrintWorkSessionDayReport();
            }
        }

        private void AddEventHandlers()
        {
            BtnDayOpening.Clicked += BtnDayOpening_Clicked;
            BtnSessionOpening.Clicked += BtnSessionOpening_Clicked;
        }

        private void OpenDay()
        {
            if (WorkSessionService.OpenDay() == false)
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
            if (WorkSessionService.DayIsOpen())
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

        public void ShowClosePeriodMessage(Window parentWindow, pos_worksessionperiod pWorkSessionPeriod)
        {
            string messageResource = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day) ?
              LocalizedString.Instance["dialog_message_worksession_day_close_successfully"] :
              LocalizedString.Instance["dialog_message_worksession_terminal_close_successfully"]
            ;
            //used to store number of payments used, to increase dialog window size
            int workSessionPeriodTotalCount = 0;
            //Window Height Helper vars  
            int lineHeight = 28;
            int windowHeight = 300;
            pWorkSessionPeriod.DateEnd = DateTime.Now;
            //Get Session Period Details
            Hashtable resultHashTable = WorkSessionProcessor.GetSessionPeriodSummaryDetails(pWorkSessionPeriod.Oid);
            //Get Total Money in CashDrawer On Open/Close
            string totalMoneyInCashDrawerOnOpen = string.Format("{0}: {1}", LocalizedString.Instance["global_total_cashdrawer_on_open"], LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawerOnOpen"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            string totalMoneyInCashDrawer = string.Format("{0}: {1}", LocalizedString.Instance["global_total_cashdrawer"], LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawer"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            //Get Total Money and TotalMoney Out (NonPayments)
            string totalMoneyIn = string.Format("{0}: {1}", LocalizedString.Instance["global_cashdrawer_money_in"], LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyIn"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            string totalMoneyOut = string.Format("{0}: {1}", LocalizedString.Instance["global_cashdrawer_money_out"], LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyOut"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            //Init Message
            string messageTotalSummary = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, totalMoneyInCashDrawerOnOpen, totalMoneyInCashDrawer, totalMoneyIn, totalMoneyOut);

            //Get Payments Totals

            XPCollection workSessionPeriodTotal = WorkSessionProcessor.GetSessionPeriodTotal(pWorkSessionPeriod);
            if (workSessionPeriodTotal.Count > 0)
            {
                messageTotalSummary += string.Format("{0}{1}{0}", Environment.NewLine, LocalizedString.Instance["global_total_by_type_of_payment"]);
                foreach (pos_worksessionperiodtotal item in workSessionPeriodTotal)
                {
                    messageTotalSummary += string.Format("{1}-{2}: {3}{0}", Environment.NewLine, item.PaymentMethod.Acronym, item.PaymentMethod.Designation, LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(item.Total, XPOSettings.ConfigurationSystemCurrency.Acronym));
                }
                workSessionPeriodTotalCount = workSessionPeriodTotal.Count;
            }

            windowHeight = (workSessionPeriodTotalCount > 0) ? windowHeight + ((workSessionPeriodTotalCount + 2) * lineHeight) : windowHeight + lineHeight;


            CustomAlerts.Information(parentWindow)
                        .WithSize(new Size(600, windowHeight))
                        .WithTitleResource("global_information")
                        .WithMessage(string.Format(messageResource, messageTotalSummary))
                        .ShowAlert();
        }
    }
}
