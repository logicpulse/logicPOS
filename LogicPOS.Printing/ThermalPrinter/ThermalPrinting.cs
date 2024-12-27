using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.DTOs.Printing;
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
        protected readonly CompanyPrintingInformationsDto CompanyInformationsDto;
        protected Document DocumentMaster;
        protected WorkSessionData WorkSessionDocumentsData;
        protected WorkSessionData WorkSessionReceiptsData;


        public ThermalPrinting(PrinterDto printerDto, string terminalDesignation, string userName, CompanyPrintingInformationsDto companyPrintingInformationsDto)
        {
            PrinterDto = printerDto;
            TerminalDesignation = terminalDesignation;
            UserName = userName;
            CompanyInformationsDto = companyPrintingInformationsDto;
        }
        public void ThermalWorkSessionPrinting(WorkSessionData workSessionDocumentsData, WorkSessionData workSessionReceiptsData)
        {
            WorkSessionReceiptsData = workSessionReceiptsData;
            WorkSessionDocumentsData = workSessionDocumentsData;
            PrintWorkSession();
        }

        public void ThermalOrderPrinting(PrintOrderTicketDto orderTicketDto)
        {
            OrderTicketDto = orderTicketDto;
            PrintOrder();
        }



        public void ThermalDocumentPrinting(List<int> docCopyName, Document documentMaster)
        {
            DocCopyName = docCopyName;
            DocumentMaster = documentMaster;
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
            var workSession = new WorkSession(PrinterDto, TerminalDesignation, UserName, WorkSessionDocumentsData, WorkSessionReceiptsData, CompanyInformationsDto);
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

        public bool PrintCashDrawerOpenAndMoneyInOut(
         string pTicketTitle,
            decimal pTotalAmountInCashDrawer,
            decimal pMovementAmount,
        string pMovementDescription)
        {
            bool result = false;

            if (PrinterDto != null)
            {
                switch (PrinterDto.Token)
                {
                    case "THERMAL_PRINTER_WINDOWS":
                        CashDrawer internalDocumentCashDrawer = new CashDrawer(
                            PrinterDto,
                            pTicketTitle,
                            pTotalAmountInCashDrawer,
                            pMovementAmount,
                            TerminalDesignation,
                            UserName,
                            CompanyInformationsDto);


                        internalDocumentCashDrawer.Print();
                        break;
                }
                result = true;
            }
            return result;
        }

    }
}