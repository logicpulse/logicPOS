using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Movements
{
    public class StockMovementsFilterModal : Modal
    {
        #region Components
        private TextBox TxtStartDate { get; set; }
        private TextBox TxtEndDate { get; set; }
        private TextBox TxtArticle { get; set; }
        private TextBox TxtCustomer { get; set; }
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        #endregion

        public StockMovementsFilterModal(Window parent) :
            base(parent,
                LocalizedString.Instance["window_title_dialog_filter"],
                new Size(540, 568),
                PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_date_picker.png")
        {
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            AddEventHandlers();

            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnClear, ResponseType.None),
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            return new List<IValidatableField>
            {
                TxtStartDate,
                TxtEndDate
            };
        }

        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnClear.Clicked += BtnClear_Clicked;
        }

        public GetStockMovementsQuery GetStockMovementsQuery()
        {
            if(AllFieldsAreValid() == false)
            {
                return null;
            }

            var query = new GetStockMovementsQuery();

            if(string.IsNullOrWhiteSpace(TxtStartDate.Text) == false)
            {
                query.StartDate = DateTime.Parse(TxtStartDate.Text);
            }

            if (string.IsNullOrWhiteSpace(TxtEndDate.Text) == false)
            {
                query.EndDate = DateTime.Parse(TxtEndDate.Text);
            }

            if (TxtArticle.SelectedEntity != null)
            {
                var article = TxtArticle.SelectedEntity as Article;
                query.ArticleId = article.Id;
            }

            if (TxtCustomer.SelectedEntity != null)
            {
                var customer = TxtCustomer.SelectedEntity as Customer;
                query.CustomerId = customer.Id;
            }

            return query;
        }
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
            TxtCustomer.Clear();
        }

        protected override void OnResponse(ResponseType response)
        {
            if (response == ResponseType.None)
            {
                Run();
                return;
            }
        }

        protected override Widget CreateBody()
        {
            var verticalLayout = new VBox(false, 2);

            InitializeTextBoxes();
            verticalLayout.PackStart(TextBox.CreateHbox(TxtStartDate, TxtEndDate), false, false, 0);
            verticalLayout.PackStart(TxtArticle.Component, false, false, 0);
            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);

            return verticalLayout;
        }

        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtArticle();
            InitializeTxtCustomer();
        }

        private void InitializeTxtCustomer()
        {
            TxtCustomer = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_customer"),
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtCustomer.Entry.IsEditable = false;

            TxtCustomer.SelectEntityClicked += BtnSelectCustomer_Clicked;
        }

        private void BtnSelectCustomer_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, PageOptions.SelectionPageOptions);
            var selectCustomerModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCustomerModal.Run();
            selectCustomerModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtArticle()
        {
            TxtArticle = new TextBox(this,
                                              GeneralUtils.GetResourceByName("global_article"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: true,
                                              includeKeyBoardButton: false);

            TxtArticle.Entry.IsEditable = false;

            TxtArticle.SelectEntityClicked += BtnSelectArticle_Clicked;
        }

        private void BtnSelectArticle_Clicked(object sender, EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectArticleModal = new EntitySelectionModal<Article>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectArticleModal.Run();
            selectArticleModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtArticle.Text = page.SelectedEntity.Designation;
                TxtArticle.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtStartDate()
        {
            TxtStartDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_start"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtStartDate.Text = firstDayOfMonth.ToString("yyyy-MM-dd");

            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
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

        private void InitializeTxtEndDate()
        {
            TxtEndDate = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_date_end"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Date,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            TxtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
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
