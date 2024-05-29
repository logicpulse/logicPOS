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
    }
}
