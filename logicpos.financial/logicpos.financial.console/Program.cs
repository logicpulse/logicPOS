using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.console.App;
using logicpos.financial.console.Objects;
using logicpos.financial.console.Test.Classes.HardWare.Printer;
using logicpos.financial.console.Test.Classes.HardWare.Printers.Thermal;
using logicpos.financial.console.Test.WS;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.shared.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace logicpos.financial.console
{
    internal class Program
    {
        //Log4Net basics with a Console Application (c#)
        //http://geekswithblogs.net/MarkPearl/archive/2012/01/30/log4net-basics-with-a-console-application-c.aspx
        //Add the assembly for the log4net.config to Properties/AssemblyInfo.cs
        //[assembly: log4net.Config.XmlConfigurator(Watch = true)]

        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SortedList<string, Action> _testActions;
        private static readonly string _line = string.Empty;

        private static void Main(string[] args)
        {
            //Init Settings Main Config Settings
            DataLayerFramework.Settings = ConfigurationManager.AppSettings;
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
            Console.WriteLine(string.Format("BootStrap {0}....", ConsoleSettings.AppName));

            try
            {
                //Prepare AutoCreateOption
                AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;

                //Init Settings Main Config Settings
                //DataLayerFramework.Settings = ConfigurationManager.AppSettings;

                //CultureInfo/Localization
                string culture = DataLayerFramework.Settings["culture"];
                if (!string.IsNullOrEmpty(culture))
                {
                    /* IN006018 and IN007009 */
                    //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                }
                //SharedFramework.CurrentCulture = CultureInfo.CurrentUICulture;
                string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";                
                string getCultureFromDB = XPOSettings.Session.ExecuteScalar(sql).ToString();
                SharedFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);

                //Always use en-US NumberFormat because of mySql Requirements
                SharedFramework.CurrentCultureNumberFormat = CultureInfo.GetCultureInfo(ConsoleSettings.CultureNumberFormat);

                // Init Paths
                DataLayerFramework.Path = new Hashtable
                {
                    { "temp", SharedUtils.OSSlash(DataLayerFramework.Settings["pathTemp"]) },
                    { "saftpt", SharedUtils.OSSlash(DataLayerFramework.Settings["pathSaftPt"]) }
                };
                //Create Directories
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["temp"])));
                SharedUtils.CreateDirectory(SharedUtils.OSSlash(Convert.ToString(DataLayerFramework.Path["saftpt"])));

                //Get DataBase Details
               DataLayerFramework.DatabaseType = (DatabaseType) Enum.Parse(typeof(DatabaseType), DataLayerFramework.Settings["databaseType"]);
                SharedFramework.DatabaseName = ConsoleSettings.DatabaseName; 
                //Xpo Connection String
                string xpoConnectionString = string.Format(DataLayerFramework.Settings["xpoConnectionString"], SharedFramework.DatabaseName.ToLower());

                //Init XPO Connector DataLayer
                try
                {
                    _logger.Debug(string.Format("Init XpoDefault.DataLayer: [{0}]", xpoConnectionString));
                    XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                    XPOSettings.Session = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw;
                }

                //Get Terminal from Db
                DataLayerFramework.LoggedTerminal = Utils.GetTerminal();

                //SettingsApp
                DataLayerSettings.ConfigurationSystemCountry = (cfg_configurationcountry)DataLayerUtils.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcountry), new Guid(DataLayerFramework.Settings["xpoOidConfigurationCountrySystemCountry"]));
                SharedSettings.ConfigurationSystemCurrency = (cfg_configurationcurrency)DataLayerUtils.GetXPGuidObject(XPOSettings.Session, typeof(cfg_configurationcurrency), new Guid(DataLayerFramework.Settings["xpoOidConfigurationCurrencySystemCurrency"]));

                //PreferenceParameters
               SharedFramework.PreferenceParameters = SharedUtils.GetPreferencesParameters();

                //Try to Get open Session Day/Terminal for this Terminal
                SharedFramework.WorkSessionPeriodDay = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Day);
                SharedFramework.WorkSessionPeriodTerminal = ProcessWorkSessionPeriod.GetSessionPeriod(WorkSessionPeriodType.Terminal);

                //Init FastReports Custom Functions and Custom Vars
                CustomFunctions.Register(ConsoleSettings.AppName);

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
                _logger.Error(ex.Message, ex);
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
                DataLayerFramework.Path["assets"] = @"c:\SVN\logicpos\trunk\src\logicpos\Assets\";
                DataLayerFramework.Path["reports"] = @"c:\SVN\logicpos\trunk\src\logicpos\Resources\Reports\";
                //Get Terminal from DB
                DataLayerFramework.LoggedUser = (sys_userdetail)DataLayerUtils.GetXPGuidObject(XPOSettings.Session, typeof(sys_userdetail), new Guid(DataLayerFramework.Settings["xpoOidUserDetailDefaultLoggedUser"]));
                //Get Permissions
                SharedFramework.LoggedUserPermissions = SharedUtils.GetUserPermissions();
                //Override Terminal Printer (PDF)
                //DataLayerFramework.LoggedTerminal.Printer = (sys_configurationprinters)XPOSettings.Session.GetObjectByKey(typeof(sys_configurationprinters), SettingsApp.LoggedTerminalPrinter);
                ConsoleGlobalApp.PrinterExportPDF = (sys_configurationprinters)XPOSettings.Session.GetObjectByKey(typeof(sys_configurationprinters), ConsoleSettings.XpoOidPrinterExportPDF);
                ConsoleGlobalApp.PrinterThermal = (sys_configurationprinters)XPOSettings.Session.GetObjectByKey(typeof(sys_configurationprinters), ConsoleSettings.XpoOidPrinterThermal);

                if (DataLayerFramework.LoggedTerminal == null) _logger.Debug("Invalid Printer. Working Without Printer");
                //Override Licence
                SharedFramework.LicenseCompany = "Logicpulse : Demonstração";
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private static void InitTestActions()
        {
            // Init Dictionary
            _testActions = new SortedList<string, Action>
            {
                // Add Tests
                //FrameworkUtils
                //_testActions.Add("01) FrameworkUtils.CurrentDateTimeAtomic()", () => FrameworkUtils.CurrentDateTimeAtomic());

                //WebService
                {
                    "01) TestWSInterface.GetSecurityToken()",
                    () =>
                    TestWSInterface.GetSecurityToken()
                },

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
                {
                    "02) TestThermalPrinterGeneric.OpenDoor()",
                    () =>
                    TestThermalPrinterGeneric.OpenDoor(ConsoleGlobalApp.PrinterThermal)
                },
                //Thermal Printer test Documents
                {
                    "03) TestThermalPrinterGeneric.Print()",
                    () =>
                    TestThermalPrinterGeneric.Print(ConsoleGlobalApp.PrinterThermal)
                },
                {
                    "04) TestThermalPrinterFinanceDocument.Print()",
                    () =>
                    TestThermalPrinterFinanceDocumentMaster.Print(ConsoleGlobalApp.PrinterThermal)
                },
                {
                    "05) TestThermalPrinterFinanceDocumentPayment.Print()",
                    () =>
                    TestThermalPrinterFinanceDocumentPayment.Print(ConsoleGlobalApp.PrinterThermal)
                },
                {
                    "06) TestThermalPrinterInternalDocumentOrderRequest.Print()",
                    () =>
                    TestThermalPrinterInternalDocumentOrderRequest.Print(ConsoleGlobalApp.PrinterThermal)
                },
                {
                    "07) TestThermalPrinterInternalDocumentCashDrawer.Print()",
                    () =>
                    TestThermalPrinterInternalDocumentCashDrawer.Print(ConsoleGlobalApp.PrinterThermal)
                },
                {
                    "08) TestThermalPrinterInternalDocumentWorkSession.Print()",
                    () =>
                    TestThermalPrinterInternalDocumentWorkSession.Print(ConsoleGlobalApp.PrinterThermal)
                }
            };
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

            Console.WriteLine(string.Format("{0} : Database: [{1}]", ConsoleSettings.AppName, ConsoleSettings.DatabaseName));
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
