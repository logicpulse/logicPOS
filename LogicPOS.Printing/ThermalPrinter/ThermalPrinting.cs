using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using System;
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
        protected readonly CompanyPrintingInformationsDto CompanyInformationsDto;
        protected readonly Document DocumentMaster;
        protected readonly WorkSessionData WorkSessionDocumentsData;
        protected readonly WorkSessionData WorkSessionReceiptsData;


        public ThermalPrinting(PrinterDto printerDto, string terminalDesignation,string userName, CompanyPrintingInformationsDto companyPrintingInformationsDto, WorkSessionData workSessionDocumentsData, WorkSessionData workSessionReceiptsData)
        {
            PrinterDto = printerDto;
            WorkSessionReceiptsData = workSessionReceiptsData;
            WorkSessionDocumentsData = workSessionDocumentsData;
            TerminalDesignation = terminalDesignation;
            UserName = userName;
            CompanyInformationsDto = companyPrintingInformationsDto;
            PrintWorkSession();
        }

        public ThermalPrinting(PrinterDto printerDto, CompanyPrintingInformationsDto companyInformationsDto, PrintOrderTicketDto orderTicketDto, string terminalDesignation, string userName)
        {
            PrinterDto = printerDto;
            CompanyInformationsDto = companyInformationsDto;
            OrderTicketDto = orderTicketDto;
            TerminalDesignation = terminalDesignation;
            UserName = userName;
            PrintOrder();
        }



        public ThermalPrinting(PrinterDto printerDto, CompanyPrintingInformationsDto companyInformationsDto, List<int> docCopyName, Document documentMaster, string terminalDesignation, string userName)
        {
            PrinterDto = printerDto;
            DocCopyName = docCopyName;
            CompanyInformationsDto = companyInformationsDto;
            DocumentMaster = documentMaster;
            UserName = userName;
            TerminalDesignation = terminalDesignation;
            PrintDocument();
        }


        public void PrintOrder()
        {
            if (OrderTicketDto.OrderDetails.Count > 0)
            {
                foreach (var item in OrderTicketDto.OrderDetails)
                {
                    item.ArticlePrinter = PrinterDto;

                }
                OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(PrinterDto, OrderTicketDto, TerminalDesignation, UserName, CompanyInformationsDto, true);
                thermalPrinterInternalDocumentOrderRequest.Print();
            }
        }
        public void PrintWorkSession()
        {
            var workSession = new WorkSession(PrinterDto, TerminalDesignation,UserName,WorkSessionDocumentsData, WorkSessionReceiptsData, CompanyInformationsDto);
            workSession.Print();
        }

        public void PrintDocument()
        {

            var document = new FinanceMaster(
             PrinterDto,
             TerminalDesignation,
             UserName,
             DocumentMaster,
             CompanyInformationsDto,
             DocCopyName,
             true,
             "Original"
            );
            document.Print();
        }



    }
}
