using LogicPOS.DTOs.Printing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;

namespace LogicPOS.Printing.Templates
{
    public abstract class ThermalPrinterBaseInternalTemplate : ThermalPrinterBaseTemplate
    {
        public ThermalPrinterBaseInternalTemplate(PrinterReferenceDto printer)
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
            _genericThermalPrinter.SetAlignCenter();

            //Extended Footer Text
            _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_internal_document_footer1"));
            _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_internal_document_footer2"));
            _genericThermalPrinter.LineFeed();
            _genericThermalPrinter.WriteLine(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_internal_document_footer3"));

            //Reset to Left
            _genericThermalPrinter.SetAlignLeft();

            //Line Feed
            _genericThermalPrinter.LineFeed();
        }
    }
}
