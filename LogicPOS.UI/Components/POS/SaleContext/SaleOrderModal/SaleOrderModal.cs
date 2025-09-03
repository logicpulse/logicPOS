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
        private readonly PosTicket _ticket;
        public SaleOrderModal(Window parent, PosTicket ticket) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_orders"),
                                                   new Size(500, 220),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_orders.png")
        {
            _ticket = ticket;
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

            ThermalPrintingService.PrintTicket(_ticket, SaleContext.CurrentTable);
        }

        private void BtnTableConsult_Clicked(object sender, EventArgs e)
        {

            var command = new AddDocumentCommand();
            var details = SaleContext.CurrentOrder.GetDocumentDetails().ToList();

            if (details == null || details.Count == 0)
            {
                SimpleAlerts.Information()
                            .WithTitle("Mesa vazia")
                            .WithMessage("Não existem pedidos associados a esta mesa")
                            .ShowAlert();
                return;
            }

            var country = CountriesService.Countries.FirstOrDefault(c => c.Code2 == PreferenceParametersService.CompanyInformations.CountryCode2);
            var customer = CustomersService.Default;
            command.Type = (country.Code2.ToUpper() == "AO") ? "CM" : "DC";

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
