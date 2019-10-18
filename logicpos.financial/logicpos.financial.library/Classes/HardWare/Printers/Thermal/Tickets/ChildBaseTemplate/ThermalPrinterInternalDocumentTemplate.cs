using System;
using logicpos.datalayer.DataLayer.Xpo;

//Used this to SubClass ThermalPrinterBaseInternalTemplate

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal.Tickets
{
    public class ThermalPrinterInternalDocumentTemplate : ThermalPrinterBaseInternalTemplate
    {
        public ThermalPrinterInternalDocumentTemplate(sys_configurationprinters pPrinter)
            : base(pPrinter)
        {
            _ticketTitle = "DYNAMIC TITLE";
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                base.PrintTitles();
                
                //Align Center
                _thermalPrinterGeneric.SetAlignCenter();

                //Content
                _thermalPrinterGeneric.WriteLine("REPLACE CONTENT STUB");

                //Reset to Left
                _thermalPrinterGeneric.SetAlignLeft();

                //Line Feed
                _thermalPrinterGeneric.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
