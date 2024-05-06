using logicpos.datalayer.DataLayer.Xpo;
using logicpos.shared.App;
using System;
using LogicPOS.Settings.Extensions;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public abstract class ThermalPrinterBaseInternalTemplate : ThermalPrinterBaseTemplate
    {
        public ThermalPrinterBaseInternalTemplate(sys_configurationprinters pPrinter)
            : base(pPrinter, SharedSettings.PrinterThermalImageCompanyLogo)
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
            _thermalPrinterGeneric.SetAlignCenter();

            //Extended Footer Text
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_internal_document_footer1"));
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_internal_document_footer2"));
            _thermalPrinterGeneric.LineFeed();
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_internal_document_footer3"));

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();

            //Line Feed
            _thermalPrinterGeneric.LineFeed();
        }
    }
}
