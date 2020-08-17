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

        void _touchButtonCashDrawer_Clicked(object sender, EventArgs e)
        {
            bool result;

            //ProcessWorkSessionPeriod.GetSessionPeriodMovementTotalDebug(GlobalFramework.WorkSessionPeriodTerminal, true );
            PosCashDrawerDialog dialogCashDrawer = new PosCashDrawerDialog(this, DialogFlags.DestroyWithParent);

            int response = dialogCashDrawer.Run();
            if (response == (int)ResponseType.Ok)
            {
                //Get Fresh XPO Objects, Prevent Deleted Object Bug
                pos_worksessionperiod workSessionPeriodDay = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);
                pos_worksessionperiod workSessionPeriodTerminal;

                switch (dialogCashDrawer.MovementType.Token)
                {
                    case "CASHDRAWER_OPEN":

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
                              dialogCashDrawer.MovementType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              dialogCashDrawer.MovementAmountMoney,
                              dialogCashDrawer.MovementDescription
                            );
                        }

                        if (result)
                        {
                            //PrintWorkSessionMovement
                            FrameworkCalls.PrintCashDrawerOpenAndMoneyInOut(dialogCashDrawer, GlobalFramework.LoggedTerminal.ThermalPrinter, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "ticket_title_worksession_terminal_open"), 0.0m, dialogCashDrawer.TotalAmountInCashDrawer, dialogCashDrawer.MovementDescription);

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.WindowPos.TouchButtonPosToolbarNewFinanceDocument.Sensitive = true;
                            //Open CashDrawer
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, new Size(500, 280), MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_cashdrawer_open_successfully"));
                        }
                        else
                        {
                            Utils.ShowMessageTouch(dialogCashDrawer, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_contact_support"));
                        }
                        break;

                    case "CASHDRAWER_CLOSE":

                        //Stop Terminal Period
                        result = ProcessWorkSessionPeriod.SessionPeriodClose(GlobalFramework.WorkSessionPeriodTerminal);

                        if (result)
                        {
                            //Update UI
                            GlobalApp.WindowPos.UpdateWorkSessionUI();
                            GlobalApp.WindowPos.LabelCurrentTable.Text = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "status_message_open_cashdrawer");

                            //Get Fresh XPO Objects, Prevent Deleted Object Bug
                            workSessionPeriodTerminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodTerminal.Oid);

                            //Add CASHDRAWER_CLOSE Movement to Day Period
                            result = ProcessWorkSessionMovement.PersistWorkSessionMovement(
                              workSessionPeriodTerminal,
                              dialogCashDrawer.MovementType,
                              GlobalFramework.LoggedUser,
                              GlobalFramework.LoggedTerminal,
                              FrameworkUtils.CurrentDateTimeAtomic(),
                              dialogCashDrawer.MovementAmountMoney,
                              dialogCashDrawer.MovementDescription
                            );

                            //PrintWorkSessionMovement
                            FrameworkCalls.PrintWorkSessionMovement(dialogCashDrawer, GlobalFramework.LoggedTerminal.Printer, GlobalFramework.WorkSessionPeriodTerminal);

                            //Enable UI Buttons When Have Open Session
                            GlobalApp.WindowPos.TouchButtonPosToolbarNewFinanceDocument.Sensitive = false;
                            //Show ClosePeriodMessage
                            ShowClosePeriodMessage(dialogCashDrawer, GlobalFramework.WorkSessionPeriodTerminal);
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
                            PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                            //Audit
                            FrameworkUtils.Audit("CASHDRAWER_IN", string.Format(
                                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_cashdrawer_in"), 
                                FrameworkUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney), 
                                dialogCashDrawer.MovementDescription)
                            );

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
                            PrintRouter.OpenDoor(GlobalFramework.LoggedTerminal.Printer);
                            //Audit
                            FrameworkUtils.Audit("CASHDRAWER_OUT", string.Format(
                                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "audit_message_cashdrawer_out"), 
                                FrameworkUtils.DecimalToStringCurrency(dialogCashDrawer.MovementAmountMoney), 
                                dialogCashDrawer.MovementDescription)
                            );
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
