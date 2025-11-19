using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsFilterModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            TxtStartDate.Clear();
            TxtEndDate.Clear();
            TxtDocumentType.Clear();
            TxtCustomer.Clear();
            TxtPaymentMethod.Clear();
            TxtPaymentCondition.Clear();
        }

        private void TxtCustomer_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCustomer.Text))
            {
                TxtCustomer.Clear();
            }
        }

        private void TxtDocumentType_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtDocumentType.Text))
            {
                TxtDocumentType.Clear();
            }
        }

        private void TxtPaymentMethod_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtPaymentMethod.Text))
            {
                TxtPaymentMethod.Clear();
            }
        }

        private void TxtPaymentCondition_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtPaymentCondition.Text))
            {
                TxtPaymentCondition.Clear();
            }
        }
       
        private void BtnSelectCustomer_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, CustomersPage.CustomerSelectionOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
            }
        }
       
        private void BtnSelectDocumentType_Clicked(object sender, EventArgs e)
        {
            var page = new DocumentTypesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentType.Text = page.SelectedEntity.Designation;
                TxtDocumentType.SelectedEntity = page.SelectedEntity;
            }
        }
        
        private void BtnSelectPaymentCondition_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentConditionsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentConditionModal = new EntitySelectionModal<PaymentCondition>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentConditionModal.Run();
            selectPaymentConditionModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentCondition.Text = page.SelectedEntity.Designation;
                TxtPaymentCondition.SelectedEntity = page.SelectedEntity;
            }
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
       
        private void TxtStartDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtStartDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }
        
        private void TxtEndDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtEndDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }
    }
}
