using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.GetCompanyInformations;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Reports.WorkSession.Common;
using LogicPOS.Api.Features.Reports.WorkSession.GetWorkSessionData;
using LogicPOS.Api.Features.WorkSessions.GetLastClosedDay;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Documents;
using LogicPOS.Printing.Utility;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.POS.PrintingContext
{
    public static class PrintingServices
    {

        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        private static async Task<ErrorOr<WorkSessionData>> GetWorkSessionDocumentData(CancellationToken ct = default)
        {
            var lastWorkSessionPeriod = await GetLastWorkSessionDayClosed(ct);
            var commandDocuments = new GetWorkSessionDocumentsDataQuery(lastWorkSessionPeriod.Value.Id);
            return await _mediator.Send(commandDocuments, ct);
        }

        private static async Task<ErrorOr<WorkSessionData>> GetWorkSessionReceiptData(CancellationToken ct = default)
        {
            var lastWorkSessionPeriod = await GetLastWorkSessionDayClosed(ct);
            var commandReceipts = new GetWorkSessionReceiptsDataQuery(lastWorkSessionPeriod.Value.Id);
            return await _mediator.Send(commandReceipts, ct);
        }
        private static async Task<ErrorOr<WorkSessionPeriod>> GetLastWorkSessionDayClosed(CancellationToken cancellationToken)
        {
            var command = new GetLastClosedDayQuery();
            return await _mediator.Send(command, cancellationToken);
        }
        public static void PrintWorkSessionDayReport()
        {

            var workSessionDocumentsData = GetWorkSessionDocumentData().Result;
            var workSessionReceiptsData = GetWorkSessionReceiptData().Result;
            var companyInformations = GetCompanyPrintingInformation();

            if (workSessionDocumentsData.IsError || workSessionReceiptsData.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(workSessionDocumentsData.FirstError.Description)
                            .WithMessage(workSessionReceiptsData.FirstError.Description)
                            .ShowAlert();
                return;
            }

            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null)
            {
                return;
            }

            var print = new ThermalPrinting(printer, TerminalService.Terminal.Designation, AuthenticationService.User.Name, companyInformations);
            print.ThermalWorkSessionPrinting(workSessionDocumentsData.Value, workSessionReceiptsData.Value);
        }

        private static PrinterDto GetTerminalThermalPrinter(Terminal terminal)
        {

            if (terminal.ThermalPrinter != null)
            {
                return new PrinterDto()
                {
                    Designation = terminal.ThermalPrinter.Designation,
                    Token = terminal.ThermalPrinter.Type.Token,
                    IsThermal = terminal.ThermalPrinter.Type.ThermalPrinter,
                    CutCommand = "0x42,0x00"
                };
            }
            else
            {
                return null;
            }
        }

        private static CompanyPrintingInformationsDto GetCompanyPrintingInformation()
        {
            var result = _mediator.Send(new GetCompanyInformationsQuery()).Result;
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }
            var companyInformations = result.Value;
            return new CompanyPrintingInformationsDto()
            {
                Address = companyInformations.Address,
                Name = companyInformations.Name,
                BusinessName = companyInformations.BussinessName,
                ComercialName = companyInformations.ComercialName,
                City = companyInformations.City,
                Country = companyInformations.CountryCode2,
                Logo = companyInformations.LogoBmp,
                Email = companyInformations.Email,
                MobilePhone = companyInformations.MobilePhone,
                PostalCode = companyInformations.PostalCode,
                FiscalNumber = companyInformations.FiscalNumber,
                Phone = companyInformations.Phone,
                StockCapital = companyInformations.StockCapital,
                Website = companyInformations.Website,
                DocumentFinalLine1 = companyInformations.DocumentFinalLine1,
                DocumentFinalLine2 = companyInformations.DocumentFinalLine2,
                TicketFinalLine1 = companyInformations.TicketFinalLine1,
                TicketFinalLine2 = companyInformations.TicketFinalLine2,
            };
        }

        public static void PrintOrder(SaleItemsPage itemsPage)
        {

            var orderTicket = GetOrderTicket(itemsPage);

            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null)
            {
                return;
            }
            CompanyPrintingInformationsDto companyInformationsDto = GetCompanyPrintingInformation();
            var print = new ThermalPrinting(printer, TerminalService.Terminal.Designation, AuthenticationService.User.Name, companyInformationsDto);
            print.ThermalOrderPrinting(orderTicket);

        }
        private static PrintOrderTicketDto GetOrderTicket(SaleItemsPage itemsPage)
        {
            var orderTicket = new PrintOrderTicketDto();
            orderTicket.OrderDetails = new List<PrintOrderDetailDto>();

            orderTicket.TicketId = (int)itemsPage.Ticket.Number;
            orderTicket.TableDesignation = SaleContext.CurrentOrder.Table.Designation;
            orderTicket.PlaceDesignation = SaleContext.CurrentOrder.Table.Place.Designation;
            foreach (var item in itemsPage.Ticket.Items)
            {
                orderTicket.OrderDetails.Add(new PrintOrderDetailDto()
                {
                    Designation = item.Article.Designation,
                    Quantity = item.Quantity,
                    UnitMeasure = item.Article.MeasurementUnit.Acronym
                });
            }

            return orderTicket;
        }

        public static void PrintDocument(Guid id)
        {
            var result = _mediator.Send(new GetDocumentByIdQuery(id)).Result;
            if (result.IsError)
            {
                CustomAlerts.Error()
                            .WithMessage(result.FirstError.Description)
                            .ShowAlert();
            }
            var document = result.Value;
            List<int> docCopyName = new List<int>();
            docCopyName.Add(0);
            PrinterDto printer = GetTerminalThermalPrinter(TerminalService.Terminal);
            if (printer == null)
            {
                return;
            }
            CompanyPrintingInformationsDto companyInformationsDto = GetCompanyPrintingInformation();
            var print = new ThermalPrinting(printer, TerminalService.Terminal.Designation, AuthenticationService.User.Name, companyInformationsDto);
            print.ThermalDocumentPrinting(docCopyName, document);
        }
    }
}
