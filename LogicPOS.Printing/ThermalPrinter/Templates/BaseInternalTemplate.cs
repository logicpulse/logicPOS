using LogicPOS.DTOs.Printing;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Printing.Templates
{
    public abstract class BaseInternalTemplate : Template
    {
        public BaseInternalTemplate(PrinterDto printer)
            : base(
                  printer,
                  PrintingSettings.ThermalPrinter.CompanyLogoLocation)
        {
        }

        //Override Parent Template
        public override bool Print()
        {
            bool result;
            try
            {
                //Call Base Template PrintHeader
                PrintHeader(true); /* IN009055 - when is Order, true */

                //Call Child Content (Overrided)
                PrintContent();

                //Call PrintFooterExtended
                PrintFooterExtended();
                //Call Base Template PrintFooter
                PrintFooter();

                //End Job
                PrintBuffer();

                result = true;
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        public void PrintFooterExtended()
        {
            //Align Center
            _printer.SetAlignCenter();

            //Extended Footer Text
            _printer.WriteLine(GeneralUtils.GetResourceByName("global_internal_document_footer1"));
            _printer.WriteLine(GeneralUtils.GetResourceByName("global_internal_document_footer2"));
            _printer.LineFeed();
            _printer.WriteLine(GeneralUtils.GetResourceByName("global_internal_document_footer3"));

            //Reset to Left
            _printer.SetAlignLeft();

            //Line Feed
            _printer.LineFeed();
        }
    }
}
