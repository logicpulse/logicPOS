using Gtk;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosOrdersDialog : BaseDialog
    {
        public PosOrdersDialog(Window parentWindow, DialogFlags pDialogFlags, string pTable)
            : base(parentWindow, pDialogFlags)
        {
            //Init Local Vars
            string windowTitle = string.Format("{0} : {1} #{2}", GeneralUtils.GetResourceByName("window_title_dialog_orders"), GeneralUtils.GetResourceByName("global_place_table"), pTable);
            Size windowSize = new Size(429, 205);//618 (3buts)
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_orders.png";

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Icons
            string fileIconListOrders = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_ticketpad_orderlist.png";
            string fileIconPrintOrder = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_print.png";
            //String fileIconListFinanceDocuments = SharedUtils.OSSlash(GeneralSettings.Path["images"] + @"Icons\icon_pos_default.png");

            //Buttons
            IconButtonWithText buttonPrintOrder = new IconButtonWithText(
                new ButtonSettings {
                    Name = "touchButtonPrintOrder_Green",
                    BackgroundColor = ColorSettings.DefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_print_order"),
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
                    Icon = fileIconPrintOrder,
                    IconSize = sizeIcon,
                    ButtonSize = new Size(buttonWidth, buttonHeight)
                    }
                );

            IconButtonWithText buttonTableConsult = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonListOrders_Green",
                    BackgroundColor = ColorSettings.DefaultButtonBackground,
                    Text = GeneralUtils.GetResourceByName("dialog_orders_button_label_table_consult"),
                    Font = FontSettings.Button,
                    FontColor = ColorSettings.DefaultButtonFont,
                    Icon = fileIconListOrders,
                    IconSize = sizeIcon,
                    ButtonSize = new Size(buttonWidth, buttonHeight)
                });

            //Table
            Table table = new Table(1, 2, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(buttonPrintOrder, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonTableConsult, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
           
            //Init Object
            this.Initialize(this,
                            pDialogFlags,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            table,
                            null);

            //Events
            buttonPrintOrder.Clicked += buttonPrintOrder_Clicked;
            buttonTableConsult.Clicked += buttonTableConsult_Clicked;
            //buttonListFinanceDocuments.Clicked += buttonListFinanceDocuments_Clicked;

            // Enable/Disable PrintTicket based on Printer Type, Currently PrintTicket is only Implemented in Thermal Printers
            //TK016249 - Impressoras - Diferenciação entre Tipos
            bool printerMissing = (TerminalSettings.HasLoggedTerminal && TerminalSettings.LoggedTerminal.ThermalPrinter.PrinterType.Token.StartsWith("THERMAL_PRINTER_"));
            buttonPrintOrder.Sensitive = printerMissing;
            buttonTableConsult.Sensitive = printerMissing;
        }
    }
}

