using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using System;
using System.Collections;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.WorkSession
{
    public enum MovementTypeTotal
    {
        None, All, AllNonMoney,
        Money, BankCheck, CashMachine, Credit, Visa, CurrentAccount,
        MoneyInCashDrawer, MoneyIn, MoneyOut
    }

    public class ProcessWorkSessionPeriod
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static pos_worksessionperiod GetSessionPeriod(WorkSessionPeriodType pWorkSessionPeriodType)
        {
            string whereTerminal = String.Empty;
            if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
            {
                whereTerminal = string.Format("Terminal = '{0}' AND ", GlobalFramework.LoggedTerminal.Oid);
            }

            string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE {1}PeriodType = '{0}' AND SessionStatus = 0;", Convert.ToInt16(pWorkSessionPeriodType), whereTerminal);
            Guid workSessionPeriodOid = FrameworkUtils.GetGuidFromQuery(sql);
            if (workSessionPeriodOid != Guid.Empty)
            {
                pos_worksessionperiod resultWorkSessionPeriod = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(workSessionPeriodOid);
                //Add Parent Reference, not used because we use GlobalFramework.WorkSessionPeriodDay and not GlobalFramework.WorkSessionPeriodTerminal.Parent
                if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
                {
                    //Child > Parent Reference
                    resultWorkSessionPeriod.Parent = GlobalFramework.WorkSessionPeriodDay;
                }

                return resultWorkSessionPeriod;
            }
            else
            {
                return null;
            }
        }

        public static bool SessionPeriodOpen(WorkSessionPeriodType pWorkSessionPeriodType, string pDescription = "")
        {
            try
            {
                string periodType = (pWorkSessionPeriodType == WorkSessionPeriodType.Day) ? "Day" : "Terminal";
                string description = (pDescription != String.Empty) ? string.Format(" - {0}", pDescription) : String.Empty;
                pos_configurationplaceterminal terminal = GlobalFramework.SessionXpo.GetObjectByKey<pos_configurationplaceterminal>(GlobalFramework.LoggedTerminal.Oid);
                DateTime dateTime = FrameworkUtils.CurrentDateTimeAtomic();

                pos_worksessionperiod workSessionPeriod = new pos_worksessionperiod(GlobalFramework.SessionXpo)
                {
                    PeriodType = pWorkSessionPeriodType,
                    SessionStatus = WorkSessionPeriodStatus.Open,
                    Designation = string.Format("{0} - {1}{2}", periodType, dateTime.ToString(SettingsApp.DateTimeFormat), description),
                    DateStart = dateTime,
                    Terminal = terminal
                };
                //Assign Parent
                if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
                {
                    workSessionPeriod.Parent = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(GlobalFramework.WorkSessionPeriodDay.Oid);
                }
                //Persist
                workSessionPeriod.Save();

                if (pWorkSessionPeriodType == WorkSessionPeriodType.Day)
                {
                    GlobalFramework.WorkSessionPeriodDay = workSessionPeriod;
                }
                else
                {
                    GlobalFramework.WorkSessionPeriodTerminal = workSessionPeriod;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Close WorkSessionPeriod, work in all PeriodTypes Day and Terminal
        /// </summary>
        /// <param name="pWorkSessionPeriod"></param>
        public static bool SessionPeriodClose(pos_worksessionperiod pWorkSessionPeriod)
        {
            bool result = false;

            //Store Totals first, with a UOF, Error return without close session, we process error outside
            bool resultPersistTotals = ProcessWorkSessionPeriod.PersistWorkSessionTotals(pWorkSessionPeriod);

            if (resultPersistTotals)
            {
                try
                {
                    pos_worksessionperiod workSessionPeriod = GlobalFramework.SessionXpo.GetObjectByKey<pos_worksessionperiod>(pWorkSessionPeriod.Oid);
                    DateTime dateTime = FrameworkUtils.CurrentDateTimeAtomic();
                    workSessionPeriod.DateEnd = dateTime;
                    workSessionPeriod.SessionStatus = WorkSessionPeriodStatus.Close;
                    workSessionPeriod.Save();

                    //Assign to Singleton
                    if (workSessionPeriod.PeriodType == WorkSessionPeriodType.Day)
                    {
                        GlobalFramework.WorkSessionPeriodDay = workSessionPeriod;
                    }
                    else
                    {
                        GlobalFramework.WorkSessionPeriodTerminal = workSessionPeriod;
                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
            return result;
        }

        public static XPCollection GetSessionPeriodTotal(pos_worksessionperiod pWorkSessionPeriod)
        {
            return GetSessionPeriodTotal(GlobalFramework.SessionXpo, pWorkSessionPeriod);
        }

        public static XPCollection GetSessionPeriodTotal(Session pSession, pos_worksessionperiod pWorkSessionPeriod)
        {
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Period = '{0}'", pWorkSessionPeriod.Oid));
            SortProperty sortProperty = new SortProperty("Ord", SortingDirection.Ascending);
            XPCollection resultXPCollection = new XPCollection(pSession, typeof(pos_worksessionperiodtotal), criteria, sortProperty);

            return resultXPCollection;
        }

        public static bool PersistWorkSessionTotals(pos_worksessionperiod pWorkSessionPeriod)
        {
            try
            {
                //Start UnitOfWork
                using (UnitOfWork uowSession = new UnitOfWork())
                {
                    uint paymentMethodOrd = 0;
                    string paymentMethodToken = String.Empty;
                    MovementTypeTotal movementTypeTotal = MovementTypeTotal.None;
                    pos_worksessionperiod workSessionPeriod;
                    fin_configurationpaymentmethod configurationPaymentMethod;
                    pos_worksessionperiodtotal workSessionPeriodTotal;
                    //Can filter by Day or Terminal type
                    string wherePeriodField = (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Terminal) ? "wspPeriod" : "wspPeriodParent";

                    string sql = string.Format(@"
                        SELECT 
                              DISTINCT(cpmPaymentMethodToken) as Token,
                              cpmPaymentMethodOrd as Ord
                        FROM 
                            view_worksessionmovement 
                        WHERE 
                            cpmPaymentMethodToken IS NOT NULL AND {0} = '{1}'
                        ORDER 
                            BY cpmPaymentMethodOrd
                        ;"
                      , wherePeriodField
                      , pWorkSessionPeriod.Oid
                    );

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(uowSession, sql);
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        paymentMethodOrd = Convert.ToUInt16(row.Values[xPSelectData.GetFieldIndex("Ord")]);
                        paymentMethodToken = row.Values[xPSelectData.GetFieldIndex("Token")].ToString();

                        switch (paymentMethodToken)
                        {
                            case "MONEY":
                                movementTypeTotal = MovementTypeTotal.Money;
                                break;
                            case "BANK_CHECK":
                                movementTypeTotal = MovementTypeTotal.BankCheck;
                                break;
                            case "CASH_MACHINE":
                                movementTypeTotal = MovementTypeTotal.CashMachine;
                                break;
                            case "CREDIT_CARD":
                                movementTypeTotal = MovementTypeTotal.Credit;
                                break;
                            case "VISA":
                                movementTypeTotal = MovementTypeTotal.Visa;
                                break;
                            case "CURRENT_ACCOUNT":
                                movementTypeTotal = MovementTypeTotal.CurrentAccount;
                                break;
                        }

                        if (movementTypeTotal != MovementTypeTotal.None)
                        {
                            //Get XPObjects
                            workSessionPeriod = uowSession.GetObjectByKey<pos_worksessionperiod>(pWorkSessionPeriod.Oid);
                            configurationPaymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObjectFromField(uowSession, typeof(fin_configurationpaymentmethod), "Token", paymentMethodToken);

                            //Persist WorkSessionPeriodTotal
                            workSessionPeriodTotal = new pos_worksessionperiodtotal(uowSession)
                            {
                                Ord = paymentMethodOrd,
                                PaymentMethod = configurationPaymentMethod,
                                Total = GetSessionPeriodMovementTotal(workSessionPeriod, movementTypeTotal),
                                Period = workSessionPeriod
                            };
                        }
                    };

                    try
                    {
                        //Commit UOW Changes
                        uowSession.CommitChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        //Rollback
                        uowSession.RollbackTransaction();
                        _log.Error(ex.Message, ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Get total number of Opened Terminal Period Sessions
        /// </summary>
        public static XPSelectData GetSessionPeriodOpenTerminalSessions()
        {
            try
            {
                //string sql = string.Format(@"SELECT Count(*) as Count FROM pos_worksessionperiod WHERE Parent = '{0}' AND SessionStatus = 0;", GlobalFramework.WorkSessionPeriodDay.Oid.ToString());
                string sql = string.Format(@"SELECT Oid, Designation, DateStart, Terminal FROM pos_worksessionperiod WHERE PeriodType = 1 AND Parent = '{0}' AND SessionStatus = 0;", GlobalFramework.WorkSessionPeriodDay.Oid.ToString());
                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                return xPSelectData;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Get total number of Opened Orders
        /// </summary>
        public static XPSelectData GetOpenOrderTables()
        {
            try
            {
                string sql = string.Format(@"SELECT PlaceTable FROM fin_documentordermain WHERE OrderStatus = 1;");
                XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sql);
                return xPSelectData;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return null;
            }
        }

        public static decimal GetSessionPeriodMovementTotal(pos_worksessionperiod pWorkSessionPeriod, MovementTypeTotal pMovementTypeTotal)
        {
            return GetSessionPeriodMovementTotal(GlobalFramework.SessionXpo, pWorkSessionPeriod, pMovementTypeTotal);
        }

        public static decimal GetSessionPeriodMovementTotal(Session pSession, pos_worksessionperiod pWorkSessionPeriod, MovementTypeTotal pMovementTypeTotal)
        {
            string sqlShared = @"
                SELECT 
                    SUM(wsmMovementAmount) AS Total 
                FROM 
                    view_worksessionmovement 
                WHERE 
                    ((dfmDocumentStatusStatus IS NULL OR dfmDocumentStatusStatus <> 'A') AND (dfpPaymentStatus IS NULL OR dfpPaymentStatus <> 'A')) 
                    AND
                    {0}
                ;"
            ;

            decimal resultTotal = 0.0m;
            string sqlWhereTypeTotal = String.Empty;
            string sqlWherePeriod = String.Empty;

            if (pMovementTypeTotal != MovementTypeTotal.None)
            {
                try
                {
                    switch (pMovementTypeTotal)
                    {
                        case MovementTypeTotal.All:
                            sqlWhereTypeTotal = @"((cpmPaymentMethodToken = 'MONEY' AND wmtMovementTypeToken <> 'CASHDRAWER_OPEN' AND wmtMovementTypeToken <> 'FINANCE_DOCUMENT')) OR (cpmPaymentMethodToken <> 'MONEY')";
                            break;
                        case MovementTypeTotal.AllNonMoney:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND wmtMovementTypeToken <> 'FINANCE_DOCUMENT')";
                            break;
                        case MovementTypeTotal.Money:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken = 'MONEY' AND wmtMovementTypeToken <> 'FINANCE_DOCUMENT')";
                            break;
                        //Total Money Ins and Outs, includes Money Payments, and CashOpen 
                        case MovementTypeTotal.MoneyInCashDrawer:
                            //sqlWhereTypeTotal = @"(wmtMovementTypeToken = 'CASHDRAWER_IN' OR wmtMovementTypeToken = 'CASHDRAWER_OUT')";
                            sqlWhereTypeTotal = @"(wmtMovementTypeToken = 'CASHDRAWER_OPEN' OR wmtMovementTypeToken = 'CASHDRAWER_IN' OR wmtMovementTypeToken = 'CASHDRAWER_OUT')";
                            break;
                        //Total Money Ins and Outs (Without Sells)
                        case MovementTypeTotal.MoneyIn:
                            sqlWhereTypeTotal = @"(wmtMovementTypeToken = 'CASHDRAWER_IN' AND wsmPaymentMethod IS NULL)";
                            break;
                        case MovementTypeTotal.MoneyOut:
                            sqlWhereTypeTotal = @"(wmtMovementTypeToken = 'CASHDRAWER_OUT' AND wsmPaymentMethod IS NULL)";
                            break;
                        case MovementTypeTotal.BankCheck:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND cpmPaymentMethodToken = 'BANK_CHECK')";
                            break;
                        case MovementTypeTotal.CashMachine:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND cpmPaymentMethodToken = 'CASH_MACHINE')";
                            break;
                        case MovementTypeTotal.Credit:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND cpmPaymentMethodToken = 'CREDIT_CARD')";
                            break;
                        case MovementTypeTotal.Visa:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND cpmPaymentMethodToken = 'VISA')";
                            break;
                        case MovementTypeTotal.CurrentAccount:
                            sqlWhereTypeTotal = @"(cpmPaymentMethodToken <> 'MONEY' AND cpmPaymentMethodToken = 'CURRENT_ACCOUNT')";
                            break;
                        default:
                            break;
                    }

                    switch (pWorkSessionPeriod.PeriodType)
                    {
                        case WorkSessionPeriodType.Day:
                            //sqlWherePeriod = string.Format(@"AND wspPeriodStatus = 0 AND (wspPeriodParent = '{0}')", pWorkSessionPeriod.Oid);
                            sqlWherePeriod = string.Format(@"AND (wspPeriodParent = '{0}')", pWorkSessionPeriod.Oid);
                            break;
                        case WorkSessionPeriodType.Terminal:
                            //sqlWherePeriod = string.Format(@"AND wspPeriodStatus = 0 AND (wspPeriod = '{0}')", pWorkSessionPeriod.Oid/*GlobalFramework.WorkSessionPeriodTerminal.Oid*/);
                            sqlWherePeriod = string.Format(@"AND (wspPeriod = '{0}')", pWorkSessionPeriod.Oid/*GlobalFramework.WorkSessionPeriodTerminal.Oid*/);
                            break;
                        default:
                            break;
                    }

                    string sql = string.Format(sqlShared, string.Format("(({0}) {1})", sqlWhereTypeTotal, sqlWherePeriod));
                    resultTotal = Convert.ToDecimal(pSession.ExecuteScalar(sql));
                    //_log.Debug(string.Format("pMovementTypeTotal: [{0}], resultTotal: [{1}], sql: [{2}]", pMovementTypeTotal, resultTotal, sql));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
            return resultTotal;
        }

        public static string GetSessionPeriodMovementTotalDebug(pos_worksessionperiod pWorkSessionPeriod, bool pOutputToLog)
        {
            string result = String.Empty;
            string resultDetail = string.Format("WorkSessionPeriod:[{0}] Type:[{1}]", pWorkSessionPeriod.Oid, pWorkSessionPeriod.PeriodType);
            string resultValues = String.Empty;
            string resultFields = String.Empty;

            foreach (MovementTypeTotal typeTotal in Enum.GetValues(typeof(MovementTypeTotal)))
            {
                resultFields += string.Format("{0}\t", typeTotal);
                resultValues += string.Format("{0}\t", GetSessionPeriodMovementTotal(pWorkSessionPeriod, typeTotal).ToString());
            }

            result = string.Format("{0}{1}{2}{3}{4}{5}{6}", Environment.NewLine, resultDetail, Environment.NewLine, resultFields, Environment.NewLine, resultValues, Environment.NewLine);
            if (pOutputToLog) _log.Debug(result);

            return result;
        }

        public static decimal GetSessionPeriodCashDrawerOpenOrCloseAmount(String pMoventTypeToken)
        {
            return GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(GlobalFramework.SessionXpo, GlobalFramework.WorkSessionPeriodTerminal, pMoventTypeToken);
        }

        public static decimal GetSessionPeriodCashDrawerOpenOrCloseAmount(pos_worksessionperiod pWorkSessionPeriod, String pMoventTypeToken)
        {
            return GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(GlobalFramework.SessionXpo, pWorkSessionPeriod, pMoventTypeToken);
        }

        /// <summary>
        /// Get Last CASHDRAWER_OPEN/CASHDRAWER_CLOSE Value, Required WorkSessionPeriod, to Re-Print Sessions from Other Terminals, do not use GlobalFramework.LoggedTerminal.Oid
        /// </summary>
        public static decimal GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(Session pSession, pos_worksessionperiod pWorkSessionPeriod, String pMoventTypeToken)
        {
            //If Has a Valid pWorkSessionPeriod get Terminal from it, else use logged Terminal
            Guid whereTerminalGuid = (pWorkSessionPeriod != null) ? pWorkSessionPeriod.Terminal.Oid : GlobalFramework.LoggedTerminal.Oid;

            //Required, CASHDRAWER_OPEN always comes from WorkSessionPeriod, and CASHDRAWER_CLOSE comes from latest cash close record (DESC)
            string whereOpen = (pMoventTypeToken == "CASHDRAWER_OPEN") ? string.Format(" AND wspPeriod = '{0}'", pWorkSessionPeriod.Oid) : String.Empty;

            string sql = string.Format(@"
                SELECT 
                    wsmMovementAmount  
                FROM 
                    view_worksessionmovement
                WHERE 
                    (cptTerminal = '{0}' AND wmtMovementTypeToken = '{1}'{2}) 
                ORDER BY 
                    wsmDate DESC;"
              , whereTerminalGuid
              , pMoventTypeToken
              , whereOpen
            );

            decimal resultAmount = Convert.ToDecimal(pSession.ExecuteScalar(sql));
            //_log.Debug(string.Format("pMoventTypeToken sql: [{0}]", sql));

            return resultAmount;
        }

        /// <summary>
        /// Get CashDrawer Open Amount for all Used terminals, Required three queries
        /// </summary>
        public static decimal GetSessionPeriodDayCashDrawerOpenOrCloseAmount(pos_worksessionperiod pWorkSessionPeriod, bool pEnableOpenMode)
        {
            decimal result = 0.0m;

            try
            {
                //1) Get Day CASHDRAWER_OPEN Terminals
                string sqlTerminals = string.Format(@"
                    SELECT 
                        DISTINCT(cptTerminal), cptTerminalOrd
                    FROM 
                        view_worksessionmovement 
                    WHERE 
                        wspPeriodParent = '{0}' AND 
                        wmtMovementTypeToken = 'CASHDRAWER_OPEN'
                    ORDER BY 
                        cptTerminalOrd
                ;"
                  , pWorkSessionPeriod.Oid
                );
                SelectedData xpoSelectedDataTerminals = GlobalFramework.SessionXpo.ExecuteQuery(sqlTerminals);

                //2) MODE OPEN - GET FIRST CashDrawer Period for WorkSession Day
                //2) MODE CLOSE - GET LAST CashDrawer Period for WorkSession Day
                //2) Diference is orderBy DESC(OPEN) or ASC(CLOSE)
                string orderBy = (pEnableOpenMode) ? "ASC" : "DESC";
                string currentTerminal;
                List<string> listPeriod = new List<string>();
                foreach (SelectStatementResultRow item in xpoSelectedDataTerminals.ResultSet[0].Rows)
                {
                    currentTerminal = item.Values[0].ToString();
                    string sqlPeriod = string.Format(@"
                    SELECT 
                        wspPeriod 
                    FROM 
                        view_worksessionmovement 
                    WHERE 
                        wspPeriodParent = '{0}' AND 
                        cptTerminal = '{1}' AND 
                        wmtMovementTypeToken = 'CASHDRAWER_OPEN' 
                    ORDER BY 
                        wsmDate {2}
                    ;"
                        , pWorkSessionPeriod.Oid
                        , currentTerminal
                        , orderBy
                    );
                    listPeriod.Add(GlobalFramework.SessionXpo.ExecuteScalar(sqlPeriod).ToString());
                }

                //If dont have periods return 0
                if (listPeriod.Count <= 0) return 0.0m;

                //3) Get CashDrawer Open Amount for all Terminal > Periods
                string sqlCashDrawerAmount = @"
                SELECT 
                    SUM(wsmMovementAmount) as Total 
                FROM 
                    view_worksessionmovement 
                WHERE 
                    (wmtMovementTypeToken = 'CASHDRAWER_OPEN'{0}) {1}
                ";
                //TODO: Remove This After Tests
                //Problems in SQLServer
                //ORDER BY 
                //  cptTerminalOrd

                int i = 0;
                string wherePeriod = String.Empty;
                string whereSeparator = String.Empty;
                foreach (string item in listPeriod)
                {
                    whereSeparator = (i > 0) ? " OR " : String.Empty;
                    wherePeriod += string.Format("{0}wspPeriod = '{1}'", whereSeparator, item);
                    i++;
                }

                //Shared for Both Modes
                wherePeriod = string.Format(" AND ({0})", wherePeriod);
                //Require to Add for OPEN Mode
                string whereClose = (!pEnableOpenMode) ? " OR wmtMovementTypeToken = 'CASHDRAWER_IN' OR wmtMovementTypeToken = 'CASHDRAWER_OUT'" : String.Empty;

                //Final Query
                sqlCashDrawerAmount = string.Format(sqlCashDrawerAmount, whereClose, wherePeriod);
                //_log.Debug(string.Format("sqlCashDrawerAmount: [{0}]", sqlCashDrawerAmount));
                var cashDrawerAmount = GlobalFramework.SessionXpo.ExecuteScalar(sqlCashDrawerAmount);
                result = (cashDrawerAmount != null) ? Convert.ToDecimal(cashDrawerAmount) : 0.0m;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static Hashtable GetSessionPeriodSummaryDetails(pos_worksessionperiod pWorkSessionPeriod)
        {
            //Get Total Money and TotalMoney Out (NonPayments)
            decimal totalMoneyIn = ProcessWorkSessionPeriod.GetSessionPeriodMovementTotal(pWorkSessionPeriod, MovementTypeTotal.MoneyIn);
            decimal totalMoneyOut = -ProcessWorkSessionPeriod.GetSessionPeriodMovementTotal(pWorkSessionPeriod, MovementTypeTotal.MoneyOut);

            //Get Total Money in Cash when Open/Close for Day and Terminals
            decimal totalMoneyInCashDrawerOnOpen;
            decimal totalMoneyInCashDrawer;
            if (pWorkSessionPeriod.PeriodType == WorkSessionPeriodType.Day)
            {
                totalMoneyInCashDrawerOnOpen = ProcessWorkSessionPeriod.GetSessionPeriodDayCashDrawerOpenOrCloseAmount(pWorkSessionPeriod, true);
                totalMoneyInCashDrawer = ProcessWorkSessionPeriod.GetSessionPeriodDayCashDrawerOpenOrCloseAmount(pWorkSessionPeriod, false);
            }
            else
            {
                totalMoneyInCashDrawerOnOpen = ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount(pWorkSessionPeriod, "CASHDRAWER_OPEN");
                //Get Total in CashDrawer On Close or on Working: Using latest CASHDRAWER_CLOSE get from terminal method if Terminal Session
                if (pWorkSessionPeriod.SessionStatus == WorkSessionPeriodStatus.Close)
                {
                    totalMoneyInCashDrawer = ProcessWorkSessionPeriod.GetSessionPeriodCashDrawerOpenOrCloseAmount(pWorkSessionPeriod, "CASHDRAWER_CLOSE");
                }
                else
                {
                    totalMoneyInCashDrawer = ProcessWorkSessionPeriod.GetSessionPeriodMovementTotal(pWorkSessionPeriod, MovementTypeTotal.MoneyInCashDrawer);
                }
            }

            Hashtable resultHashTable = new Hashtable();
            resultHashTable.Add("totalMoneyInCashDrawerOnOpen", totalMoneyInCashDrawerOnOpen);
            resultHashTable.Add("totalMoneyInCashDrawer", totalMoneyInCashDrawer);
            resultHashTable.Add("totalMoneyIn", totalMoneyIn);
            resultHashTable.Add("totalMoneyOut", totalMoneyOut);

            return resultHashTable;
        }

        //public static decimal GetLastPaymentMethodTotal(WorkSessionPeriodType pWorkSessionPeriodType, WorkSessionPeriod pWorkSessionPeriod, ConfigurationPaymentMethod pPaymentMethod)
        //{
        //  string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE SessionStatus = 1 AND PeriodType = {0} AND Terminal = '{1}' ORDER BY DateEnd DESC;", pWorkSessionPeriodType, GlobalFramework.LoggedTerminal.Oid);
        //  Guid workSessionPeriodOid = Utils.GetGuidFromQuery(sql);
        //  if (workSessionPeriodOid != Guid.Empty)
        //  {
        //    WorkSessionPeriod workSessionPeriod = GlobalFramework.SessionXpo.GetObjectByKey<WorkSessionPeriod>(workSessionPeriodOid);
        //    foreach (WorkSessionPeriodTotal item in workSessionPeriod.TotalPeriod)
        //    {
        //      _log.Debug(string.Format("Message: [{0}]", item.Total));
        //    }
        //    //WorkSessionPeriodTotal  WorkSessionPeriodTotal = GlobalFramework.SessionXpo.GetObjectByKey<WorkSessionPeriodTotal>(workSessionPeriodOid);
        //    return 1.0m;
        //  }
        //  else
        //  {
        //    return 0.0m;
        //  }
        //  return 1.0m;
        //}
    }
}
