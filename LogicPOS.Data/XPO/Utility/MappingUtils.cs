using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.DTOs.Common;
using LogicPOS.DTOs.Printing;
using System.Collections.Generic;

namespace LogicPOS.Data.XPO.Utility
{
    public static class MappingUtils
    {
        public static PrintingPrinterDto GetPrinterDto(sys_configurationprinters printer)
        {
            return new PrintingPrinterDto
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
                var orderDetailDto = GetPrintOrderDetailDto(detail);
                ticketDto.OrderDetails.Add(orderDetailDto);
            }

            return ticketDto;
        }

        public static PrintOrderDetailDto GetPrintOrderDetailDto(fin_documentorderdetail detail)
        {
            var orderDetailDto =  new PrintOrderDetailDto
            {
                Designation = detail.Designation,
                Quantity = detail.Quantity,
                UnitMeasure = detail.UnitMeasure
            };

            if(detail.Article != null && detail.Article.Printer != null)
            {
                orderDetailDto.ArticlePrinter = GetPrinterDto(detail.Article.Printer);
            }

            return orderDetailDto;
        }

        public static PrintingDocumentTypeDto GetPrintingDocumentTypeDto(fin_documentfinancetype documentType)
        {
            return new PrintingDocumentTypeDto
            {
                Id = documentType.Oid,
                IsSaftDocumentTypePayments = documentType.SaftDocumentType == SaftDocumentType.Payments,
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

        public static PrintWorkSessionDto GetPrintWorkSessionDto(pos_worksessionperiod workSessionPeriod)
        {
            return new PrintWorkSessionDto
            {
                Id = workSessionPeriod.Oid,
                PeriodType = workSessionPeriod.PeriodType.ToString(),
                SessionStatus = workSessionPeriod.SessionStatus.ToString(),
                TerminalDesignation = workSessionPeriod.Terminal.Designation,
                StartDate = workSessionPeriod.DateStart
            };
        }

        public static CurrenyDto GetCurrencyDto(cfg_configurationcurrency currency)
        {
            return new CurrenyDto
            {
                Id = currency.Oid,
                Acronym = currency.Acronym,
                ExchangeRate = currency.ExchangeRate
            };
        }


    }
}
