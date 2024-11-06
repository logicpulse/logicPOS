using LogicPOS.Api.Entities;
using LogicPOS.DTOs.Printing;
using LogicPOS.DTOs.Reporting;
using LogicPOS.Printing.Documents;
using System.Collections.Generic;

namespace LogicPOS.Printing.Utility
{
    public class ThermalPrinting 
    {
        protected PrinterDto PrinterDto { get; set; }
        public List<int> DocCopyName { get; private set; }

        protected List<PrintOrderDetailDto> OrderDetails;
        PrintOrderTicketDto OrderTicketDto;
        protected string TerminalDesignation { get; set; }
        protected string UserName { get; set; }
        protected readonly CompanyInformationsDto CompanyInformationsDto;
        protected readonly Document DocumentMaster;

        public ThermalPrinting(PrinterDto printerDto,CompanyInformationsDto companyInformationsDto, List<int> docCopyName, Document documentMaster, string terminalDesignation, string userDesignation)
        {
            PrinterDto = printerDto;
            DocCopyName = docCopyName;
            CompanyInformationsDto = companyInformationsDto;
            DocumentMaster = documentMaster;
            UserName = userDesignation;
            TerminalDesignation= terminalDesignation;
            PrintInvoice();
        }  
        
        public ThermalPrinting(PrinterDto printerDto, CompanyInformationsDto companyInformationsDto, PrintOrderTicketDto orderTicketDto, string terminalDesignation, string userDesignation)
        {
            PrinterDto = printerDto;
            CompanyInformationsDto = companyInformationsDto;
            OrderTicketDto = orderTicketDto;
            TerminalDesignation = terminalDesignation;
            UserName= userDesignation;
            PrintOrder();
        }

        public void PrintOrder()
        {
            if (OrderTicketDto.OrderDetails.Count > 0)
            {
                foreach (var item in OrderTicketDto.OrderDetails)
                {
                    item.ArticlePrinter = PrinterDto;

                }
                OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(PrinterDto, OrderTicketDto,TerminalDesignation,UserName,CompanyInformationsDto, true);
                thermalPrinterInternalDocumentOrderRequest.Print();
            }
        }



        public void PrintInvoice()
        {

            var F = new FinanceMaster(
             PrinterDto,
             TerminalDesignation,
             UserName,
             DocumentMaster,
             CompanyInformationsDto,
             DocCopyName,
             true,
             "Original"
            );
            F.Print();
        }



    }
}
