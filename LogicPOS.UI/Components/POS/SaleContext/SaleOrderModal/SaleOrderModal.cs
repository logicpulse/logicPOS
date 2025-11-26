using Gtk;
using LogicPOS.Api.Features.Documents;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public class SaleOrderModal : Modal
    {
        private IconButtonWithText BtnPrintOrder { get; set; }
        private IconButtonWithText BtnTableConsult { get; set; }
        public SaleOrderModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_orders"),
                                                   new Size(500, 220),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_orders.png")
        {
           
        }

        private void InitializeButtons()
        {
            BtnTableConsult = DesignButton(BtnTableConsult, GeneralUtils.GetResourceByName("dialog_orders_button_label_table_consult"),
                                                                           AppSettings.Paths.Images + @"Icons\icon_pos_table_view_order.png",
                                                                           new Size(240, 150));
            BtnTableConsult.Clicked += BtnTableConsult_Clicked;

            BtnPrintOrder = DesignButton(BtnPrintOrder, GeneralUtils.GetResourceByName("dialog_orders_button_label_print_order"),
                                                                        AppSettings.Paths.Images + @"Icons\icon_pos_print.png",
                                                                        new Size(240, 150));
            BtnPrintOrder.Clicked += BtnPrintOrder_Clicked;
        }

        private IconButtonWithText DesignButton(Button button, string buttonLabel, string buttonIcon, Size buttonSize)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonUserId_Green",
                    Text = buttonLabel,
                    Icon = buttonIcon,
                    ButtonSize = buttonSize
                });
        }


        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            return new ActionAreaButtons
                {

                    new ActionAreaButton(BtnPrintOrder, ResponseType.None),

                    new ActionAreaButton(BtnTableConsult, ResponseType.None)
                 };
        }
        
        protected override Widget CreateBody()
        {
            HBox hbox = new HBox(false, 0);
            hbox.PackStart(BtnPrintOrder, true, true, 0);
            hbox.PackStart(BtnTableConsult, true, true, 0);
            return hbox;
        }
       
        private void BtnPrintOrder_Clicked(object sender, EventArgs e)
        {
            if (SaleContext.CurrentOrder.Tickets.Count() == 0 || SaleContext.CurrentOrder.Tickets==null)
            {
                SimpleAlerts.Information()
                            .WithTitle("Mesa vazia")
                            .WithMessage("Não existem pedidos associados a esta mesa")
                            .ShowAlert();
                return;
            }
            ThermalPrintingService.PrintTicket(new Printing.Thermal.Printers.TicketPrintingData
            {
                Number = SaleContext.CurrentOrder.Tickets.Last().Number,
                Place = SaleContext.CurrentTable.Place,
                Table = SaleContext.CurrentTable.Designation,
                Items = SaleContext.CurrentOrder.Tickets.Last().Items.Select(i => new Printing.Thermal.Printers.TicketItem
                {
                    Article = i.Article.Designation,
                    Quantity = i.Quantity,
                    Unit = i.Article.Unit
                }).ToList()
            });
        }

        private void BtnTableConsult_Clicked(object sender, EventArgs e)
        {

            var command = new IssueDocumentCommand();
            var details = SaleContext.CurrentOrder.GetDocumentDetails().ToList();

            if (details == null || details.Count == 0)
            {
                SimpleAlerts.Information()
                            .WithTitle("Mesa vazia")
                            .WithMessage("Não existem pedidos associados a esta mesa")
                            .ShowAlert();
                return;
            }

            var country = CountriesService.Default;
            var customer = CustomersService.Default;
            command.Type = (country.Code2.ToUpper() == "AO") ? "CM" : "DC";

            if(customer!=null)
                command.CustomerId = customer.Id;

            if (command.CustomerId == null)
            {
                command.Customer = new DocumentCustomer
                {
                    Name = GeneralUtils.GetResourceByName("global_final_consumer"),
                    FiscalNumber = customer.FiscalNumber,
                    Country = country.Code2,
                    CountryId = country.Id
                };
            }
            command.Details = details;

            var printingData = DocumentsService.IssueDocumentForPrinting(command);
           
            if (printingData == null)
            {
                return;
            }  

            ThermalPrintingService.PrintInvoice(printingData.Value);

        }
    }
}
