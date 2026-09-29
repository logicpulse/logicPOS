using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public class DeleteOrderModal : Modal
    {
        private readonly Guid _orderId;

        public DeleteOrderModal(Guid orderId, Window parent) : base(parent,
                                                      "Eliminar Pedido",
                                                      new Size(500, 200),
                                                      AppSettings.Paths.Images + @"Icons\Windows\icon_window_document_new.png")
        {
            _orderId = orderId;
        }

        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public TextBox TxtReason { get; set; }


        protected override Widget CreateBody()
        {
            Initialize();

            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtReason.Component, false, false, 0);
            return verticalLayout;
        }

        private void Initialize()
        {
            BtnOk.Sensitive = false;
            BtnOk.Clicked += BtnOk_Clicked;
            InitializeTxtReason();
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            var result = OrdersService.DeleteOrder(SaleContext.CurrentOrder.Id.Value, TxtReason.Text);

            if (result == false)
            {
                Respond(ResponseType.Cancel);
            }
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }


        private void InitializeTxtReason()
        {
            TxtReason = new TextBox(WindowSettings.Source,
                                       "Motivo",
                                       isRequired: true,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtReason.Entry.Changed += (sender, args) =>
            {
                BtnOk.Sensitive = !string.IsNullOrWhiteSpace(TxtReason.Text);
            };
        }


    }
}
