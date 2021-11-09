using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Hardware.Printers;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.Classes.DataLayer;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosCashDialog
    {
        void _touchButtonStartStopWorkSessionPeriodDay_Clicked(object sender, EventArgs e)
        {
            //Stop WorkSessionPeriodDay
            if (GlobalFramework.WorkSessionPeriodDay != null && GlobalFramework.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
            {
                //Check if we can StopSessionPeriodDay
                bool resultCanClose = CanCloseWorkSessionPeriodDay();
                if (resultCanClose == false) return;

                // ShowRequestBackupDialog and Backup only if PluginSoftwareVendor is Active
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    //Request User to do a DatabaseBackup, After Check Can Close
                    DataBaseBackup.ShowRequestBackupDialog(this);
                }

                //Stop WorkSession Period Day
                bool result = ProcessWorkSessionPeriod.SessionPeriodClose(GlobalFramework.WorkSessionPeriodDay);
                if (result)
                {
                    _touchButtonStartStopWorkSessionPeriodDay.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_day");
                    _touchButtonCashDrawer.Sensitive = false;

                    //Show ClosePeriodMessage
                    ShowClosePeriodMessage(this, GlobalFramework.WorkSessionPeriodDay); /* IN009054 -  WorkSessionPeriodDay: NullReferenceException when closing day on non-licensed app */

                    //PrintWorkSessionMovement Day
                    //PrintRouter.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodDay);
                    //PrintRouter.PrintWorkSessionMovement(GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);
                    ResponseType pResponse = Utils.ShowMessageTouch(
                      this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print"),
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_request_print_document_confirmation"));

                    if (pResponse == ResponseType.Yes)
                    {
                        FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, GlobalFramework.WorkSessionPeriodTerminal);
                    }
                    //FrameworkCalls.PrintWorkSessionMovement(this, GlobalFramework.LoggedTerminal.ThermalPrinter, GlobalFramework.WorkSessionPeriodDay);
                }
            }
            //Start WorkSessionPeriodDay
            else
            {
                bool result = ProcessWorkSessionPeriod.SessionPeriodOpen(WorkSessionPeriodType.Day);
                if (result)
                {
                    _touchButtonStartStopWorkSessionPeriodDay.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_close_day");
                    _touchButtonCashDrawer.Sensitive = true;
                }
            }
        }

        private bool CanCloseWorkSessionPeriodDay()
        {
            //Check if has Working Open Orders/Tables
            XPSelectData xPSelectDataTables = ProcessWorkSessionPeriod.GetOpenOrderTables();
            int noOfOpenOrderTables = xPSelectDataTables.Data.Length;
            if (noOfOpenOrderTables > 0)
            {
                string openOrderTables = string.Empty;
                pos_configurationplacetable currentOpenOrderTable;
                Guid tableOid = Guid.Empty;
                foreach (SelectStatementResultRow row in xPSelectDataTables.Data)
                {
                    tableOid = new Guid(row.Values[xPSelectDataTables.GetFieldIndex("PlaceTable")].ToString());
                    currentOpenOrderTable = GlobalFramework.SessionXpo.GetObjectByKey<pos_configurationplacetable>(tableOid);
                    openOrderTables += string.Format("{0}{1}", currentOpenOrderTable.Designation, " ");
                }

                ResponseType dialogResponse = Utils.ShowMessageTouch(
                  this,
                  DialogFlags.Modal,
                  new Size(620, 300),
                  MessageType.Error,
                  ButtonsType.Close,
                  resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"),
                  string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_worksession_period_warning_open_orders_tables"), noOfOpenOrderTables, string.Format("{0}{1}", Environment.NewLine, openOrderTables))
                );

                //Exit Event Button Without Close Cash Drwawer
                return false;
            }

            //Check if has Working Terminal Sessions
            XPSelectData xPSelectDataTerminals = ProcessWorkSessionPeriod.GetSessionPeriodOpenTerminalSessions();
            int noOfTerminalOpenSessions = xPSelectDataTerminals.Data.Length;
            if (noOfTerminalOpenSessions > 0)
            {
                string openTerminals = string.Empty;
                pos_configurationplaceterminal currentOpenSessionTerminal;
                Guid terminalOid = Guid.Empty;
                foreach (SelectStatementResultRow row in xPSelectDataTerminals.Data)
                {
                    terminalOid = new Guid(row.Values[xPSelectDataTerminals.GetFieldIndex("Terminal")].ToString());
                    currentOpenSessionTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_configurationplaceterminal>(terminalOid);
                    openTerminals += string.Format("{0}{1} - {2}", Environment.NewLine, currentOpenSessionTerminal.Designation, row.Values[xPSelectDataTerminals.GetFieldIndex("Designation")].ToString());
                }

////Check if Has Opened Terminal Connections before Close Day
//PosMessageDialog messageDialog = new PosMessageDialog(
//    this,
//    DialogFlags.DestroyWithParent,
//    new Size(600, 300),
//    string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_worksession_period_warning_open_terminals, noOfTerminalOpenSessions, string.Format("{0}{1}", Environment.NewLine, openTerminals)),
//    MessageType.Warning,
//    ResponseType.Ok,
//    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_ok
//);

//int messageDialogResponse = messageDialog.Run();
//messageDialog.Destroy();

                ResponseType responseType = Utils.ShowMessageTouch(this, DialogFlags.Modal, new Size(600, 400), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"),
                    string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_worksession_period_warning_open_terminals"), noOfTerminalOpenSessions, string.Format("{0}{1}", Environment.NewLine, openTerminals))
                );

                if (responseType == ResponseType.Yes) {
                    return Utils.CloseAllOpenTerminals(this, GlobalFramework.SessionXpo);

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
        void _touchButtonCashDrawer_Clicked(object sender, EventArgs e)
        {
            bool result;
            //Update UI
            GlobalApp.WindowPos.UpdateWorkSessionUI();
           

            //ProcessWorkSessionPeriod.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodTerminal, true );
            PosCashDrawerDialog dialogCashDrawer = new PosCashDrawerDialog(this, DialogFlags.DestroyWithParent);

            int response = dialogCashDrawer.Run();
            if (response == (int)ResponseType.Ok)
            {
                //Get Fresh XPO Objects, Prevent Deleted Object Bug
                pos_worksessionperiod workSessionPeriodDay = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);
                pos_worksessionperiod workSessionPeriodTerminal = GlobalFramework.WorkSessionPeriodTerminal;
                decimal addedMoney = 0.00m;
                var originalMovType = dialogCashDrawer.MovementType;

                bool newMoviment = false;

                switch (dialogCashDrawer.MovementType.Token)
                {
                    case "CASHDRAWER_OPEN":
                        newMoviment = false;
                        //Start Terminal Period
                        result = ProcessWorkSessionPeriod.SessionPeriodOpen(WorkSessionPeriodType.Terminal, dialogCashDrawer.MovementDescription);
                 
                        if (result)
                        {
                            //Update UI
                            GlobalApp.WindowPos.UpdateWorkSessionUI();
                            GlobalApp.WindowPos.TicketList.UpdateOrderStatusBar();

                            //Here we already have GlobalFramework.WorkSessionPeriodTerminal, assigned on ProcessWorkSessionPeriod.SessionPeriodStart
                            //Get Fresh XPO Objects, Prevent Deleted Object Bug
                            workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                             result = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              originalMovType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              dialogCashDrawer.TotalAmountInCashDrawer,
                              dialogCashDrawer.MovementDescription
                            );

                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, new Size(500, 280), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_cashdrawer_open_successfully"));

                            var pResponse = Utils.ShowMessageTouch(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print"),
                            resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_request_print_document_confirmation"));

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.WindowPos.TouchButtonPosToolbarNewFinanceDocument.Sensitive = false;
                            //Show ClosePeriodMessage
                            //ShowClosePeriodMessage(dialogCashDrawer, workSessionPeriodTerminal);
                            if (pResponse == ResponseType.Yes)
                            {
                                //PrintWorkSessionMovement
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_terminal_open"), 0.0m, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);
                            }
                            //Enable UI Buttons When Have Open Session
                            GlobalApp.WindowPos.TouchButtonPosToolbarNewFinanceDocument.Sensitive = true;

                        }
                        else
                        {
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_contact_support"));
                        }


                        //Check if is added money
                        if (dialogCashDrawer.MovementAmountMoney >= 0 && dialogCashDrawer.MovementAmountMoney != dialogCashDrawer.TotalAmountInCashDrawer)
                        {
                            //if 1 ADD if -1 REMOVE
                            int addRemoveMoney = 1;
                            //Add or Remove Text String for print ticket
                            string moneyInOutLabel = "";
                            string moneyInOutLabelAudit = "";
                            string audit = "";

                            //Added money to total amount
                            if (dialogCashDrawer.TotalAmountInCashDrawer > dialogCashDrawer.MovementAmountMoney)
                            {
                                addedMoney = dialogCashDrawer.TotalAmountInCashDrawer - dialogCashDrawer.MovementAmountMoney;
                                addRemoveMoney = -1;
                                moneyInOutLabel = "ticket_title_worksession_money_out";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_out";
                                audit = "CASHDRAWER_OUT";
                                dialogCashDrawer.MovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), Guid.Parse("069564cb-074a-4c91-931e-554454b1ab7e"));
                            }
                            else
                            {
                                addedMoney = dialogCashDrawer.MovementAmountMoney - dialogCashDrawer.TotalAmountInCashDrawer;
                                addRemoveMoney = 1;
                                moneyInOutLabel = "ticket_title_worksession_money_in";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_in";
                                audit = "CASHDRAWER_IN";
                                dialogCashDrawer.MovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), Guid.Parse("2ef29ce6-314c-4f40-897f-e31802dbeef3"));
                            }
                            //Total = IN
                            dialogCashDrawer.TotalAmountInCashDrawer = dialogCashDrawer.MovementAmountMoney;

                            // GlobalFramework.WorkSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);

                            workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                            var resultProcess = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              dialogCashDrawer.MovementType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              (addRemoveMoney * addedMoney),
                              dialogCashDrawer.MovementDescription
                            );

                            if (resultProcess)
                            {

                                //PrintCashDrawerOpenAndMoneyInOut
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);

                                //Open CashDrawer
                                var resultOpenDoor = PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                                if (!resultOpenDoor)
                                {
                                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                                }
                                else
                                {
                                    //Audit
                                    FrameworkUtils.Audit(audit, string.Format(
                                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], moneyInOutLabelAudit),
                                        FrameworkUtils.DecimalToStringCurrency(addedMoney),
                                        dialogCashDrawer.MovementDescription)
                                    );
                                }
                     
                                newMoviment = true;
                            }
                        }


                        break;

                    case "CASHDRAWER_CLOSE":
                        
                        newMoviment = false;

                        //Check if is added money
                        if (dialogCashDrawer.MovementAmountMoney >= 0 && dialogCashDrawer.MovementAmountMoney != dialogCashDrawer.TotalAmountInCashDrawer)
                        {
                            //if 1 ADD if -1 REMOVE
                            int addRemoveMoney = 1;
                            //Add or Remove Text String for print ticket
                            string moneyInOutLabel = "";
                            string moneyInOutLabelAudit = "";
                            string audit = "";

                            //Added money to total amount
                            if (dialogCashDrawer.TotalAmountInCashDrawer > dialogCashDrawer.MovementAmountMoney)
                            {
                                addedMoney = dialogCashDrawer.TotalAmountInCashDrawer - dialogCashDrawer.MovementAmountMoney;
                                addRemoveMoney = -1;
                                moneyInOutLabel = "ticket_title_worksession_money_out";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_out";
                                audit = "CASHDRAWER_OUT";
                                dialogCashDrawer.MovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), Guid.Parse("069564cb-074a-4c91-931e-554454b1ab7e"));
                            }
                            else
                            {
                                addedMoney = dialogCashDrawer.MovementAmountMoney - dialogCashDrawer.TotalAmountInCashDrawer;
                                addRemoveMoney = 1;
                                moneyInOutLabel = "ticket_title_worksession_money_in";
                                moneyInOutLabelAudit = "audit_message_cashdrawer_in";
                                audit = "CASHDRAWER_IN";
                                dialogCashDrawer.MovementType = (pos_worksessionmovementtype)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(pos_worksessionmovementtype), Guid.Parse("2ef29ce6-314c-4f40-897f-e31802dbeef3"));

                            }

                            //Total = IN
                            dialogCashDrawer.TotalAmountInCashDrawer = dialogCashDrawer.MovementAmountMoney;

                            // GlobalFramework.WorkSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);

                            workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                            var resultProcess = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              dialogCashDrawer.MovementType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              (addRemoveMoney * addedMoney),
                              dialogCashDrawer.MovementDescription
                            );

                            if (resultProcess)
                            {

                                //PrintCashDrawerOpenAndMoneyInOut
                                FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], moneyInOutLabel), addedMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);

                                //Open CashDrawer
                                var resultOpenDoor = PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                                if (!resultOpenDoor)
                                {
                                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                                }
                                else
                                {
                                    //Audit
                                    FrameworkUtils.Audit(audit, string.Format(
                                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], moneyInOutLabelAudit),
                                        FrameworkUtils.DecimalToStringCurrency(addedMoney),
                                        dialogCashDrawer.MovementDescription)
                                    );
                                }

                      
                                newMoviment = true;
                            }
                        }
                        //workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                        //Stop Terminal Period
                        result = ProcessWorkSessionPeriod.SessionPeriodClose(workSessionPeriodTerminal);

                        if (result)
                        {                              
                            //Update UI
                            GlobalApp.WindowPos.UpdateWorkSessionUI();
                            GlobalApp.WindowPos.LabelCurrentTable.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "status_message_open_cashdrawer");

                            //Get Fresh XPO Objects, Prevent Deleted Object Bug
                            workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(workSessionPeriodTerminal.Oid);

                            //dialogCashDrawer.TotalAmountInCashDrawer -= dialogCashDrawer.MovementAmountMoney;

       
                            //Add CASHDRAWER_CLOSE Movement to Day Period
                            result = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              originalMovType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              dialogCashDrawer.MovementAmountMoney,
                              dialogCashDrawer.MovementDescription
                            );

                            var pResponse = Utils.ShowMessageTouch(
                            this, DialogFlags.Modal, new Size(500, 350), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_print"),
                            resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_request_print_document_confirmation"));

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.WindowPos.TouchButtonPosToolbarNewFinanceDocument.Sensitive = false;
                            //Show ClosePeriodMessage
                            ShowClosePeriodMessage(dialogCashDrawer, workSessionPeriodTerminal);

                            if (pResponse == ResponseType.Yes)
                            {
                                //PrintWorkSessionMovement
                                FrameworkCalls.PrintWorkSessionMovement(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, workSessionPeriodTerminal);
                            }
     
                            //GlobalFramework.WorkSessionPeriodTerminal = null;
                        }
                        break;

                    case "CASHDRAWER_IN":

                        dialogCashDrawer.TotalAmountInCashDrawer += dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                        result = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                          workSessionPeriodTerminal,
                          dialogCashDrawer.MovementType,
                          GlobalFramework.LoggedUser,
                          GlobalFramework.LoggedTerminal,
                          FrameworkUtils.CurrentDateTimeAtomic(),
                          dialogCashDrawer.MovementAmountMoney,
                          dialogCashDrawer.MovementDescription
                        );

                        if (result)
                        {
                            //PrintCashDrawerOpenAndMoneyInOut
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_money_in"), dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);
                            //Open CashDrawer
                            var resultOpenDoor = PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                            if (!resultOpenDoor)
                            {
                                Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                            }
                            else
                            {
                                //Audit
                                FrameworkUtils.Audit("CASHDRAWER_IN", string.Format(
                                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_cashdrawer_in"),
                                    FrameworkUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney),
                                    dialogCashDrawer.MovementDescription)
                                );
                            }

                            //ShowMessage
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"));
                        }
                        else
                        {
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_contact_support"));
                        }
                        break;

                    //Work with Terminal Session
                    case "CASHDRAWER_OUT":

                        dialogCashDrawer.TotalAmountInCashDrawer -= dialogCashDrawer.MovementAmountMoney;

                        workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                        //In Period Terminal
                        result = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                          workSessionPeriodTerminal,
                          dialogCashDrawer.MovementType,
                          GlobalFramework.LoggedUser,
                          GlobalFramework.LoggedTerminal,
                          FrameworkUtils.CurrentDateTimeAtomic(),
                          -dialogCashDrawer.MovementAmountMoney,
                          dialogCashDrawer.MovementDescription
                        );

                        if (result)
                        {
                            //PrintCashDrawerOpenAndMoneyInOut
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_money_out"), dialogCashDrawer.MovementAmountMoney, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);
                            //Open CashDrawer
                            var resultOpenDoor = PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                            if (!resultOpenDoor)
                            {
                                Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "open_cash_draw_permissions")));
                            }
                            else
                            {
                                //Audit
                                FrameworkUtils.Audit("CASHDRAWER_OUT", string.Format(
                                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_cashdrawer_out"),
                                    FrameworkUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney),
                                    dialogCashDrawer.MovementDescription)
                                );
                            }
               
                            //ShowMessage
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, new Size(500, 300), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"));
                        }
                        else
                        {
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_contact_support"));
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
            //_log.Debug(string.Format("ProcessWorkSessionPeriod: [{0}]", ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount(GlobalFramework.WorkSessionPeriodDay)));
            //if (GlobalFramework.WorkSessionPeriodDay != null) ProcessWorkSessionPeriod.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodDay, true);
            //if (GlobalFramework.WorkSessionPeriodTerminal != null) ProcessWorkSessionPeriod.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodTerminal, true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        public void UpdateButtons()
        {
            //Update Toolbar UI Buttons After ToolBox and ToolBar
            if (GlobalFramework.WorkSessionPeriodDay != null)
            {
                //With Valid WorkSessionPeriodDay
                if (GlobalFramework.WorkSessionPeriodDay.SessionStatus == WorkSessionPeriodStatus.Open)
                {
                    if (_touchButtonStartStopWorkSessionPeriodDay.LabelText != resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_close_day"))
                    {
                        _touchButtonStartStopWorkSessionPeriodDay.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_close_day");
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
                if (_touchButtonStartStopWorkSessionPeriodDay.LabelText != resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_day"))
                {
                    _touchButtonStartStopWorkSessionPeriodDay.LabelText = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_worksession_open_day");
                }
                _touchButtonCashDrawer.Sensitive = false;
            }
        }

        /// <summary>
        /// Show Close Message, Shared for Day and Terminal Sessions
        /// </summary>
        /// <param name="pWorkSessionPeriod"></param>
        /// <returns></returns>
        public void ShowClosePeriodMessage(Window pSourceWindow, pos_worksessionperiod pWorkSessionPeriod)
        {
            string messageResource = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day) ?
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_worksession_day_close_successfully") :
              resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_worksession_terminal_close_successfully")
            ;
            string messageTotalSummary = string.Empty;
            //used to store number of payments used, to increase dialog window size
            int workSessionPeriodTotalCount = 0;
            //Window Height Helper vars  
            int lineHeight = 28;
            int windowHeight = 300;
            pWorkSessionPeriod.DateEnd = DateTime.Now;
            //Get Session Period Details
            Hashtable resultHashTable = ProcessWorkSessionPeriod.GetSessionPeriodSummaryDetails(pWorkSessionPeriod);
            //Get Total Money in CashDrawer On Open/Close
            string totalMoneyInCashDrawerOnOpen = string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_cashdrawer_on_open"), FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawerOnOpen"]));
            string totalMoneyInCashDrawer = string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_cashdrawer"), FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyInCashDrawer"]));
            //Get Total Money and TotalMoney Out (NonPayments)
            string totalMoneyIn = string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_cashdrawer_money_in"), FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyIn"]));
            string totalMoneyOut = string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_cashdrawer_money_out"), FrameworkUtils.DecimalToStringCurrency((decimal)resultHashTable["totalMoneyOut"]));
            //Init Message
            messageTotalSummary = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}", Environment.NewLine, totalMoneyInCashDrawerOnOpen, totalMoneyInCashDrawer, totalMoneyIn, totalMoneyOut);

            //Get Payments Totals
            try
            {
                XPCollection workSessionPeriodTotal = ProcessWorkSessionPeriod.GetSessionPeriodTotal(pWorkSessionPeriod);
                if (workSessionPeriodTotal.Count > 0)
                {
                    messageTotalSummary += string.Format("{0}{1}{0}", Environment.NewLine, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total_by_type_of_payment"));
                    foreach (pos_worksessionperiodtotal item in workSessionPeriodTotal)
                    {
                        messageTotalSummary += string.Format("{1}-{2}: {3}{0}", Environment.NewLine, item.PaymentMethod.Acronym, item.PaymentMethod.Designation, FrameworkUtils.DecimalToStringCurrency(item.Total));
                    }
                    workSessionPeriodTotalCount = workSessionPeriodTotal.Count;
                }

                windowHeight = (workSessionPeriodTotalCount > 0) ? windowHeight + ((workSessionPeriodTotalCount + 2) * lineHeight) : windowHeight + lineHeight;

                Utils.ShowMessageTouch(
                  pSourceWindow,
                  DialogFlags.Modal,
                  new Size(600, windowHeight),
                  MessageType.Info,
                  ButtonsType.Close,
                  "Info",
                  string.Format(messageResource, messageTotalSummary)
                );
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
