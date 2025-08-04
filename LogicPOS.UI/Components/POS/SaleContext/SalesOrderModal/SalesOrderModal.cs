using Gtk;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public class SalesOrderModal : Modal
    {
        public dynamic Theme { get; set; }
        private IconButtonWithText BtnPrintOrder { get; set; }
        private IconButtonWithText BtnTableConsult { get; set; }
                        
        public SalesOrderModal(Window parent) : base(parent,
                                                   GeneralUtils.GetResourceByName("window_title_dialog_orders"),
                                                   new Size(500, 250),
                                                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_orders.png")
        {
           

        }

        private IconButtonWithText InitializeButton(Button button, string buttonLabel, string buttonIcon, Size buttonSize)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonUserId_Green",
                    Text = buttonLabel,
                   // Font = Theme.Font,
                    //BackgroundColor=Color.GreenYellow,
                    Icon = buttonIcon,
                    //IconSize = (Theme.IconSize as string).ToSize(),
                    ButtonSize = buttonSize
                });
        }


        protected override ActionAreaButtons CreateActionAreaButtons()
        {

            return new ActionAreaButtons
                {

                    new ActionAreaButton(InitializeButton(BtnPrintOrder,GeneralUtils.GetResourceByName("dialog_orders_button_label_print_order"),
                                                                        AppSettings.Paths.Images + @"Icons\icon_pos_print.png",
                                                                        new Size(240, 150))
                                                                        , ResponseType.None),

                    new ActionAreaButton(InitializeButton(BtnTableConsult,GeneralUtils.GetResourceByName("dialog_orders_button_label_table_consult"),
                                                                          AppSettings.Paths.Images + @"Icons\icon_pos_table_view_order.png",
                                                                          new Size(240, 150)), ResponseType.None)
                 };
        }

        protected override Widget CreateBody()
        {
            HBox hbox = new HBox(false, 0);
            hbox.PackStart(BtnPrintOrder, true, true, 0);
            hbox.PackStart(BtnTableConsult, true, true, 0);
            return hbox;
        }
    }
}
