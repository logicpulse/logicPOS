﻿using DevExpress.Xpo;
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

                //Stop WorkSession Period Day
                bool result = WorkSessionProcessor.SessionPeriodClose(XPOSettings.WorkSessionPeriodDay);
                if (result)
                {
                    _touchButtonStartStopWorkSessionPeriodDay.ButtonLabel.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_worksession_open_day");
                    _touchButtonCashDrawer.Sensitive = false;

                    //Show ClosePeriodMessage
                    ShowClosePeriodMessage(this, XPOSettings.WorkSessionPeriodDay); /* IN009054 -  WorkSessionPeriodDay: NullReferenceException when closing day on non-licensed app */

                    //PrintWorkSessionMovement Day
                    //PrintRouter.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);
                    //PrintRouter.PrintWorkSessionMovement(TerminalSettings.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                    ResponseType pResponse = logicpos.Utils.ShowMessageBox(
                      this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_print"),
                      CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_request_print_document_confirmation"));

                    if (pResponse == ResponseType.Yes)
                    {
                        var workSessionDto = XPOUtility.WorkSession.GetCurrentWorkSessionPeriodDayDto();
                        FrameworkCalls.PrintWorkSessionMovement(this, TerminalSettings.LoggedTerminal.ThermalPrinter, workSessionDto);
                    }
                    //FrameworkCalls.PrintWorkSessionMovement(this, TerminalSettings.LoggedTerminal.ThermalPrinter, GlobalFramework.WorkSessionPeriodDay);
                }
            }
            //Start WorkSessionPeriodDay
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

                ResponseType dialogResponse = logicpos.Utils.ShowMessageBox(
                  this,
                  DialogFlags.Modal,
                  new Size(620, 300),
                  MessageType.Error,
                  ButtonsType.Close,
                  CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"),
                  string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_worksession_period_warning_open_orders_tables"), noOfOpenOrderTables, string.Format("{0}{1}", Environment.NewLine, openOrderTables))
                );

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

                ////Check if Has Opened Terminal Connections before Close Day
                //PosMessageDialog messageDialog = new PosMessageDialog(
                //    this,
                //    DialogFlags.DestroyWithParent,
                //    new Size(600, 300),
                //    string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_worksession_period_warning_open_terminals, noOfTerminalOpenSessions, string.Format("{0}{1}", Environment.NewLine, openTerminals)),
                //    MessageType.Warning,
                //    ResponseType.Ok,
                //    CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_ok
                //);

                //int messageDialogResponse = messageDialog.Run();
                //messageDialog.Destroy();

                ResponseType responseType = logicpos.Utils.ShowMessageBox(this, DialogFlags.Modal, new Size(600, 400), MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"),
                    string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_worksession_period_warning_open_terminals"), noOfTerminalOpenSessions, string.Format("{0}{1}", Environment.NewLine, openTerminals))
                );

                if (responseType == ResponseType.Yes)
                {
                    return logicpos.Utils.CloseAllOpenTerminals(this, XPOSettings.Session);

                }
                else
                {
                    //Exit Event Button Without Close Period Day Session
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
            GlobalApp.PosMainWindow.UpdateWorkSessionUI();


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
                        //Start Terminal Period
                        result = WorkSessionProcessor.SessionPeriodOpen(WorkSessionPeriodType.Terminal, dialogCashDrawer.MovementDescription);

                        if (result)
                        {
                            //Update UI
                            GlobalApp.PosMainWindow.UpdateWorkSessionUI();
                            GlobalApp.PosMainWindow.TicketList.UpdateOrderStatusBar();

                            //Here we already have GlobalFramework.WorkSessionPeriodTerminal, assigned on WorkSessionProcessor.SessionPeriodStart
                            //Get Fresh XPO Objects, Prevent Deleted Object Bug
                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                            result = WorkSessionProcessor.PersistWorkSessionMovement(
                             workSessionPeriodTerminal,
                             originalMovType,
                             XPOSettings.LoggedUser,
                             TerminalSettings.LoggedTerminal,
                             XPOUtility.CurrentDateTimeAtomic(),
                             dialogCashDrawer.TotalAmountInCashDrawer,
                             dialogCashDrawer.MovementDescription
                           );

                            logicpos.Utils.ShowMessageBox(dialogCashDrawer, DialogFlags.Modal, new Size(500, 280), MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_cashdrawer_open_successfully"));

                            var pResponse = logicpos.Utils.ShowMessageBox(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_print"),
                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_request_print_document_confirmation"));

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.PosMainWindow.BtnNewDocument.Sensitive = false;
                            //Show ClosePeriodMessage
                            //ShowClosePeriodMessage(dialogCashDrawer, workSessionPeriodTerminal);
                            if (pResponse == ResponseType.Yes)
                            {
                                //PrintWorkSessionMovement
                                var thermalPrinter = TerminalSettings.LoggedTerminal.ThermalPrinter;
                                if (thermalPrinter != null)
                                {
                                    FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(
                                        dialogCashDrawer,
                                        thermalPrinter,
                                        CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_terminal_open"),
                                        0.0m,
                                        dialogCashDrawer.TotalAmountInCashDrawer,
                                        dialogCashDrawer.MovementDescription);
                                }
                            }
                            //Enable UI Buttons When Have Open Session
                            GlobalApp.PosMainWindow.BtnNewDocument.Sensitive = true;

                        }
                        else
                        {
                            logicpos.Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "app_error_contact_support"));
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
                            //Total = IN
                            dialogCashDrawer.TotalAmountInCashDrawer = dialogCashDrawer.MovementAmountMoney;

                            // GlobalFramework.WorkSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);

                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                            var resultProcess = WorkSessionProcessor.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              dialogCashDrawer.MovementType,
                              XPOSettings.LoggedUser,
                              TerminalSettings.LoggedTerminal,
                              XPOUtility.CurrentDateTimeAtomic(),
                              (addRemoveMoney * addedMoney),
                              dialogCashDrawer.MovementDescription
                            );

                            if (resultProcess)
                            {

                                //PrintCashDrawerOpenAndMoneyInOut
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalSettings.LoggedTerminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);

                                //Open CashDrawer
                                var resultOpenDoor = PrintingUtils.OpenDoor();
                                if (!resultOpenDoor)
                                {
                                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "open_cash_draw_permissions")));
                                }
                                else
                                {
                                    //Audit
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

                            var resultProcess = WorkSessionProcessor.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              dialogCashDrawer.MovementType,
                              XPOSettings.LoggedUser,
                              TerminalSettings.LoggedTerminal,
                              XPOUtility.CurrentDateTimeAtomic(),
                              (addRemoveMoney * addedMoney),
                              dialogCashDrawer.MovementDescription
                            );

                            if (resultProcess)
                            {

                                //PrintCashDrawerOpenAndMoneyInOut
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalSettings.LoggedTerminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);

                                //Open CashDrawer
                                var resultOpenDoor = PrintingUtils.OpenDoor();
                                if (!resultOpenDoor)
                                {
                                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "open_cash_draw_permissions")));
                                }
                                else
                                {
                                    //Audit
                                    XPOUtility.Audit(audit, string.Format(
                                         CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, moneyInOutLabelAudit),
                                         LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(addedMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                         dialogCashDrawer.MovementDescription)
                                     );
                                }
                            }
                        }
                        //workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                        //Stop Terminal Period
                        result = WorkSessionProcessor.SessionPeriodClose(workSessionPeriodTerminal);

                        if (result)
                        {
                            //Update UI
                            GlobalApp.PosMainWindow.UpdateWorkSessionUI();
                            GlobalApp.PosMainWindow.LabelCurrentTable.Text = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "status_message_open_cashdrawer");

                            //Get Fresh XPO Objects, Prevent Deleted Object Bug
                            workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(workSessionPeriodTerminal.Oid);

                            //dialogCashDrawer.TotalAmountInCashDrawer -= dialogCashDrawer.MovementAmountMoney;


                            //Add CASHDRAWER_CLOSE Movement to Day Period
                            result = WorkSessionProcessor.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              originalMovType,
                              XPOSettings.LoggedUser,
                              TerminalSettings.LoggedTerminal,
                              XPOUtility.CurrentDateTimeAtomic(),
                              dialogCashDrawer.MovementAmountMoney,
                              dialogCashDrawer.MovementDescription
                            );

                            var pResponse = logicpos.Utils.ShowMessageBox(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_button_label_print"),
                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_request_print_document_confirmation"));

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.PosMainWindow.BtnNewDocument.Sensitive = false;
                            //Show ClosePeriodMessage
                            ShowClosePeriodMessage(dialogCashDrawer, workSessionPeriodTerminal);

                            if (pResponse == ResponseType.Yes)
                            {
                                //PrintWorkSessionMovement
                                var workSessionDto = MappingUtils.GetPrintWorkSessionDto(workSessionPeriodTerminal);
                                FrameworkCalls.PrintWorkSessionMovement(dialogCashDrawer, TerminalSettings.LoggedTerminal.ThermalPrinter, workSessionDto);
                            }

                            //GlobalFramework.WorkSessionPeriodTerminal = null;
                        }
                        break;

                    case "CASHDRAWER_IN":

                        dialogCashDrawer.TotalAmountInCashDrawer += dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                        result = WorkSessionProcessor.PersistWorkSessionMovement(
                          workSessionPeriodTerminal,
                          dialogCashDrawer.MovementType,
                          XPOSettings.LoggedUser,
                          TerminalSettings.LoggedTerminal,
                          XPOUtility.CurrentDateTimeAtomic(),
                          dialogCashDrawer.MovementAmountMoney,
                          dialogCashDrawer.MovementDescription
                        );

                        if (result)
                        {
                            //PrintCashDrawerOpenAndMoneyInOut
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalSettings.LoggedTerminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_money_in"), dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);
                            //Open CashDrawer
                            var resultOpenDoor = PrintingUtils.OpenDoor();
                            if (!resultOpenDoor)
                            {
                                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "open_cash_draw_permissions")));
                            }
                            else
                            {
                                //Audit
                                XPOUtility.Audit("CASHDRAWER_IN", string.Format(
                                     CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_in"),
                                     LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                     dialogCashDrawer.MovementDescription)
                                 );
                            }

                            //ShowMessage
                            logicpos.Utils.ShowMessageBox(dialogCashDrawer, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_operation_successfully"));
                        }
                        else
                        {
                            logicpos.Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "app_error_contact_support"));
                        }
                        break;

                    //Work with Terminal Session
                    case "CASHDRAWER_OUT":

                        dialogCashDrawer.TotalAmountInCashDrawer -= dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodTerminal.Oid);

                        //In Period Terminal
                        result = WorkSessionProcessor.PersistWorkSessionMovement(
                          workSessionPeriodTerminal,
                          dialogCashDrawer.MovementType,
                          XPOSettings.LoggedUser,
                          TerminalSettings.LoggedTerminal,
                          XPOUtility.CurrentDateTimeAtomic(),
                          -dialogCashDrawer.MovementAmountMoney,
                          dialogCashDrawer.MovementDescription
                        );

                        if (result)
                        {
                            //PrintCashDrawerOpenAndMoneyInOut
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, TerminalSettings.LoggedTerminal.ThermalPrinter, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "ticket_title_worksession_money_out"), dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);
                            //Open CashDrawer
                            var resultOpenDoor = PrintingUtils.OpenDoor();
                            if (!resultOpenDoor)
                            {
                                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "open_cash_draw_permissions")));
                            }
                            else
                            {
                                //Audit
                                XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                                     CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "audit_message_cashdrawer_out"),
                                     LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney, XPOSettings.ConfigurationSystemCurrency.Acronym),
                                     dialogCashDrawer.MovementDescription)
                                 );
                            }

                            //ShowMessage
                            logicpos.Utils.ShowMessageBox(dialogCashDrawer, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_operation_successfully"));
                        }
                        else
                        {
                            logicpos.Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "app_error_contact_support"));
                        }

                        break;

                    case "CASHDRAWER_MONEY_OUT":
                        break;

                    default:
                        break;
                }
            };
            dialogCashDrawer.Destroy();

            //TODO: Remove Comments
            //_logger.Debug(string.Format("WorkSessionProcessor: [{0}]", WorkSessionProcessor.GetSessionPeriodCashDrawerOpenOrCloseAmount(GlobalFramework.WorkSessionPeriodDay)));
            //if (GlobalFramework.WorkSessionPeriodDay != null) WorkSessionProcessor.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodDay, true);
            //if (GlobalFramework.WorkSessionPeriodTerminal != null) WorkSessionProcessor.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodTerminal, true);
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

            logicpos.Utils.ShowMessageBox(
              parentWindow,
              DialogFlags.Modal,
              new Size(600, windowHeight),
              MessageType.Info,
              ButtonsType.Close,
              "Info",
              string.Format(messageResource, messageTotalSummary)
            );
        }
    }
}
