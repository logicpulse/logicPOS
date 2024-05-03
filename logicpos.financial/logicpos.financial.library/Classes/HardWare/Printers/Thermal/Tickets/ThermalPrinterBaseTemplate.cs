using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.financial.library.Classes.Reports;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public abstract class ThermalPrinterBaseTemplate
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Members
        protected ThermalPrinterGeneric _thermalPrinterGeneric;
        protected Dictionary<string, string> _customVars;
        //Protected Dictionary<string, string> _systemVars;
        protected int _maxCharsPerLineNormal = 0;
        protected int _maxCharsPerLineNormalBold = 0;
        protected int _maxCharsPerLineSmall = 0;
        protected string _companyLogo = string.Empty;
        //Protected/Public
        protected string _ticketTitle = string.Empty;
        public string TicketTitle
        {
            get { return _ticketTitle; }
            set { _ticketTitle = value; }
        }
        protected string _ticketSubTitle = string.Empty;
        protected string TicketSubTitle
        {
            get { return _ticketSubTitle; }
            set { _ticketSubTitle = value; }
        }

        public ThermalPrinterBaseTemplate(sys_configurationprinters pPrinter)
            : this(pPrinter, SharedSettings.PrinterThermalImageCompanyLogo)
        {
        }

        public ThermalPrinterBaseTemplate(sys_configurationprinters pPrinter, string pCompanyLogo)
        {
            try
            {
                //Init Properties
                _thermalPrinterGeneric = new ThermalPrinterGeneric(pPrinter);
                _maxCharsPerLineNormal = _thermalPrinterGeneric.MaxCharsPerLineNormal;
                _maxCharsPerLineNormalBold = _thermalPrinterGeneric.MaxCharsPerLineNormalBold;
                _maxCharsPerLineSmall = _thermalPrinterGeneric.MaxCharsPerLineSmall;
                _companyLogo = pCompanyLogo;

                //Init Custom Vars From FastReport
                _customVars =  SharedFramework.FastReportCustomVars;
                //_systemVars = GlobalFramework.FastReportSystemVars;
                //Test FastReport Helpers with
                //_customVars["COMPANY_NAME"])) | _systemVars["APP_NAME"])) | CustomFunctions.Res("global_printed_on_date")
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool Print()
        {
            bool result;
            try
            {
                PrintHeader();
                PrintContent();
                PrintFooter();

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /* IN009055 */
        protected void PrintHeader()
        {
            PrintHeader(false); /* IN009055 - "true" when Order, and "false" when Invoice */
        }
        /// <summary>
        /// When printing the order/invoice, this method inserts the header of document, with logo or company details.
        /// The original method has been overloaded to allow to remove some details from order document.
        /// Please see IN009055 for more details.
        /// </summary>
        /// <param name="isOrder">"true" when Order, and "false" when Invoice</param>
        protected void PrintHeader(bool isOrder)
        {
            try
            {
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'TICKET_FILENAME_loggerO';";
                string result = XPOSettings.Session.ExecuteScalar(sql).ToString();

                string logo = string.Format(
                    @"{0}{1}",
                    DataLayerFramework.Path["assets"],
                    _companyLogo
                );

                sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'TICKET_PRINT_COMERCIAL_NAME';";
                var printComercialName = XPOSettings.Session.ExecuteScalar(sql).ToString();

                //Print Logo or Name + BusinessName
                //TK016249 - Impressoras - Diferenciação entre Tipos

                string companyBusinessName = _customVars["COMPANY_BUSINESS_NAME"];
                if (string.IsNullOrEmpty(companyBusinessName)) companyBusinessName = "";

                if (File.Exists(logo) && DataLayerFramework.LoggedTerminal.ThermalPrinter.ThermalPrintLogo)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        logo = result;
                    }
                    _thermalPrinterGeneric.PrintImage(logo);
                }
                
                else if (isOrder) /* IN009055 */
                {
                    _thermalPrinterGeneric.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
                }
                else if(printComercialName != null && !Convert.ToBoolean(printComercialName))
                {
                    _thermalPrinterGeneric.WriteLine(_customVars["COMPANY_NAME"]);
                }
                else if(!string.IsNullOrEmpty(companyBusinessName) && companyBusinessName.Length > 20)
                {
                    _thermalPrinterGeneric.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
                    _thermalPrinterGeneric.WriteLine(_customVars["COMPANY_NAME"]);
                }
                else
                {
                    _thermalPrinterGeneric.WriteLine(companyBusinessName, WriteLineTextMode.Big);
                    _thermalPrinterGeneric.WriteLine(_customVars["COMPANY_NAME"]);
                }

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        //Class Title and Default class SubTitle
        protected void PrintTitles()
        {
            PrintTitles(_ticketTitle, _ticketSubTitle);
        }

        //Parameter Title and Default class SubTitle
        protected void PrintTitles(string pTicketTitle)
        {
            PrintTitles(pTicketTitle, _ticketSubTitle);
        }

        protected void PrintTitles(string pTicketTitle, string pTicketSubTitle)
        {
            //Set Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            if (!string.IsNullOrEmpty(pTicketTitle)) _thermalPrinterGeneric.WriteLine(pTicketTitle, WriteLineTextMode.Big);
            if (!string.IsNullOrEmpty(pTicketSubTitle)) _thermalPrinterGeneric.WriteLine(pTicketSubTitle, WriteLineTextMode.DoubleHeightBold);
            _thermalPrinterGeneric.LineFeed();

            //Reset Align 
            _thermalPrinterGeneric.SetAlignLeft();
        }

        //Abstract Required Implementation
        public abstract void PrintContent();

        protected void PrintNotes(string pNotes)
        {
            try
            {
                //Print Notes
                if (pNotes != string.Empty)
                {
                    _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_notes"), WriteLineTextMode.Bold);
                    _thermalPrinterGeneric.WriteLine(pNotes);
                    //Line Feed
                    _thermalPrinterGeneric.LineFeed();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void PrintFooter()
        {
            try
            {
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                //Set Font Size: Small
                _thermalPrinterGeneric.SetFont(1);

                //User : Terminal
                _thermalPrinterGeneric.WriteLine(string.Format("{0} - {1}", DataLayerFramework.LoggedUser.Name, DataLayerFramework.LoggedTerminal.Designation));
                _thermalPrinterGeneric.LineFeed();

                //Printed On | Company|App|Version
                _thermalPrinterGeneric.WriteLine(string.Format("{1}: {2}{0}{3}: {4} {5}"
                    , Environment.NewLine
                    , CustomFunctions.Res("global_printed_on_date")
                    , DataLayerUtils.CurrentDateTimeAtomic().ToString(SharedSettings.DateTimeFormat)
                    , _customVars["APP_COMPANY"]
                    , _customVars["APP_NAME"]
                    , _customVars["APP_VERSION"]
                    )
                ); /* IN009211 */

                //Reset Font Size: Normal
                _thermalPrinterGeneric.SetFont(0);

                //Line Feed
                _thermalPrinterGeneric.LineFeed();

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Finish With Cut and Print Buffer
                //TK016249 - Impressoras - Diferenciação entre Tipos
                _thermalPrinterGeneric.Cut(true, DataLayerFramework.LoggedTerminal.ThermalPrinter.ThermalCutCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void PrintBuffer()
        {
            try
            {
                _thermalPrinterGeneric.PrintBuffer();
            }
            catch (Exception ex)
            {
                _logger.Error("void PrintBuffer() 1:: " + ex.Message, ex);
                throw ex;
            }
        }
    }
}
