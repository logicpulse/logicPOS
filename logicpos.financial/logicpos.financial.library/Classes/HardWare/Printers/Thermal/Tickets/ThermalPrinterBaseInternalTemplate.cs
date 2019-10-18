using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public abstract class ThermalPrinterBaseInternalTemplate : ThermalPrinterBaseTemplate
    {
        public ThermalPrinterBaseInternalTemplate(sys_configurationprinters pPrinter)
            : base(pPrinter, SettingsApp.PrinterThermalImageCompanyLogo)
        {
        }

        //Override Parent Template
        public override bool Print()
        {
            bool result = false;

            try
            {
                //Call Base Template PrintHeader
                base.PrintHeader(true); /* IN009055 - when is Order, true */

                //Call Child Content (Overrided)
                PrintContent();

                //Call PrintFooterExtended
                PrintFooterExtended();
                //Call Base Template PrintFooter
                base.PrintFooter();

                //End Job
                PrintBuffer();

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public void PrintFooterExtended()
        {
            //Align Center
            _thermalPrinterGeneric.SetAlignCenter();

            //Extended Footer Text
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1"));
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer2"));
            _thermalPrinterGeneric.LineFeed();
            _thermalPrinterGeneric.WriteLine(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer3"));

            //Reset to Left
            _thermalPrinterGeneric.SetAlignLeft();

            //Line Feed
            _thermalPrinterGeneric.LineFeed();
        }
    }
}
