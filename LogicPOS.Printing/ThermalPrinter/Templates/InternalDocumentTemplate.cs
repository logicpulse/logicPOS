using System;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;


namespace LogicPOS.Printing.Templates
{
    public class InternalDocumentTemplate : BaseInternalTemplate
    {
        public InternalDocumentTemplate(PrinterDto printer,
            string terminalDesignation,
            string userName,
            CompanyPrintingInformationsDto companyInformationsDto)
            : base(printer, companyInformationsDto, terminalDesignation, userName)
        {
            _ticketTitle = "DYNAMIC TITLE";
        }

        public override void PrintContent()
        {
            try
            {
                //Call Base Template PrintHeader
                PrintTitles();
                
                //Align Center
                _printer.SetAlignCenter();

                //Content
                _printer.WriteLine("REPLACE CONTENT STUB");

                //Reset to Left
                _printer.SetAlignLeft();

                //Line Feed
                _printer.LineFeed();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
