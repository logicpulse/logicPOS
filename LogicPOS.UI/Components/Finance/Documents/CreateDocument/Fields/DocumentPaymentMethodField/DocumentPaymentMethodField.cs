using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodField : IValidatableField
    {


        public DocumentPaymentMethodField(Window sourceWindow)
        {
            InitializeTxtPaymentMethod(sourceWindow);
            InitializeTxtAmount(sourceWindow);
            InitializeButtons();

            Component = CreateComponent();
        }

        private Widget CreateComponent()
        {
            var hbox = new HBox(false, 0);
            hbox.PackStart(TxtPaymentMethod.Component, true, true, 0);
            hbox.PackStart(TxtAmount.Component, true, true, 0);

            var buttonsBox = new HBox(false, 2);
            buttonsBox.PackStart(BtnAdd, false, true, 0);
            buttonsBox.PackStart(BtnRemove, false, true, 0);

            hbox.PackStart(buttonsBox, false, false, 0);
            return hbox;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void TxtAmount_Changed(object sender, EventArgs e)
        {

        }

        private void AddButtonsEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
        }

        public bool IsValid()
        {
            return TxtPaymentMethod.IsValid() && TxtAmount.IsValid();
        }
    }
}
