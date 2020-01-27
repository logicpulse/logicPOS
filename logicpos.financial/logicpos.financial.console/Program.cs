using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.console.App;
using logicpos.financial.console.Objects;
using logicpos.financial.console.Test.Classes.HardWare.Printer;
using logicpos.financial.console.Test.Classes.HardWare.Printers.Thermal;
using logicpos.financial.console.Test.WS;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.WorkSession;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;

namespace logicpos.financial.console
{
    class Program
    {
        //Log4Net basics with a Console Application (c#)
        //http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx
        //Add the assembly for the log4net.config to Properties/AssemblyInfo.cs
        //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedList<string, Action> _testActions;
        private static string _line = string.Empty;

        static void Main(string[] args)
        {
            //Init Settings Main Config Settings
            GlobalFramework.Settings = ConfigurationManager.AppSettings;
            //Base Bootstrap Init from LogicPos
            Init();
            //Extra Bootstrap for Console App
            InitExtended();
            //Init Test Actions
            InitTestActions();
            //Init Main
            InitMain();
        }

        //LogicPos BootStrap
        private static void Init()
        {
            Console.WriteLine(string.Format("BootStrap {0}....", SettingsApp.AppName));

            try
            {
                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;

                //Init Settings Main Config Settings
                //GlobalFramework.Settings = ConfigurationManager.AppSettings;

                //CultureInfo/Localization
                string culture = GlobalFramework.Settings["culture"];
                if (!string.IsNullOrEmpty(culture))
                {
                    /* IN006018 and IN007009 */
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                }
                //GlobalFramework.CurrentCulture = CultureInfo.CurrentUICulture;
                string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";                
                string getCultureFromDB = GlobalFramework.SessionXpo.ExecuteScalar(sql).ToString();
                GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);

                //Always use en-US NumberFormat because of mySql Requirements
                GlobalFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(SettingsApp.CultureNumberFormat);

                // Init Paths
                GlobalFramework.Path = new Hashtable();
                GlobalFramework.Path.Add("temp", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathTemp"]));
                GlobalFramework.Path.Add("saftpt", FrameworkUtils.OSSlash(GlobalFramework.Settings["pathSaftPt"]));
                //Create Directories
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["temp"])));
                FrameworkUtils.CreateDirectory(FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["saftpt"])));

                //Get DataBase Details
                GlobalFramework.DatabaseType = (DatabaseType) Enum.Parse(typeof(DatabaseType), GlobalFramework.Settings["databaseType"]);
                GlobalFramework.DatabaseName = SettingsApp.DatabaseName; 
                //Xpo Connection String
                string xpoConnectionString = string.Format(GlobalFramework.Settings["xpoConnectionString"], GlobalFramework.DatabaseName.ToLower());

                //Init XPO Connector DataLayer
                try
                {
                    _log.Debug(string.Format("Init XpoDefault.DataLayer: [{0}]", xpoConnectionString));
                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    GlobalFramework.SessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }

                //Get Terminal from Db
                GlobalFramework.LoggedTerminal = Utils.GetTerminal();

                //SettingsApp
                SettingsApp.ConfigurationSystemCountry = (cfg_configurationcountry)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcountry), new Guid(GlobalFramework.Settings["xpoOidConfigurationCountrySystemCountry"]));
                SettingsApp.ConfigurationSystemCurrency = (cfg_configurationcurrency)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(cfg_configurationcurrency), new Guid(GlobalFramework.Settings["xpoOidConfigurationCurrencySystemCurrency"]));

                //PreferenceParameters
                GlobalFramework.PreferenceParameters = FrameworkUtils.GetPreferencesParameters();

                //Try to Get open Session Day/Terminal for this Terminal
                GlobalFramework.WorkSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                GlobalFramework.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Terminal);

                //Init FastReports Custom Functions and Custom Vars
                CustomFunctions.Register(SettingsApp.AppName);

                //Init AppSession
                //string appSessionFile = SettingsApp.AppSessionFile;
                //if (File.Exists(appSessionFile))
                //{
                //    GlobalFramework.SessionApp = GlobalFrameworkSession.InitSession(appSessionFile);
                //}
                //else
                //{
                //    throw new Exception(string.Format("Missing appSessionFile: {0}", appSessionFile));
                //}
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Extra Stuff to LogicPos BootStrap
        private static void InitExtended()
        {
            try
            {
                //TODO
                //http://stackoverflow.com/questions/9640240/accessing-a-resource-file-from-a-different-project-with-resourcemanager-c-sharp
                //ResourceManager rm = new ResourceManager("Namespace.LanguageLocalization", logicpos.financial.library.Classes);
                
                //Override Paths
                GlobalFramework.Path["assets"] = @"c:\SVN\logicpos\trunk\src\logicpos\Assets\";
                GlobalFramework.Path["reports"] = @"c:\SVN\logicpos\trunk\src\logicpos\Resources\Reports\";
                //Get Terminal from DB
                GlobalFramework.LoggedUser = (sys_userdetail)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(sys_userdetail), new Guid(GlobalFramework.Settings["xpoOidUserDetailDefaultLoggedUser"]));
                //Get Permissions
                GlobalFramework.LoggedUserPermissions = FrameworkUtils.GetUserPermissions();
                //Override Terminal Printer (PDF)
                //GlobalFramework.LoggedTerminal.Printer = (sys_configurationprinters)GlobalFramework.SessionXpo.GetObjectByKey(typeof(sys_configurationprinters), SettingsApp.LoggedTerminalPrinter);
                GlobalApp.PrinterExportPDF = (sys_configurationprinters)GlobalFramework.SessionXpo.GetObjectByKey(typeof(sys_configurationprinters), SettingsApp.XpoOidPrinterExportPDF);
                GlobalApp.PrinterThermal = (sys_configurationprinters)GlobalFramework.SessionXpo.GetObjectByKey(typeof(sys_configurationprinters), SettingsApp.XpoOidPrinterThermal);

                if (GlobalFramework.LoggedTerminal == null) _log.Debug("Invalid Printer. Working Without Printer");
                //Override Licence
                GlobalFramework.LicenceCompany = "Logicpulse : Demonstração";
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private static void InitTestActions()
        {
            // Init Dictionary
            _testActions = new SortedList<string, Action>();

            // Add Tests
            //FrameworkUtils
            //_testActions.Add("01) FrameworkUtils.CurrentDateTimeAtomic()", () => FrameworkUtils.CurrentDateTimeAtomic());

            //WebService
            _testActions.Add("01) TestWSInterface.GetSecurityToken()", () => 
                TestWSInterface.GetSecurityToken()
            );

/*
            //ArticleBag
            _testActions.Add("02) TestArticleBag.ShowArticleBag()", () => 
                TestArticleBag.ShowArticleBag()
            );
            //PersistFinanceDocument
            _testActions.Add("03) TestProcessFinanceDocument.PersistFinanceDocument(Empty)", () => 
                TestProcessFinanceDocument.PersistFinanceDocumentMinimal(Guid.Empty)
            );
            _testActions.Add("04) TestProcessFinanceDocument.PersistFinanceDocument(SimplifiedInvoice)", () => 
                TestProcessFinanceDocument.PersistFinanceDocument(SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice)
            );
            _testActions.Add("05) TestProcessFinanceDocument.PersistFinanceDocument(Invoice)", () => 
                TestProcessFinanceDocument.PersistFinanceDocument(SettingsApp.XpoOidDocumentFinanceTypeInvoice)
            );
            _testActions.Add("06) TestProcessFinanceDocument.PersistFinanceDocument(CreditNote)", () => 
                TestProcessFinanceDocument.PersistFinanceDocumentCreditNote(SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
            );
            _testActions.Add("07) TestProcessFinanceDocument.PersistFinanceDocument(ConsignationInvoice)", () => 
                TestProcessFinanceDocument.PersistFinanceDocument(SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice)
            );
            //PersistFinanceDocumentPayment
            _testActions.Add("08) TestProcessFinanceDocument.PersistFinanceDocumentPayment()", () => 
                TestProcessFinanceDocument.PersistFinanceDocumentPayment()
            );
            //Test PDF
            _testActions.Add("09) TestCustomReport.DocumentMasterCreatePDF()", () => 
                TestCustomReport.DocumentMasterCreatePDF()
            );
*/
            //OpenDoor
            _testActions.Add("02) TestThermalPrinterGeneric.OpenDoor()", () => 
                TestThermalPrinterGeneric.OpenDoor(GlobalApp.PrinterThermal)
            );
            //Thermal Printer test Documents
            _testActions.Add("03) TestThermalPrinterGeneric.Print()", () => 
                TestThermalPrinterGeneric.Print(GlobalApp.PrinterThermal)
            );
            _testActions.Add("04) TestThermalPrinterFinanceDocument.Print()", () => 
                TestThermalPrinterFinanceDocumentMaster.Print(GlobalApp.PrinterThermal)
            );
            _testActions.Add("05) TestThermalPrinterFinanceDocumentPayment.Print()", () => 
                TestThermalPrinterFinanceDocumentPayment.Print(GlobalApp.PrinterThermal)
            );
            _testActions.Add("06) TestThermalPrinterInternalDocumentOrderRequest.Print()", () => 
                TestThermalPrinterInternalDocumentOrderRequest.Print(GlobalApp.PrinterThermal)
            );
            _testActions.Add("07) TestThermalPrinterInternalDocumentCashDrawer.Print()", () => 
                TestThermalPrinterInternalDocumentCashDrawer.Print(GlobalApp.PrinterThermal)
            );
            _testActions.Add("08) TestThermalPrinterInternalDocumentWorkSession.Print()", () => 
                TestThermalPrinterInternalDocumentWorkSession.Print(GlobalApp.PrinterThermal)
            );
        }

        private static void InitMain()
        {
            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            Console.TreatControlCAsInput = true;

            //Console.WriteLine("Press any combination of CTL, ALT, and SHIFT, and a console key.");
            //Console.WriteLine("Press the Escape (Esc) key to quit: \n");
            ListTestActions();

            do
            {
                Console.Write("INP> ");
                cki = Console.ReadKey();
                Console.Write("\n");

                //Pressed Number Key
                if (cki.Key.ToString().Length == 2 && cki.Key.ToString().Substring(0, 1) == "D")
                {
                    int keyNumber = Convert.ToInt16(cki.Key.ToString().Substring(1, 1)) - 1;
                    try
                    {
                        if (keyNumber >= 0 && keyNumber <= _testActions.Count)
                        {
                            Console.Clear();
                            //Console.WriteLine(string.Format("Pressed keyNumber: {0}", keyNumber, _testActions.Keys[0]));
                            Console.WriteLine(string.Format("Test: {0}", _testActions.Keys[keyNumber]));
                            Console.WriteLine(_line);

                            try
                            {
                                _testActions.Values[keyNumber].Invoke();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            Console.WriteLine(_line);
                            Console.WriteLine("Press any key...");
                            Console.ReadKey();
                            ListTestActions();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine("Press any key...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    ListTestActions();
                }
            } while (cki.Key != ConsoleKey.Escape);

            //Quit
            //Console.WriteLine("Press any key....");
            //Console.ReadKey();
        }
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper

        private static void ListTestActions()
        {
            Console.Clear();

            Console.WriteLine(string.Format("{0} : Database: [{1}]", SettingsApp.AppName, SettingsApp.DatabaseName));
            Console.WriteLine(_line);

            int i = 0;
            foreach (var testAction in _testActions)
            {
                i++;
                //Console.WriteLine(string.Format("{0}) {1}", i.ToString("00"), testAction.Key));
                Console.WriteLine(testAction.Key);
            }
            Console.WriteLine(_line);
        }
    }
}
