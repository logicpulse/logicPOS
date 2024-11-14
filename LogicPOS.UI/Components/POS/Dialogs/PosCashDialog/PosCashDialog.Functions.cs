using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.DataLayer;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Printing.Utility;
using LogicPOS.UI;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosCashDialog
    {
        private void _touchButtonStartStopWorkSessionPeriodDay_Clicked(object sender, EventArgs e)
        {
            //Stop WorkSessionPeriodDay
            if (XPOSettings.WorkSessionPeriodDay != null && XPOSettings.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
            {
                //Check if we can StopSessionPeriodDay
                bool resultCanClose = CanCloseWorkSessionPeriodDay();
                if (resultCanClose == false) return;

                // ShowRequestBackupDialog and Backup only if PluginSoftwareVendor is Active
                if (LogicPOS.Settings.PluginSettings.HasSoftwareVendorPlugin)
                {
                    //Request User to do a DatabaseBackup, After Check Can Close
                    DataBaseBackup.ShowRequestBackupDialog(this);
                }

                bool result = WorkSessionProcessor.SessionPeriodClose(XPOSettings.WorkSessionPeriodDay);
                if (result)
                {
                    _touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day");
                    _touchButtonCashDrawer.Sensitive = false;

                    ShowClosePeriodMessage(this, XPOSettings.WorkSessionPeriodDay);

                    var pResponse = CustomAlerts.Question(this)
                                                .WithSize(new Size(500, 350))
                                                .WithTitleResource("global_button_label_print")
                                                .WithMessageResource("dialog_message_request_print_document_confirmation")
                                                .ShowAlert();

                    if (pResponse == ResponseType.Yes)
                    {
                        var workSessionDto = XPOUtility.WorkSession.GetCurrentWorkSessionPeriodDayDto();
                    }
                }
            }
            else
            {
                bool result = WorkSessionProcessor.SessionPeriodOpen(WorkSessionPeriodType.Day);
                if (result)
                {
                    _touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_close_day");
                    _touchButtonCashDrawer.Sensitive = true;
                }
            }
        }

        private bool CanCloseWorkSessionPeriodDay()
        {
            //Check if has Working Open Orders/Tables
            SQLSelectResultData xPSelectDataTables = WorkSessionProcessor.GetOpenOrderTables();
            int noOfOpenOrderTables = xPSelectDataTables.DataRows.Length;
            if (noOfOpenOrderTables > 0)
            {
                string openOrderTables = string.Empty;
                pos_configurationplacetable currentOpenOrderTable;
                foreach (SelectStatementResultRow row in xPSelectDataTables.DataRows)
                {
                    Guid tableOid = new Guid(row.Values[xPSelectDataTables.GetFieldIndexFromName("PlaceTable")].ToString());
                    currentOpenOrderTable = XPOSettings.Session.GetObjectByKey<pos_configurationplacetable>(tableOid);
                    openOrderTables += string.Format("{0}{1}", currentOpenOrderTable.Designation, " ");
                }


                ResponseType dialogResponse = CustomAlerts.Error(this)
                            .WithSize(new Size(620, 300))
                            .WithTitleResource("global_error")
                            .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_worksession_period_warning_open_orders_tables"),
                                          noOfOpenOrderTables,
                                          string.Format("{0}{1}", Environment.NewLine, openOrderTables)))
                            .ShowAlert();

                //Exit Event Button Without Close Cash Drwawer
                return false;
            }

            //Check if has Working Terminal Sessions
            SQLSelectResultData xPSelectDataTerminals = WorkSessionProcessor.GetSessionPeriodOpenTerminalSessions();
            int noOfTerminalOpenSessions = xPSelectDataTerminals.DataRows.Length;
            if (noOfTerminalOpenSessions > 0)
            {
                string openTerminals = string.Empty;
                pos_configurationplaceterminal currentOpenSessionTerminal;
                foreach (SelectStatementResultRow row in xPSelectDataTerminals.DataRows)
                {
                    Guid terminalOid = new Guid(row.Values[xPSelectDataTerminals.GetFieldIndexFromName("Terminal")].ToString());
                    currentOpenSessionTerminal = XPOSettings.Session.GetObjectByKey<pos_configurationplaceterminal>(terminalOid);
                    openTerminals += string.Format("{0}{1} - {2}", Environment.NewLine, currentOpenSessionTerminal.Designation, row.Values[xPSelectDataTerminals.GetFieldIndexFromName("Designation")].ToString());
                }

                ResponseType responseType = CustomAlerts.Question(this)
                    .WithSize(new Size(600, 400))
                    .WithTitleResource("global_information")
                    .WithMessage(string.Format(GeneralUtils.GetResourceByName("dialog_message_worksession_period_warning_open_terminals"),
                                          noOfTerminalOpenSessions,
                                          $"\n{openTerminals}"))
                            .ShowAlert();

                if (responseType == ResponseType.Yes)
                {
                    return logicpos.Utils.CloseAllOpenTerminals(this, XPOSettings.Session);

                }
                else
                {

                    return false;
                }


            }
            return true;
        }

        //Alteração no funcionamento do Inicio/fecho Sessão [IN:014330]
        private void _touchButtonCashDrawer_Clicked(object sender, EventArgs e)
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
                                        CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_terminal_open"),
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
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

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
                                         CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabelAudit),
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
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

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
                                    XPOUtility.Audit(audit, string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabelAudit),
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
                            POSWindow.Instance.LabelCurrentTable.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "status_message_open_cashdrawer");

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
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalService.Terminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_money_in"), dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription, TerminalService.Terminal.Designation);

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
                                XPOUtility.Audit("CASHDRAWER_IN", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_in"),
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
                                                                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_money_out"),
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
                                XPOUtility.Audit("CASHDRAWER_OUT", string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_out"),
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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        public void UpdateButtons()
        {
            //Update Toolbar UI Buttons After ToolBox and ToolBar
            if (XPOSettings.WorkSessionPeriodDay != null)
            {
                //With Valid WorkSessionPeriodDay
                if (XPOSettings.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    if (_touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text
                        != CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_close_day"))
                    {
                        _touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_close_day");
                    }

                    if (!_touchButtonCashDrawer.Sensitive == true)
                    {
                        _touchButtonCashDrawer.Sensitive = true;
                    }

                    _touchButtonCashDrawer.Sensitive = true;
                }
            }
            //No WorkSessionPeriodDay
            else
            {
                if (_touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text != CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day"))
                {
                    _touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day");
                }
                _touchButtonCashDrawer.Sensitive = false;
            }
        }

        /// <summary>
        /// Show Close Message, Shared for Day and Terminal Sessions
        /// </summary>
        /// <param name="pWorkSessionPeriod"></param>
        /// <returns></returns>
        public void ShowClosePeriodMessage(Window parentWindow, pos_worksessionperiod pWorkSessionPeriod)
        {
            string messageResource = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day) ?
              CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_worksession_day_close_successfully") :
              CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_worksession_terminal_close_successfully")
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
            string totalMoneyInCashDrawerOnOpen = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_cashdrawer_on_open"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawerOnOpen"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            string totalMoneyInCashDrawer = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_cashdrawer"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawer"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            //Get Total Money and TotalMoney Out (NonPayments)
            string totalMoneyIn = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_cashdrawer_money_in"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyIn"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            string totalMoneyOut = string.Format("{0}: {1}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_cashdrawer_money_out"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyOut"], XPOSettings.ConfigurationSystemCurrency.Acronym));
            //Init Message
            string messageTotalSummary = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, totalMoneyInCashDrawerOnOpen, totalMoneyInCashDrawer, totalMoneyIn, totalMoneyOut);

            //Get Payments Totals

            XPCollection workSessionPeriodTotal = WorkSessionProcessor.GetSessionPeriodTotal(pWorkSessionPeriod);
            if (workSessionPeriodTotal.Count > 0)
            {
                messageTotalSummary += string.Format("{0}{1}{0}", Environment.NewLine, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_total_by_type_of_payment"));
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
