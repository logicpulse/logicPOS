using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.DTOs.Printing;
using System.Collections.Generic;

namespace LogicPOS.Data.XPO.Utility
{
    public static class MappingUtils
    {
        public static PrinterDto GetPrinterDto(sys_configurationprinters printer)
        {
            return new PrinterDto
            {
                Id = printer.Oid,
                Designation = printer.Designation,
                NetworkName = printer.NetworkName,
                Token = printer.PrinterType.Token,
                IsThermal = printer.PrinterType.ThermalPrinter
            };
        }

        public static PrintOrderTicketDto GetPrintOrderTicketDto(fin_documentorderticket ticket)
        {
            var ticketDto =  new PrintOrderTicketDto
            {
                TicketId = ticket.TicketId,
                TableDesignation = ticket.OrderMain.PlaceTable.Designation,
                PlaceDesignation = ticket.OrderMain.PlaceTable.Place.Designation
            };

            ticketDto.OrderDetails = new List<PrintOrderDetailDto>();

            foreach (var detail in ticket.OrderDetail)
            {
                ticketDto.OrderDetails.Add(new PrintOrderDetailDto
                {
                    Designation = detail.Designation,
                    Quantity = detail.Quantity,
                    UnitMeasure = detail.UnitMeasure,
                    ArticlePrinter = GetPrinterDto(detail.Article.Printer)
                });
            }

            return ticketDto;
        }

        public static PrintingDocumentTypeDto GetPrintingDocumentTypeDto(fin_documentfinancetype documentType)
        {
            return new PrintingDocumentTypeDto
            {
                Id = documentType.Oid,
                IsSaftDocumentTypePayments = documentType.SaftDocumentType == logicpos.datalayer.Enums.SaftDocumentType.Payments,
                PrintCopies = documentType.PrintCopies
            };
        }

        public static PrintingFinancePaymentDto GetPrintingFinancePaymentDto(fin_documentfinancepayment financePayment)
        {
            return new PrintingFinancePaymentDto
            {
                Id = financePayment.Oid,
                DocumentType = GetPrintingDocumentTypeDto(financePayment.DocumentType),
                ExchangeRate = financePayment.ExchangeRate,
                ExtendedValue = financePayment.ExtendedValue
            };
        }

        public static PrintDocumentMasterDto GetPrintDocumentMasterDto(fin_documentfinancemaster documentMaster)
        {
            return new PrintDocumentMasterDto
            {
                Id = documentMaster.Oid,
                ATDocQRCode = documentMaster.ATDocQRCode,
                Hash = documentMaster.Hash,
                TableDesignation = documentMaster.SourceOrderMain.PlaceTable.Designation,
                PlaceDesignation = documentMaster.SourceOrderMain.PlaceTable.Place.Designation,
                DocumentType = GetPrintingDocumentTypeDto(documentMaster.DocumentType),
                HasValidPaymentMethod = (documentMaster.PaymentMethod != null)
            };
        }
    }
}
