using System;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.DTOs.Printing;

//Used this to SubClass ThermalPrinterBaseInternalTemplate

namespace LogicPOS.Printing.Templates
{
    public class ThermalPrinterInternalDocumentTemplate : ThermalPrinterBaseInternalTemplate
    {
        public ThermalPrinterInternalDocumentTemplate(PrinterReferenceDto printer)
            : base(printer)
        {
            _ticketTitle = "DYNAMIC TITLE";
        }

        //Override Parent Template
        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                PrintTitles();
                
                //Align Center
                _genericThermalPrinter.SetAlignCenter();

                //Content
                _genericThermalPrinter.WriteLine("REPLACE CONTENT STUB");

                //Reset to Left
                _genericThermalPrinter.SetAlignLeft();

                //Line Feed
                _genericThermalPrinter.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
