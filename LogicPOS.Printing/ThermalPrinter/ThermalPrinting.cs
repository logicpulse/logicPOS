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
        protected readonly WorkSessionData WorkSessionData;
        protected readonly PrintWorkSessionDto PrintWorkSessionDto;



        public ThermalPrinting(PrinterDto printerDto, string terminalDesignation, WorkSessionData workSessionData, PrintWorkSessionDto printWorkSessionDto)
        {
            PrinterDto = printerDto;
            WorkSessionData = workSessionData;
            PrintWorkSessionDto = printWorkSessionDto;
            TerminalDesignation = terminalDesignation;
            PrintWorkSession();
        }

        public ThermalPrinting(PrinterDto printerDto, CompanyPrintingInformationsDto companyInformationsDto, List<int> docCopyName, Document documentMaster, string terminalDesignation, string userDesignation)
        {
            PrinterDto = printerDto;
            DocCopyName = docCopyName;
            CompanyInformationsDto = companyInformationsDto;
            DocumentMaster = documentMaster;
            UserName = userDesignation;
            TerminalDesignation = terminalDesignation;
            PrintDocument();
        }

        public ThermalPrinting(PrinterDto printerDto, CompanyPrintingInformationsDto companyInformationsDto, PrintOrderTicketDto orderTicketDto, string terminalDesignation, string userDesignation)
        {
            PrinterDto = printerDto;
            CompanyInformationsDto = companyInformationsDto;
            OrderTicketDto = orderTicketDto;
            TerminalDesignation = terminalDesignation;
            UserName = userDesignation;
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
                OrderRequest thermalPrinterInternalDocumentOrderRequest = new OrderRequest(PrinterDto, OrderTicketDto, TerminalDesignation, UserName, CompanyInformationsDto, true);
                thermalPrinterInternalDocumentOrderRequest.Print();
            }
        }
        public void PrintWorkSession()
        {
            var workDto = new PrintWorkSessionDto()
            {
                TerminalDesignation = TerminalDesignation,
                StartDate = DateTime.Now,
                PeriodType = WorkSessionPeriodType.Terminal.ToString(),
                SessionStatus = WorkSessionPeriodStatus.Open.ToString(),
            };
            var workS = new WorkSession(PrinterDto, TerminalDesignation, WorkSessionData, PrintWorkSessionDto);
            workS.Print();
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
