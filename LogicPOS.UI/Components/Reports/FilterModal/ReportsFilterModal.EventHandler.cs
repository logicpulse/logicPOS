

using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using Customer = LogicPOS.Api.Features.Finance.Customers.Customers.Common.Customer;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsFilterModal
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
            TxtWarehouse.Clear();
            TxtVatRate.Clear();
            TxtFamily.Clear();
            TxtSubfamily.Clear();
            TxtArticle.Clear();
        }

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
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

        private void BtnSelectVatRate_Clicked(object sender, EventArgs e)
        {
            var page = new VatRatesPage(null, PageOptions.SelectionPageOptions);
            var selectVatRateModal = new EntitySelectionModal<VatRate>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectVatRateModal.Run();
            selectVatRateModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtVatRate.Text = page.SelectedEntity.Designation;
                TxtVatRate.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectWarehouse_Clicked(object sender, EventArgs e)
        {
            var page = new WarehousesPage(null, PageOptions.SelectionPageOptions);
            var selectWarehouseModal = new EntitySelectionModal<Warehouse>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectWarehouseModal.Run();
            selectWarehouseModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtWarehouse.Text = page.SelectedEntity.Designation;
                TxtWarehouse.SelectedEntity = page.SelectedEntity;
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

        private void TxtStartDate_Entry_Changed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtStartDate.Text) && TxtStartDate.Text.Length >= 10)
            {
                if (TxtStartDate.IsValid())
                {
                    TxtStartDate.Text = TxtStartDate.Text.ValidateDate();
                }
                return;
            }
        }

        private void TxtEndDate_Entry_Changed(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(TxtEndDate.Text)) && TxtEndDate.Text.Length >= 10)
            {
                if (TxtEndDate.IsValid())
                {
                    TxtEndDate.Text = TxtEndDate.Text.ValidateDate();
                }
                return;
            }
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

        private void BtnSelectCustomer_Clicked(object sender, System.EventArgs e)
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

        private void BtnSelectFamily_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticleFamiliesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<ArticleFamily>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtFamily.Text = page.SelectedEntity.Designation;
                TxtFamily.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectSubfamily_Clicked(object sender, System.EventArgs e)
        {
            if (TxtFamily.SelectedEntity != null)
            {
                ArticleSubfamiliesPage.FamilyId=(TxtFamily.SelectedEntity as ArticleFamily).Id;
            }
            var page = new ArticleSubfamiliesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<ArticleSubfamily>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSubfamily.Text = page.SelectedEntity.Designation;
                TxtSubfamily.SelectedEntity = page.SelectedEntity;
            }
            ArticleSubfamiliesPage.FamilyId = Guid.Empty;
        }
        private void BtnSelectArticle_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<ArticleViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtArticle.Text = page.SelectedEntity.Designation;
                TxtArticle.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectSerialNumber_Clicked(object sender, System.EventArgs e)
        {
            var page = new ArticleHistoryPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<ArticleHistory>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSerialNumber.Text = page.SelectedEntity.SerialNumber;
                TxtSerialNumber.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectDocumentNumber_Clicked(object sender, System.EventArgs e)
        {
            var page = new DocumentsPage(this, PageOptions.SelectionPageOptions);
            var selectDocumentModal = new EntitySelectionModal<DocumentViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentModal.Run();
            selectDocumentModal.Destroy();
            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentNumber.Text = page.SelectedEntity.Number;
                TxtDocumentNumber.SelectedEntity = page.SelectedEntity;
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.None)
            {
                Run();
                return;
            }
        }

    }
}
