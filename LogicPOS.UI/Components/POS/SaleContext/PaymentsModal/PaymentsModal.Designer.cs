using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using Gtk;
using LogicPOS.UI.Extensions;
using System.Drawing;
using LogicPOS.UI.Components.Menus;
using LogicPOS.UI.Settings;


namespace LogicPOS.UI.Components.POS
{
    public partial class PaymentsModal
    {
        
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            InitializeLabels();

            return new ActionAreaButtons
                {
                    new ActionAreaButton(BtnInvoice, ResponseType.None),
                    new ActionAreaButton(BtnClearCustomer, ResponseType.None),
                    new ActionAreaButton(BtnNewCustomer, ResponseType.None),
                    new ActionAreaButton(BtnFullPayment, ResponseType.None),
                    new ActionAreaButton(BtnPartialPayment, ResponseType.None),
                    new ActionAreaButton(BtnOk, ResponseType.Ok),
                    new ActionAreaButton(BtnCancel, ResponseType.Cancel)
                };
        }

        protected override Widget CreateBody()
        {
            InitializeTextFields();
            VBox verticalLayout = new VBox(false, 0);
            verticalLayout.PackStart(CreateOldTopPanel(), true, true, 0);

            verticalLayout.PackStart(TextBox.CreateHbox(TxtFiscalNumber, TxtCardNumber), true, true, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtCustomer, TxtDiscount), true, true, 0);
            verticalLayout.PackStart(TxtAddress.Component, true, true, 0);
            verticalLayout.PackStart(TxtLocality.Component, true, true, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtZipCode, TxtCity, TxtCountry), true, true, 0);
            verticalLayout.PackStart(TxtNotes.Component, true, true, 0);

            return verticalLayout;
        }

        private IconButtonWithText CreatePaymentMethodButton(string text, string iconPath)
        {
            var font = AppSettings.Instance.FontBaseDialogButton;
            var fontColor = AppSettings.Instance.ColorBaseDialogDefaultButtonFont;
            var buttonIconSize = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon;
            var buttonSize = AppSettings.Instance.SizeBaseDialogDefaultButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = text,
                    Font = font,
                    FontColor = fontColor,
                    Icon = iconPath,
                    IconSize = buttonIconSize,
                    ButtonSize = buttonSize
                });
        }

        private EventBox CreateOldTotalsTable()
        {
            uint padding = 9;
            Gtk.Table table = new Gtk.Table(3, 2, false);
            table.HeightRequest = 132;

            //Row 1
            table.Attach(LabelTotal, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelTotalValue, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 2
            table.Attach(LabelDelivery, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelDeliveryValue, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 3
            table.Attach(LabelChange, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(LabelChangeValue, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            EventBox eventBox = new EventBox();
            eventBox.BorderWidth = 4;
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.ColorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
            eventBox.Add(table);

            return eventBox;
        }

        private Gtk.Table CreateOldPaymentMethodsTable()
        {
            uint padding = 0;
            Gtk.Table table = new Gtk.Table(2, 3, true) { BorderWidth = 2 };

            //Row 1
            table.Attach(BtnMoney, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnMB, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnDebitCard, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnVisa, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //Row 2
            table.Attach(BtnCheck, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCreditCard, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCurrentAccountMethod, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            table.Attach(BtnCustomerCard, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            return table;
        }

        private HBox CreateOldTopPanel()
        {
            HBox topPanel = new HBox(false, 0);
            topPanel.PackStart(CreateOldPaymentMethodsTable(), true, true, 0);
            topPanel.PackStart(CreateOldTotalsTable(), true, true, 0);
            return topPanel;
        }

    }
}
