using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public partial class VoltaRefundReceiptModal
    {
        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
            TxtQuantity.Entry.Changed += (sender, args) => UpdateTotal();
            TxtPaymentMethod.Entry.Changed += TxtPaymentMethod_Changed;
            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectCustomer_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, CustomersPage.CustomerSelectionOptions);
            var selectCustomerModal = new EntitySelectionModal<Api.Features.Finance.Customers.Customers.Common.Customer>(
                page,
                LocalizedString.Instance["window_title_dialog_select_record"]);

            ResponseType response = (ResponseType)selectCustomerModal.Run();
            selectCustomerModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                SelectCustomer(page.SelectedEntity);
            }
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
        }

        private void TxtPaymentMethod_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPaymentMethod.Text))
            {
                TxtPaymentMethod.Clear();
            }
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (Validate() == false)
            {
                Run();
                return;
            }

            var paymentMethod = TxtPaymentMethod.SelectedEntity as PaymentMethod;
            var issued = VoltaRefundReceiptService.TryIssue(
                this,
                _selectedCustomer,
                paymentMethod,
                GetQuantity(),
                (TxtOriginDocument.SelectedEntity as DocumentViewModel)?.Id);

            if (issued)
            {
                Respond(ResponseType.Ok);
                return;
            }

            Run();
        }

        private void BtnSelectOriginDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(this, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Selection);
            ResponseType response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok && modal.Page.SelectedEntity != null)
            {
                string[] allowedTypes = new string[] { "FR", "FS" };
                if (modal.Page.SelectedEntity.IsActive == false || allowedTypes.Contains(modal.Page.SelectedEntity.Type) == false)
                {
                    CustomAlerts.Warning(this)
                     .WithTitle("Documento inválido")
                     .WithMessage("Não é possível selecionar um documento que se encontra cancelado/anulado ou em rascunho. Tipos permitidos: FS, FR.")
                     .ShowAlert();

                    modal.Destroy();

                    return;
                }

                TxtOriginDocument.Text = modal.Page.SelectedEntity.Number;
                TxtOriginDocument.SelectedEntity = modal.Page.SelectedEntity;
            }

            modal.Destroy();
        }

        private bool Validate()
        {
            if (_selectedCustomer == null || string.IsNullOrWhiteSpace(TxtCustomer.Text))
            {
                CustomAlerts.Warning(this)
                    .WithMessage("Selecione o cliente.")
                    .ShowAlert();
                return false;
            }

            var quantity = GetQuantity();
            if (quantity <= 0)
            {
                CustomAlerts.Warning(this)
                    .WithMessage("Indique uma quantidade válida.")
                    .ShowAlert();
                return false;
            }

            var paymentMethod = TxtPaymentMethod.SelectedEntity as PaymentMethod;
            if (paymentMethod == null)
            {
                CustomAlerts.Warning(this)
                    .WithMessage("Selecione o método de pagamento.")
                    .ShowAlert();
                return false;
            }

            if (string.Equals(paymentMethod.Acronym, TrvDocumentUiRules.PaymentMethodAcronym, StringComparison.OrdinalIgnoreCase) == false)
            {
                CustomAlerts.Warning(this)
                    .WithTitle("Método de pagamento inválido")
                    .WithMessage("O Talão Reembolso Volta só pode ser emitido com o método de pagamento OU (Outros).")
                    .ShowAlert();
                return false;
            }

            return true;
        }
    }
}
