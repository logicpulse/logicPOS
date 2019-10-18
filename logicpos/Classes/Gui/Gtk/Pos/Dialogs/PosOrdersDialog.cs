using System;
using Gtk;
using System.Drawing;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.App;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosOrdersDialog : PosBaseDialog
    {
        public PosOrdersDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pTable)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = string.Format("{0} : {1} #{2}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_orders"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_place_table"), pTable);
            Size windowSize = new Size(429, 205);//618 (3buts)
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_orders.png");

            Size sizeIcon = new Size(50, 50);
            int buttonWidth = 162;
            int buttonHeight = 88;
            uint tablePadding = 15;

            //Icons
            String fileIconListOrders = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_ticketpad_orderlist.png");
            String fileIconPrintOrder = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_print.png");
            //String fileIconListFinanceDocuments = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_default.png");

            //Buttons
            TouchButtonIconWithText buttonPrintOrder = new TouchButtonIconWithText("touchButtonPrintOrder_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_print_order"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconPrintOrder, sizeIcon, buttonWidth, buttonHeight);
            TouchButtonIconWithText buttonTableConsult = new TouchButtonIconWithText("touchButtonListOrders_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_orders_button_label_table_consult"), _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconListOrders, sizeIcon, buttonWidth, buttonHeight);
            //TouchButtonIconWithText buttonListFinanceDocuments = new TouchButtonIconWithText("touchButtonListFinanceDocuments_Green", _colorBaseDialogDefaultButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_button_label_select_record_finance_documents, _fontBaseDialogButton, _colorBaseDialogDefaultButtonFont, fileIconPrintOrder, sizeIcon, buttonWidth, buttonHeight);

            //Table
            Table table = new Table(1, 2, true);
            table.BorderWidth = tablePadding;
            //Row 1
            table.Attach(buttonPrintOrder, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            table.Attach(buttonTableConsult, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);
            //table.Attach(buttonListFinanceDocuments, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, tablePadding, tablePadding);

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, table, null);

            //Events
            buttonPrintOrder.Clicked += buttonPrintOrder_Clicked;
            buttonTableConsult.Clicked += buttonTableConsult_Clicked;
            //buttonListFinanceDocuments.Clicked += buttonListFinanceDocuments_Clicked;

            // Enable/Disable PrintTicket based on Printer Type, Currently PrintTicket is only Implemented in Thermal Printers
			//TK016249 - Impressoras - Diferenciação entre Tipos
            bool printerMissing = (GlobalFramework.LoggedTerminal.ThermalPrinter != null && GlobalFramework.LoggedTerminal.ThermalPrinter.PrinterType.Token.StartsWith("THERMAL_PRINTER_"));
            buttonPrintOrder.Sensitive = printerMissing;
            buttonTableConsult.Sensitive = printerMissing;
        }
    }
}

