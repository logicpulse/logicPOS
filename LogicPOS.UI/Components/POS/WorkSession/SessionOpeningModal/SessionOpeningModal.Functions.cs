using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
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

        private void CloseDay()
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
                return;
            }

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
                    return;
                }

                WorkSessionService.CloseTerminalAllSessions();
            }

            DataBaseBackup.ShowRequestBackupDialog(this);

            if (!WorkSessionService.CloseDay())
            {
                return;
            }

            UpdateButtons();

            //tchial0: ShowClosePeriodMessage(this, XPOSettings.WorkSessionPeriodDay);

            var pResponse = CustomAlerts.Question(this)
                                        .WithSize(new Size(500, 350))
                                        .WithTitleResource("global_button_label_print")
                                        .WithMessageResource("dialog_message_request_print_document_confirmation")
                                        .ShowAlert();
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

            UpdateButtons();
        }

        private void BtnSessionOpening_Clicked(object sender, EventArgs e)
        {
            bool result;
            //Update UI
            POSWindow.Instance.UpdateWorkSessionUI();


            //WorkSessionProcessor.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodTerminal, true );
            PosCashDrawerDialog dialogCashDrawer = new PosCashDrawerDialog(this, DialogFlags.DestroyWithParent);

            int response = dialogCashDrawer.Run();
            if (response == (int)ResponseType.Ok)
            {
                //Get Fresh XPO Objects, Prevent Deleted Object Bug
                pos_worksessionperiod workSessionPeriodDay = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodDay.Oid);
                pos_worksessionperiod workSessionPeriodTerminal = XPOSettings.WorkSessionPeriodTerminal;
                var originalMovType = dialogCashDrawer.MovementType;
                decimal addedMoney;
                switch (dialogCashDrawer.MovementType.Token)
                {
                    case "CASHDRAWER_OPEN":
                        result = WorkSessionProcessor.SessionPeriodOpen(WorkSessionPeriodType.Terminal, dialogCashDrawer.MovementDescription);

                        if (result)
                        {
                            POSWindow.Instance.UpdateWorkSessionUI();
                            POSWindow.Instance.TicketList.UpdateOrderStatusBar();

                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                            // tchial0
                            //result = WorkSessionProcessor.PersistWorkSessionMovement(workSessionPeriodTerminal,
                            //                                                         originalMovType,
                            //                                                         XPOSettings.LoggedUser,
                            //                                                         TerminalService.Terminal,
                            //                                                         XPOUtility.CurrentDateTimeAtomic(),
                            //                                                         dialogCashDrawer.TotalAmountInCashDrawer,
                            //                                                         dialogCashDrawer.MovementDescription);

                            CustomAlerts.Information(this)
                                        .WithSize(new Size(500, 280))
                                        .WithTitleResource("global_information")
                                        .WithMessageResource("dialog_message_cashdrawer_open_successfully")
                                        .ShowAlert();


                            var pResponse = CustomAlerts.Question(this)
                                                        .WithSize(new Size(500, 350))
                                                        .WithTitleResource("global_information")
                                                        .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                        .ShowAlert();

                            POSWindow.Instance.BtnNewDocument.Sensitive = false;

                            if (pResponse == ResponseType.Yes)
                            {
                                //PrintWorkSessionMovement
                                var thermalPrinter = TerminalService.Terminal.ThermalPrinter;
                                if (thermalPrinter != null)
                                {
                                    FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(
                                        dialogCashDrawer,
                                        thermalPrinter,
                                        LocalizedString.Instance["ticket_title_worksession_terminal_open"],
                                        0.0m,
                                        dialogCashDrawer.TotalAmountInCashDrawer,
                                        dialogCashDrawer.MovementDescription,
                                        dialogCashDrawer.MovementDescription);
                                }
                            }
                            //Enable UI Buttons When Have Open Session
                            POSWindow.Instance.BtnNewDocument.Sensitive = true;

                        }
                        else
                        {
                            CustomAlerts.ShowContactSupportErrorAlert(dialogCashDrawer);
                        }


                        //Check if is added money
                        if (dialogCashDrawer.MovementAmountMoney >= 0 && dialogCashDrawer.MovementAmountMoney != dialogCashDrawer.TotalAmountInCashDrawer)
                        {

                            //if 1 ADD if -1 REMOVE
                            int addRemoveMoney;
                            //Add or Remove Text String for print ticket
                            string moneyInOutLabel;
                            string moneyInOutLabelAudit;
                            string audit;
                            //Added money to total amount
                            if (dialogCashDrawer.TotalAmountInCashDrawer > dialogCashDrawer.MovementAmountMoney)
                            {
                                addedMoney = dialogCashDrawer.TotalAmountInCashDrawer - dialogCashDrawer.MovementAmountMoney;
                                addRemoveMoney = -1;
                                moneyInOutLabel = "ticket_title_worksession_money_out";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_out";
                                audit = "CASHDRAWER_OUT";
                                dialogCashDrawer.MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(Guid.Parse("069564cb-074a-4c91-931e-554454b1ab7e"));
                            }
                            else
                            {
                                addedMoney = dialogCashDrawer.MovementAmountMoney - dialogCashDrawer.TotalAmountInCashDrawer;
                                addRemoveMoney = 1;
                                moneyInOutLabel = "ticket_title_worksession_money_in";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_in";
                                audit = "CASHDRAWER_IN";
                                dialogCashDrawer.MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(Guid.Parse("2ef29ce6-314c-4f40-897f-e31802dbeef3"));
                            }

                            dialogCashDrawer.TotalAmountInCashDrawer = dialogCashDrawer.MovementAmountMoney;


                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                            //tchial0
                            //var resultProcess = WorkSessionProcessor.PersistWorkSessionMovement(
                            //  workSessionPeriodTerminal,
                            //  dialogCashDrawer.MovementType,
                            //  XPOSettings.LoggedUser,
                            //  TerminalService.Terminal,
                            //  XPOUtility.CurrentDateTimeAtomic(),
                            //  (addRemoveMoney * addedMoney),
                            //  dialogCashDrawer.MovementDescription
                            //);

                            bool resultProcess = true;

                            if (resultProcess)
                            {
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, LocalizedString.Instance[moneyInOutLabel], addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

                                var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                                if (!resultOpenDoor)
                                {
                                    CustomAlerts.Information(this)
                                                .WithSize(new Size(500, 300))
                                                .WithTitleResource("global_information")
                                                .WithMessageResource("open_cash_draw_permissions")
                                                .ShowAlert();
                                }
                                else
                                {
                                    XPOUtility.Audit(audit, string.Format(
                                         LocalizedString.Instance[moneyInOutLabelAudit],
                                         LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(addedMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                         dialogCashDrawer.MovementDescription)
                                     );
                                }
                            }
                        }


                        break;

                    case "CASHDRAWER_CLOSE":


                        //Check if is added money
                        if (dialogCashDrawer.MovementAmountMoney >= 0 && dialogCashDrawer.MovementAmountMoney != dialogCashDrawer.TotalAmountInCashDrawer)
                        {

                            //if 1 ADD if -1 REMOVE
                            int addRemoveMoney;
                            //Add or Remove Text String for print ticket
                            string moneyInOutLabel;
                            string moneyInOutLabelAudit;
                            string audit;
                            //Added money to total amount
                            if (dialogCashDrawer.TotalAmountInCashDrawer > dialogCashDrawer.MovementAmountMoney)
                            {
                                addedMoney = dialogCashDrawer.TotalAmountInCashDrawer - dialogCashDrawer.MovementAmountMoney;
                                addRemoveMoney = -1;
                                moneyInOutLabel = "ticket_title_worksession_money_out";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_out";
                                audit = "CASHDRAWER_OUT";
                                dialogCashDrawer.MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(Guid.Parse("069564cb-074a-4c91-931e-554454b1ab7e"));
                            }
                            else
                            {
                                addedMoney = dialogCashDrawer.MovementAmountMoney - dialogCashDrawer.TotalAmountInCashDrawer;
                                addRemoveMoney = 1;
                                moneyInOutLabel = "ticket_title_worksession_money_in";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_in";
                                audit = "CASHDRAWER_IN";
                                dialogCashDrawer.MovementType = XPOUtility.GetEntityById<pos_worksessionmovementtype>(Guid.Parse("2ef29ce6-314c-4f40-897f-e31802dbeef3"));

                            }

                            //Total = IN
                            dialogCashDrawer.TotalAmountInCashDrawer = dialogCashDrawer.MovementAmountMoney;

                            // GlobalFramework.WorkSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);

                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                            //tchial0
                            //var resultProcess = WorkSessionProcessor.PersistWorkSessionMovement(workSessionPeriodTerminal,
                            //                                                                    dialogCashDrawer.MovementType,
                            //                                                                    XPOSettings.LoggedUser,
                            //                                                                    TerminalService.Terminal,
                            //                                                                    XPOUtility.CurrentDateTimeAtomic(),
                            //                                                                    (addRemoveMoney * addedMoney),
                            //                                                                    dialogCashDrawer.MovementDescription);

                            bool resultProcess = true;

                            if (resultProcess)
                            {

                                //PrintCashDrawerOpenAndMoneyInOut
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, LocalizedString.Instance[moneyInOutLabel], addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

                                //Open CashDrawer
                                var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                                if (!resultOpenDoor)
                                {

                                    CustomAlerts.Information(this)
                                                .WithTitleResource("global_information")
                                                .WithMessageResource("open_cash_draw_permissions")
                                                .ShowAlert();
                                }
                                else
                                {
                                    //Audit
                                    XPOUtility.Audit(audit, string.Format(LocalizedString.Instance[moneyInOutLabelAudit],
                                                                          LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(addedMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                                                          dialogCashDrawer.MovementDescription)
                                     );
                                }
                            }
                        }

                        result = WorkSessionProcessor.SessionPeriodClose(workSessionPeriodTerminal);

                        if (result)
                        {
                            POSWindow.Instance.UpdateWorkSessionUI();
                            POSWindow.Instance.LabelCurrentTable.Text = LocalizedString.Instance["status_message_open_cashdrawer"];

                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(workSessionPeriodTerminal.Oid);

                            //tchial0
                            //result = WorkSessionProcessor.PersistWorkSessionMovement(workSessionPeriodTerminal,
                            //                                                         originalMovType,
                            //                                                         XPOSettings.LoggedUser,
                            //                                                         TerminalService.Terminal,
                            //                                                         XPOUtility.CurrentDateTimeAtomic(),
                            //                                                         dialogCashDrawer.MovementAmountMoney,
                            //                                                         dialogCashDrawer.MovementDescription);




                            var pResponse = CustomAlerts.Question(this)
                                                        .WithSize(new Size(500, 350))
                                                        .WithTitleResource("global_button_label_print")
                                                        .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                        .ShowAlert();

                            POSWindow.Instance.BtnNewDocument.Sensitive = false;

                            ShowClosePeriodMessage(dialogCashDrawer, workSessionPeriodTerminal);

                            if (pResponse == ResponseType.Yes)
                            {
                                var workSessionDto = MappingUtils.GetPrintWorkSessionDto(workSessionPeriodTerminal);
                                FrameworkCalls.PrintWorkSessionMovement(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, workSessionDto, TerminalService.Terminal.Designation);
                            }

                        }
                        break;

                    case "CASHDRAWER_IN":

                        dialogCashDrawer.TotalAmountInCashDrawer += dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                        //result = WorkSessionProcessor.PersistWorkSessionMovement(workSessionPeriodTerminal,
                        //                                                         dialogCashDrawer.MovementType,
                        //                                                         XPOSettings.LoggedUser,
                        //                                                         TerminalService.Terminal,
                        //                                                         XPOUtility.CurrentDateTimeAtomic(),
                        //                                                         dialogCashDrawer.MovementAmountMoney,
                        //                                                         dialogCashDrawer.MovementDescription);
                        result = true;

                        if (result)
                        {
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, LocalizedString.Instance["ticket_title_worksession_money_in"], dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

                            var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                            if (!resultOpenDoor)
                            {
                                CustomAlerts.Information(this)
                                            .WithTitleResource("global_information")
                                            .WithMessageResource("open_cash_draw_permissions")
                                            .ShowAlert();
                            }
                            else
                            {
                                XPOUtility.Audit("CASHDRAWER_IN", string.Format(LocalizedString.Instance["audit_message_cashdrawer_in"],
                                                                                LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                                                                dialogCashDrawer.MovementDescription)
                                 );
                            }

                            CustomAlerts.Information(this)
                                        .WithSize(new Size(500, 280))
                                        .WithTitleResource("global_information")
                                        .WithMessageResource("dialog_message_operation_successfully")
                                        .ShowAlert();
                        }
                        else
                        {
                            CustomAlerts.ShowContactSupportErrorAlert(dialogCashDrawer);
                        }
                        break;

                    case "CASHDRAWER_OUT":

                        dialogCashDrawer.TotalAmountInCashDrawer -= dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                        result = WorkSessionProcessor.PersistWorkSessionMovement(workSessionPeriodTerminal,
                                                                                 dialogCashDrawer.MovementType,
                                                                                 XPOSettings.LoggedUser,
                                                                                 null,
                                                                                 XPOUtility.CurrentDateTimeAtomic(),
                                                                                 -dialogCashDrawer.MovementAmountMoney,
                                                                                 dialogCashDrawer.MovementDescription);

                        if (result)
                        {

                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer,
                                                                            TerminalService.Terminal.ThermalPrinter,
                                                                            LocalizedString.Instance["ticket_title_worksession_money_out"],
                                                                            dialogCashDrawer.MovementAmountMoney,
                                                                            dialogCashDrawer.TotalAmountInCashDrawer,
                                                                            dialogCashDrawer.MovementDescription,
                                                                            TerminalService.Terminal.Designation);

                            var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                            if (!resultOpenDoor)
                            {
                                CustomAlerts.Information(this)
                                            .WithTitleResource("global_information")
                                            .WithMessageResource("open_cash_draw_permissions")
                                            .ShowAlert();
                            }
                            else
                            {
                                XPOUtility.Audit("CASHDRAWER_OUT", string.Format(LocalizedString.Instance["audit_message_cashdrawer_out"],
                                                                                 LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                                                                 dialogCashDrawer.MovementDescription)
                                 );
                            }

                            CustomAlerts.Information(this)
                                        .WithSize(new Size(500, 280))
                                        .WithTitleResource("global_information")
                                        .WithMessageResource("dialog_message_operation_successfully")
                                        .ShowAlert();
                        }
                        else
                        {
                            CustomAlerts.ShowContactSupportErrorAlert(dialogCashDrawer);
                        }

                        break;

                    case "CASHDRAWER_MONEY_OUT":
                        break;

                    default:
                        break;
                }
            };
            dialogCashDrawer.Destroy();
        }

        public void UpdateButtons()
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
