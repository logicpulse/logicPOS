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
            verticalLayout.PackStart(CreateTotalsTable(), true, true, 0);
            verticalLayout.PackStart(CreatePaymentMethodsPanel(), true, true, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtFiscalNumber, TxtCardNumber), true, true, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtCustomer, TxtDiscount), true, true, 0);
            verticalLayout.PackStart(TxtAddress.Component, true, true, 0);
            verticalLayout.PackStart(TxtLocality.Component, true, true, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtZipCode, TxtCity, TxtCountry), true, true, 0);
            verticalLayout.PackStart(TxtNotes.Component, true, true, 0);

            return verticalLayout;
        }

        private HBox CreatePaymentMethodsPanel()
        {
            HBox hbox = new HBox(false, 0);
            PaymentMethodsMenu = new PaymentMethodsMenu(BtnPrevious, BtnNext,this);
            PaymentMethodsMenu.OnEntitySelected += PaymentMethodSelected;
            hbox.HeightRequest = AppSettings.Instance.SizeBaseDialogDefaultButton.Height + 10;
            hbox.PackStart(BtnPrevious, false, false, 0);
            hbox.PackStart(PaymentMethodsMenu, true, true, 0);
            hbox.PackEnd(BtnNext, false, false, 0);
            return hbox;
        }

        private EventBox CreateTotalsTable()
        {
            uint padding = 5;
            Gtk.Table table = new Gtk.Table(3, 2, false);
            table.HeightRequest = 100;

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
            eventBox.BorderWidth = 2;
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.ColorPosPaymentsDialogTotalPannelBackground.ToGdkColor());
            eventBox.Add(table);

            return eventBox;
        }
        private void InitializeButtons()
        {
            InitializeScrollersButtons();
            AddEventHandlers();
            BtnFullPayment.Sensitive = false;
        }
        private void InitializeTextFields()
        {
            InitializeTxtCustomer();
            InitializeTxtFiscalNumber();
            InitializeTxtCardNumber();
            InitializeTxtDiscount();
            InitializeTxtAddress();
            InitializeTxtLocality();
            InitializeTxtZipCode();
            InitializeTxtCity();
            InitializeTxtCountry();
            InitializeTxtNotes();
        }


    }
}
