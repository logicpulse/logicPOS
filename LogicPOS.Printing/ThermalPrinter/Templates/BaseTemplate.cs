using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Printing;
using LogicPOS.Globalization;
using LogicPOS.Printing.Common;
using LogicPOS.Printing.Enums;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace LogicPOS.Printing.Templates
{
    public abstract class BaseTemplate
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected Members
        protected GenericThermalPrinter _genericThermalPrinter;
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

        public BaseTemplate(PrintingPrinterDto printer)
            : this(
                  printer,
                  PrintingSettings.ThermalPrinter.CompanyLogoLocation)
        {
        }

        public BaseTemplate(
            PrintingPrinterDto printer,
            string pCompanyLogo)
        {
            try
            {
                //Init Properties
                _genericThermalPrinter = new GenericThermalPrinter(printer);
                _maxCharsPerLineNormal = _genericThermalPrinter.MaxCharsPerLineNormal;
                _maxCharsPerLineNormalBold = _genericThermalPrinter.MaxCharsPerLineNormalBold;
                _maxCharsPerLineSmall = _genericThermalPrinter.MaxCharsPerLineSmall;
                _companyLogo = pCompanyLogo;

                //Init Custom Vars From FastReport
                _customVars = PrintingSettings.FastReportCustomVars;
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
                _genericThermalPrinter.SetAlignCenter();

                string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'TICKET_FILENAME_loggerO';";
                string result = XPOSettings.Session.ExecuteScalar(sql).ToString();

                string logo = string.Format(
                    @"{0}{1}",
                    PathsSettings.Paths["assets"],
                    _companyLogo
                );

                sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'TICKET_PRINT_COMERCIAL_NAME';";
                var printComercialName = XPOSettings.Session.ExecuteScalar(sql).ToString();

                //Print Logo or Name + BusinessName
                //TK016249 - Impressoras - Diferenciação entre Tipos

                string companyBusinessName = _customVars["COMPANY_BUSINESS_NAME"];
                if (string.IsNullOrEmpty(companyBusinessName)) companyBusinessName = "";

                if (File.Exists(logo) && TerminalSettings.LoggedTerminal.ThermalPrinter.ThermalPrintLogo)
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        logo = result;
                    }
                    _genericThermalPrinter.PrintImage(logo);
                }

                else if (isOrder) /* IN009055 */
                {
                    _genericThermalPrinter.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
                }
                else if (printComercialName != null && !Convert.ToBoolean(printComercialName))
                {
                    _genericThermalPrinter.WriteLine(_customVars["COMPANY_NAME"]);
                }
                else if (!string.IsNullOrEmpty(companyBusinessName) && companyBusinessName.Length > 20)
                {
                    _genericThermalPrinter.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
                    _genericThermalPrinter.WriteLine(_customVars["COMPANY_NAME"]);
                }
                else
                {
                    _genericThermalPrinter.WriteLine(companyBusinessName, WriteLineTextMode.Big);
                    _genericThermalPrinter.WriteLine(_customVars["COMPANY_NAME"]);
                }

                //Reset to Left
                _genericThermalPrinter.SetAlignLeft();

                //Line Feed
                _genericThermalPrinter.LineFeed();
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
            _genericThermalPrinter.SetAlignCenter();

            if (!string.IsNullOrEmpty(pTicketTitle)) _genericThermalPrinter.WriteLine(pTicketTitle, WriteLineTextMode.Big);
            if (!string.IsNullOrEmpty(pTicketSubTitle)) _genericThermalPrinter.WriteLine(pTicketSubTitle, WriteLineTextMode.DoubleHeightBold);
            _genericThermalPrinter.LineFeed();

            //Reset Align 
            _genericThermalPrinter.SetAlignLeft();
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
                    _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_notes"), WriteLineTextMode.Bold);
                    _genericThermalPrinter.WriteLine(pNotes);
                    //Line Feed
                    _genericThermalPrinter.LineFeed();
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
                _genericThermalPrinter.SetAlignCenter();

                //Set Font Size: Small
                _genericThermalPrinter.SetFont(1);

                //User : Terminal
                _genericThermalPrinter.WriteLine(string.Format("{0} - {1}", XPOSettings.LoggedUser.Name, TerminalSettings.LoggedTerminal.Designation));
                _genericThermalPrinter.LineFeed();

                //Printed On | Company|App|Version
                _genericThermalPrinter.WriteLine(string.Format("{1}: {2}{0}{3}: {4} {5}"
                    , Environment.NewLine
                    , GeneralUtils.GetResourceByName("global_printed_on_date")
                    , XPOUtility.CurrentDateTimeAtomic().ToString(CultureSettings.DateTimeFormat)
                    , _customVars["APP_COMPANY"]
                    , _customVars["APP_NAME"]
                    , _customVars["APP_VERSION"]
                    )
                ); /* IN009211 */

                //Reset Font Size: Normal
                _genericThermalPrinter.SetFont(0);

                //Line Feed
                _genericThermalPrinter.LineFeed();

                //Reset to Left
                _genericThermalPrinter.SetAlignLeft();

                //Finish With Cut and Print Buffer
                //TK016249 - Impressoras - Diferenciação entre Tipos
                _genericThermalPrinter.Cut(true, TerminalSettings.LoggedTerminal.ThermalPrinter.ThermalCutCommand);
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
                _genericThermalPrinter.PrintBuffer();
            }
            catch (Exception ex)
            {
                _logger.Error("void PrintBuffer() 1:: " + ex.Message, ex);
                throw ex;
            }
        }
    }
}
