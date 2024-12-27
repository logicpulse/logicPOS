using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Common;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Enums;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LogicPOS.Printing.Templates
{
    public abstract class Template
    {
        protected GenericThermalPrinter _printer;
        protected Dictionary<string, string> _customVars;
        protected int _maxCharsPerLineNormal = 0;
        protected int _maxCharsPerLineNormalBold = 0;
        protected int _maxCharsPerLineSmall = 0;
        protected CompanyPrintingInformationsDto _companyInformationsDto;
        protected string _ticketTitle = string.Empty;
        protected string _ticketSubTitle = string.Empty;
        protected string _userName = string.Empty;
        protected string _terminalDesignation = string.Empty;



        public Template(
           PrinterDto printer,
           string terminalDesignation, string userName)
        {
            _terminalDesignation = terminalDesignation;
            _userName = userName;
        }


        public Template(
        PrinterDto printer,
        string terminalDesignation,
        string userName,
        CompanyPrintingInformationsDto pCompanyInformationsDtos)
        {
            try
            {

                _printer = new GenericThermalPrinter(printer);
                _maxCharsPerLineNormal = _printer.MaxCharsPerLineNormal;
                _maxCharsPerLineNormalBold = _printer.MaxCharsPerLineNormalBold;
                _maxCharsPerLineSmall = _printer.MaxCharsPerLineSmall;
                _companyInformationsDto = pCompanyInformationsDtos;
                _terminalDesignation = terminalDesignation;
                _userName = userName;
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

        string ConvertBase64ToBmp(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var file = Path.GetTempFileName()+".bmp";
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                using (Image image = Image.FromStream(ms))
                {
                    image.Save(file, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            return file;
        }

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
        /// 
        protected void PrintHeader(bool isOrder)
        {
            _printer.SetAlignCenter();

           
            string logo = _companyInformationsDto?.Logo;

            var printComercialName = _companyInformationsDto.ComercialName;

            //Print Logo or Name + BusinessName
            //TK016249 - Impressoras - Diferenciação entre Tipos

            string companyBusinessName = _companyInformationsDto.BusinessName;
            if (string.IsNullOrEmpty(companyBusinessName))
            {
                companyBusinessName = _companyInformationsDto.Name;
            }
            if (File.Exists(logo))
            {
                _printer.PrintImage(logo);
            }

            else if (isOrder) /* IN009055 */
            {
                _printer.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
            }
            else if (printComercialName != null && !Convert.ToBoolean(printComercialName))
            {
                _printer.WriteLine(_companyInformationsDto.Name);
            }
            else if (!string.IsNullOrEmpty(companyBusinessName) && companyBusinessName.Length > 20)
            {
                _printer.WriteLine(companyBusinessName, WriteLineTextMode.DoubleHeightBold);
                _printer.WriteLine(_companyInformationsDto.Name);
            }
            else
            {
                _printer.WriteLine(companyBusinessName, WriteLineTextMode.Big);
                _printer.WriteLine(_companyInformationsDto.Name);
            }

            //Reset to Left
            _printer.SetAlignLeft();

            //Line Feed
            _printer.LineFeed();
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
            _printer.SetAlignCenter();

            if (!string.IsNullOrEmpty(pTicketTitle)) _printer.WriteLine(pTicketTitle, WriteLineTextMode.Big);
            if (!string.IsNullOrEmpty(pTicketSubTitle)) _printer.WriteLine(pTicketSubTitle, WriteLineTextMode.DoubleHeightBold);
            _printer.LineFeed();

            //Reset Align 
            _printer.SetAlignLeft();
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
                    _printer.WriteLine(GeneralUtils.GetResourceByName("global_notes"), WriteLineTextMode.Bold);
                    _printer.WriteLine(pNotes);
                    //Line Feed
                    _printer.LineFeed();
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
                _printer.SetAlignCenter();

                //Set Font Size: Small
                _printer.SetFont(1);

                //User : Terminal
                _printer.WriteLine(string.Format("{0} - {1}", _userName, _terminalDesignation));
                _printer.LineFeed();

                //Printed On | Company|App|Version
                _printer.WriteLine(string.Format("{1}: {2}{0}{3}: {4} {5}"
                    , Environment.NewLine
                    , GeneralUtils.GetResourceByName("global_printed_on_date")
                    , XPOUtility.CurrentDateTimeAtomic().ToString(CultureSettings.DateTimeFormat)
                    , "LogicPulse"//_customVars["APP_COMPANY"]
                    , "LogicPOS"//_customVars["APP_NAME"]
                    , "vs1.010.1"//_customVars["APP_VERSION"]
                    )
                ); /* IN009211 */

                //Reset Font Size: Normal
                _printer.SetFont(0);

                //Line Feed
                _printer.LineFeed();

                //Reset to Left
                _printer.SetAlignLeft();

                //Finish With Cut and Print Buffer
                //TK016249 - Impressoras - Diferenciação entre Tipos
                _printer.Cut(true, _printer.CutCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void PrintBuffer()
        {
            _printer.PrintBuffer();
        }
    }
}
