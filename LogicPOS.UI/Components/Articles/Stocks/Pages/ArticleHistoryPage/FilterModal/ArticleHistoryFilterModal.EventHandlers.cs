using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles.Stocks.Pages.StockMovementsPage;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public partial class ArticleHistoryFilterModal : Modal
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
            TxtArticle.Clear();
            TxtSerialNumber.Clear();
        }
        private void ArticleAutocompleteLine_Selected(object article)
        {
            TxtArticle.SelectedEntity=article as ArticleViewModel;
        }
        private void BtnSelectSerialNumber_Clicked(object sender, EventArgs e)
        {
            var page = new ArticleHistoryPage(null, PageOptions.SelectionPageOptions);
            var selectCustomerModal = new EntitySelectionModal<ArticleHistory>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCustomerModal.Run();
            selectCustomerModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSerialNumber.Text = page.SelectedEntity.SerialNumber;
                TxtSerialNumber.SelectedEntity = page.SelectedEntity;
            }
        }
        private void BtnSelectArticle_Clicked(object sender, EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<ArticleViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtArticle.Text = page.SelectedEntity.Designation;
                TxtArticle.SelectedEntity = page.SelectedEntity;
            }
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
