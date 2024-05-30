using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Data.Enums;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LogicPOS.Data.Services
{
    public class WorkSessionProcessor
    {
        public static pos_worksessionperiod GetSessionPeriod(WorkSessionPeriodType pWorkSessionPeriodType)
        {
            string whereTerminal = string.Empty;
            if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
            {
                whereTerminal = string.Format("Terminal = '{0}' AND ", TerminalSettings.LoggedTerminal.Oid);
            }

            string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE {1}PeriodType = '{0}' AND SessionStatus = 0;", Convert.ToInt16(pWorkSessionPeriodType), whereTerminal);
            Guid workSessionPeriodOid = XPOHelper.GetGuidFromQuery(sql);
            if (workSessionPeriodOid != Guid.Empty)
            {
                pos_worksessionperiod resultWorkSessionPeriod = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(workSessionPeriodOid);
                //Add Parent Reference, not used because we use GlobalFramework.WorkSessionPeriodDay and not GlobalFramework.WorkSessionPeriodTerminal.Parent
                if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
                {
                    //Child > Parent Reference
                    resultWorkSessionPeriod.Parent = XPOSettings.WorkSessionPeriodDay;
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
            string periodType = (pWorkSessionPeriodType == WorkSessionPeriodType.Day) ? "Day" : "Terminal";
            string description = (pDescription != string.Empty) ? string.Format(" - {0}", pDescription) : string.Empty;
            pos_configurationplaceterminal terminal = XPOSettings.Session.GetObjectByKey<pos_configurationplaceterminal>(TerminalSettings.LoggedTerminal.Oid);
            DateTime dateTime = XPOHelper.CurrentDateTimeAtomic();

            pos_worksessionperiod workSessionPeriod = new pos_worksessionperiod(XPOSettings.Session)
            {
                PeriodType = pWorkSessionPeriodType,
                SessionStatus = WorkSessionPeriodStatus.Open,
                Designation = string.Format("{0} - {1}{2}", periodType, dateTime.ToString(LogicPOS.Settings.CultureSettings.DateTimeFormat), description),
                DateStart = dateTime,
                Terminal = terminal
            };
            //Assign Parent
            if (pWorkSessionPeriodType == WorkSessionPeriodType.Terminal)
            {
                workSessionPeriod.Parent = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(XPOSettings.WorkSessionPeriodDay.Oid);
            }
            //Persist
            workSessionPeriod.Save();

            if (pWorkSessionPeriodType == WorkSessionPeriodType.Day)
            {
                XPOSettings.WorkSessionPeriodDay = workSessionPeriod;
            }
            else
            {
                XPOSettings.WorkSessionPeriodTerminal = workSessionPeriod;
            }

            return true;
        }

        /// <summary>
        /// Close WorkSessionPeriod, work in all PeriodTypes Day and Terminal
        /// </summary>
        /// <param name="pWorkSessionPeriod"></param>
        public static bool SessionPeriodClose(pos_worksessionperiod pWorkSessionPeriod)
        {
            bool result = false;

            //Store Totals first, with a UOF, Error return without close session, we process error outside
            bool resultPersistTotals = PersistWorkSessionTotals(pWorkSessionPeriod);

            if (resultPersistTotals)
            {
                pos_worksessionperiod workSessionPeriod = XPOSettings.Session.GetObjectByKey<pos_worksessionperiod>(pWorkSessionPeriod.Oid);
                DateTime dateTime = XPOHelper.CurrentDateTimeAtomic();
                workSessionPeriod.DateEnd = dateTime;
                workSessionPeriod.SessionStatus = WorkSessionPeriodStatus.Close;
                workSessionPeriod.Save();

                //Assign to Singleton
                if (workSessionPeriod.PeriodType == WorkSessionPeriodType.Day)
                {
                    XPOSettings.WorkSessionPeriodDay = workSessionPeriod;
                }
                else
                {
                    XPOSettings.WorkSessionPeriodTerminal = workSessionPeriod;
                }

                result = true;

            }
            return result;
        }

        public static XPCollection GetSessionPeriodTotal(pos_worksessionperiod pWorkSessionPeriod)
        {
            return GetSessionPeriodTotal(XPOSettings.Session, pWorkSessionPeriod);
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
            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                uint paymentMethodOrd = 0;
                string paymentMethodToken = string.Empty;
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

                XPSelectData xPSelectData = XPOHelper.GetSelectedDataFromQuery(uowSession, sql);
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
                        configurationPaymentMethod = (fin_configurationpaymentmethod)XPOHelper.GetXPGuidObjectFromField(uowSession, typeof(fin_configurationpaymentmethod), "Token", paymentMethodToken);

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
                    return false;
                }
            }
        }

        /// <summary>
        /// Get total number of Opened Terminal Period Sessions
        /// </summary>
        public static XPSelectData GetSessionPeriodOpenTerminalSessions()
        {
            //string sql = string.Format(@"SELECT Count(*) as Count FROM pos_worksessionperiod WHERE Parent = '{0}' AND SessionStatus = 0;", GlobalFramework.WorkSessionPeriodDay.Oid.ToString());
            string sql = string.Format(@"SELECT Oid, Designation, DateStart, Terminal FROM pos_worksessionperiod WHERE PeriodType = 1 AND Parent = '{0}' AND SessionStatus = 0;", XPOSettings.WorkSessionPeriodDay.Oid.ToString());
            XPSelectData xPSelectData = XPOHelper.GetSelectedDataFromQuery(sql);
            return xPSelectData;
        }

        /// <summary>
        /// Get total number of Opened Orders
        /// </summary>
        public static XPSelectData GetOpenOrderTables()
        {
            string sql = string.Format(@"SELECT PlaceTable FROM fin_documentordermain WHERE OrderStatus = 1;");
            XPSelectData xPSelectData = XPOHelper.GetSelectedDataFromQuery(sql);
            return xPSelectData;
        }

        public static decimal GetSessionPeriodMovementTotal(pos_worksessionperiod pWorkSessionPeriod, MovementTypeTotal pMovementTypeTotal)
        {
            return GetSessionPeriodMovementTotal(XPOSettings.Session, pWorkSessionPeriod, pMovementTypeTotal);
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
            string sqlWhereTypeTotal = string.Empty;
            string sqlWherePeriod = string.Empty;

            if (pMovementTypeTotal != MovementTypeTotal.None)
            {
                switch (pMovementTypeTotal)
                {
                    //Alteração no funcionamento do Inicio/fecho Sessão [IN:014330]
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
                //_logger.Debug(string.Format("pMovementTypeTotal: [{0}], resultTotal: [{1}], sql: [{2}]", pMovementTypeTotal, resultTotal, sql));

            }
            return resultTotal;
        }

        public static string GetSessionPeriodMovementTotalDebug(pos_worksessionperiod pWorkSessionPeriod, bool pOutputToLog)
        {
            string resultDetail = string.Format("WorkSessionPeriod:[{0}] Type:[{1}]", pWorkSessionPeriod.Oid, pWorkSessionPeriod.PeriodType);
            string resultValues = string.Empty;
            string resultFields = string.Empty;

            foreach (MovementTypeTotal typeTotal in Enum.GetValues(typeof(MovementTypeTotal)))
            {
                resultFields += string.Format("{0}\t", typeTotal);
                resultValues += string.Format("{0}\t", GetSessionPeriodMovementTotal(pWorkSessionPeriod, typeTotal).ToString());
            }

            string result = string.Format("{0}{1}{2}{3}{4}{5}{6}", Environment.NewLine, resultDetail, Environment.NewLine, resultFields, Environment.NewLine, resultValues, Environment.NewLine);

            return result;
        }

        public static decimal GetSessionPeriodCashDrawerOpenOrCloseAmount(string pMoventTypeToken)
        {
            return GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(XPOSettings.Session, XPOSettings.WorkSessionPeriodTerminal, pMoventTypeToken);
        }

        public static decimal GetSessionPeriodCashDrawerOpenOrCloseAmount(pos_worksessionperiod pWorkSessionPeriod, string pMoventTypeToken)
        {
            return GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(XPOSettings.Session, pWorkSessionPeriod, pMoventTypeToken);
        }

        /// <summary>
        /// Get Last CASHDRAWER_OPEN/CASHDRAWER_CLOSE Value, Required WorkSessionPeriod, to Re-Print Sessions from Other Terminals, do not use TerminalSettings.LoggedTerminal.Oid
        /// </summary>
        public static decimal GetSessionPeriodTerminalCashDrawerOpenOrCloseAmount(Session pSession, pos_worksessionperiod pWorkSessionPeriod, string pMoventTypeToken)
        {
            //If Has a Valid pWorkSessionPeriod get Terminal from it, else use logged Terminal
            Guid whereTerminalGuid = (pWorkSessionPeriod != null) ? pWorkSessionPeriod.Terminal.Oid : TerminalSettings.LoggedTerminal.Oid;

            //Required, CASHDRAWER_OPEN always comes from WorkSessionPeriod, and CASHDRAWER_CLOSE comes from latest cash close record (DESC)
            string whereOpen = (pMoventTypeToken == "CASHDRAWER_OPEN") ? string.Format(" AND wspPeriod = '{0}'", pWorkSessionPeriod.Oid) : string.Empty;

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
            //_logger.Debug(string.Format("pMoventTypeToken sql: [{0}]", sql));

            return resultAmount;
        }

        /// <summary>
        /// Get CashDrawer Open Amount for all Used terminals, Required three queries
        /// </summary>
        public static decimal GetSessionPeriodDayCashDrawerOpenOrCloseAmount(pos_worksessionperiod pWorkSessionPeriod, bool pEnableOpenMode)
        {
            decimal result = 0.0m;

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
            SelectedData xpoSelectedDataTerminals = XPOSettings.Session.ExecuteQuery(sqlTerminals);

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
                listPeriod.Add(XPOSettings.Session.ExecuteScalar(sqlPeriod).ToString());
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
            string wherePeriod = string.Empty;
            string whereSeparator = string.Empty;
            foreach (string item in listPeriod)
            {
                whereSeparator = (i > 0) ? " OR " : string.Empty;
                wherePeriod += string.Format("{0}wspPeriod = '{1}'", whereSeparator, item);
                i++;
            }

            //Shared for Both Modes
            wherePeriod = string.Format(" AND ({0})", wherePeriod);
            //Require to Add for OPEN Mode
            string whereClose = (!pEnableOpenMode) ? " OR wmtMovementTypeToken = 'CASHDRAWER_IN' OR wmtMovementTypeToken = 'CASHDRAWER_OUT'" : string.Empty;

            //Final Query
            sqlCashDrawerAmount = string.Format(sqlCashDrawerAmount, whereClose, wherePeriod);
            //_logger.Debug(string.Format("sqlCashDrawerAmount: [{0}]", sqlCashDrawerAmount));
            var cashDrawerAmount = XPOSettings.Session.ExecuteScalar(sqlCashDrawerAmount);
            result = (cashDrawerAmount != null) ? Convert.ToDecimal(cashDrawerAmount) : 0.0m;


            return result;
        }

        public static Hashtable GetSessionPeriodSummaryDetails(Guid workSessionId)
        {
            var workSession = XPOHelper.GetEntityById<pos_worksessionperiod>(workSessionId);

            //Get Total Money and TotalMoney Out (NonPayments)
            decimal totalMoneyIn = GetSessionPeriodMovementTotal(workSession, MovementTypeTotal.MoneyIn);
            decimal totalMoneyOut = -GetSessionPeriodMovementTotal(workSession, MovementTypeTotal.MoneyOut);

            //Get Total Money in Cash when Open/Close for Day and Terminals
            decimal totalMoneyInCashDrawerOnOpen;
            decimal totalMoneyInCashDrawer;
            if (workSession.PeriodType == WorkSessionPeriodType.Day)
            {
                totalMoneyInCashDrawerOnOpen = GetSessionPeriodDayCashDrawerOpenOrCloseAmount(workSession, true);
                totalMoneyInCashDrawer = GetSessionPeriodDayCashDrawerOpenOrCloseAmount(workSession, false);
            }
            else
            {
                totalMoneyInCashDrawerOnOpen = GetSessionPeriodCashDrawerOpenOrCloseAmount(workSession, "CASHDRAWER_OPEN");
                //Get Total in CashDrawer On Close or on Working: Using latest CASHDRAWER_CLOSE get from terminal method if Terminal Session
                if (workSession.SessionStatus == WorkSessionPeriodStatus.Close)
                {
                    totalMoneyInCashDrawer = GetSessionPeriodCashDrawerOpenOrCloseAmount(workSession, "CASHDRAWER_CLOSE");
                }
                else
                {
                    totalMoneyInCashDrawer = GetSessionPeriodMovementTotal(workSession, MovementTypeTotal.MoneyInCashDrawer);
                }
            }

            Hashtable resultHashTable = new Hashtable
            {
                { "totalMoneyInCashDrawerOnOpen", totalMoneyInCashDrawerOnOpen },
                { "totalMoneyInCashDrawer", totalMoneyInCashDrawer },
                { "totalMoneyIn", totalMoneyIn },
                { "totalMoneyOut", totalMoneyOut }
            };

            return resultHashTable;
        }

        //public static decimal GetLastPaymentMethodTotal(WorkSessionPeriodType pWorkSessionPeriodType, WorkSessionPeriod pWorkSessionPeriod, ConfigurationPaymentMethod pPaymentMethod)
        //{
        //  string sql = string.Format(@"SELECT Oid FROM pos_worksessionperiod WHERE SessionStatus = 1 AND PeriodType = {0} AND Terminal = '{1}' ORDER BY DateEnd DESC;", pWorkSessionPeriodType, TerminalSettings.LoggedTerminal.Oid);
        //  Guid workSessionPeriodOid = Utils.GetGuidFromQuery(sql);
        //  if (workSessionPeriodOid != Guid.Empty)
        //  {
        //    WorkSessionPeriod workSessionPeriod = XPOSettings.Session.GetObjectByKey<WorkSessionPeriod>(workSessionPeriodOid);
        //    foreach (WorkSessionPeriodTotal item in workSessionPeriod.TotalPeriod)
        //    {
        //      _logger.Debug(string.Format("Message: [{0}]", item.Total));
        //    }
        //    //WorkSessionPeriodTotal  WorkSessionPeriodTotal = XPOSettings.Session.GetObjectByKey<WorkSessionPeriodTotal>(workSessionPeriodOid);
        //    return 1.0m;
        //  }
        //  else
        //  {
        //    return 0.0m;
        //  }
        //  return 1.0m;
        //}


        //No DocumentFinanceMaster|DocumentFinancePayment - With and Without Session
        public static bool PersistWorkSessionMovement(pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(XPOSettings.Session, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(XPOSettings.Session, pWorkSessionPeriod, pWorkSessionMovementType, null, null, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        //Main Method  - With and Without Session
        public static bool PersistWorkSessionMovement(pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            return PersistWorkSessionMovement(XPOSettings.Session, pWorkSessionPeriod, pWorkSessionMovementType, pDocumentFinanceMaster, pDocumentFinancePayment, pUserDetail, pTerminal, pDate, pMovementAmount, pDescription);
        }

        public static bool PersistWorkSessionMovement(Session pSession, pos_worksessionperiod pWorkSessionPeriod, pos_worksessionmovementtype pWorkSessionMovementType, fin_documentfinancemaster pDocumentFinanceMaster, fin_documentfinancepayment pDocumentFinancePayment, sys_userdetail pUserDetail, pos_configurationplaceterminal pTerminal, DateTime pDate, decimal pMovementAmount, string pDescription, uint pOrd = 1)
        {
            //Prevent Deleted Objects, Get Fresh Objects
            sys_userdetail userDetail = pSession.GetObjectByKey<sys_userdetail>(XPOSettings.LoggedUser.Oid);
            pos_configurationplaceterminal terminal = pSession.GetObjectByKey<pos_configurationplaceterminal>(TerminalSettings.LoggedTerminal.Oid);
            pos_worksessionmovementtype workSessionMovementType = pSession.GetObjectByKey<pos_worksessionmovementtype>(pWorkSessionMovementType.Oid);

            pos_worksessionmovement workSessionMovement = new pos_worksessionmovement(pSession)
            {
                Ord = pOrd,
                WorkSessionPeriod = pWorkSessionPeriod,
                WorkSessionMovementType = workSessionMovementType,
                UserDetail = userDetail,
                Terminal = terminal,
                Date = pDate,
                MovementAmount = pMovementAmount,
                Description = pDescription
            };
            //Assign parent DocumentFinanceMaster and PaymentMethod
            if (pDocumentFinanceMaster != null)
            {
                workSessionMovement.DocumentFinanceMaster = pDocumentFinanceMaster;
                workSessionMovement.DocumentFinanceType = pDocumentFinanceMaster.DocumentType;
                workSessionMovement.PaymentMethod = pDocumentFinanceMaster.PaymentMethod;
            }
            //Assign parent DocumentFinancePayment and PaymentMethod
            if (pDocumentFinancePayment != null)
            {
                workSessionMovement.DocumentFinancePayment = pDocumentFinancePayment;
                workSessionMovement.DocumentFinanceType = pDocumentFinancePayment.DocumentType;
                workSessionMovement.PaymentMethod = pDocumentFinancePayment.PaymentMethod;
            }

            //Save WorkSessionMovement if not in UOW
            if (pSession.GetType() == typeof(Session)) workSessionMovement.Save();

            return true;
        }
    }
}
