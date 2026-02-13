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
       
        protected override Widget CreateBody()
        {
            var verticalLayout = new VBox(false, 2);

            InitializeTextBoxes();
            verticalLayout.PackStart(TextBox.CreateHbox(TxtStartDate, TxtEndDate), false, false, 0);
            verticalLayout.PackStart(TxtArticle.Component, false, false, 0);
            verticalLayout.PackStart(TxtSerialNumber.Component, false, false, 0);

            return verticalLayout;
        }

        private void InitializeTextBoxes()
        {
            InitializeTxtStartDate();
            InitializeTxtEndDate();
            InitializeTxtArticle();
            InitializeTxtSerialNumber();
        }

        private void InitializeTxtSerialNumber()
        {
            TxtSerialNumber = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_serial_number"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtSerialNumber.Entry.IsEditable = true;

            TxtSerialNumber.SelectEntityClicked += BtnSelectSerialNumber_Clicked;
        }

        private void InitializeTxtArticle()
        {
            TxtArticle = new TextBox(this,
                                     GeneralUtils.GetResourceByName("global_article"),
                                     isRequired: false,
                                     isValidatable: false,
                                     includeSelectButton: true,
                                     includeKeyBoardButton: false);

            TxtArticle.SelectEntityClicked += BtnSelectArticle_Clicked;
            TxtArticle.WithAutoCompletion(ArticlesService.AutocompleteLines, id => ArticlesService.GetArticleViewModel(id));
            TxtArticle.OnCompletionSelected += ArticleAutocompleteLine_Selected;
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
            TxtStartDate.Entry.Changed += TxtStartDate_Entry_Changed;
            TxtStartDate.SelectEntityClicked += TxtStartDate_SelectEntityClicked;
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
            TxtEndDate.Entry.Changed += TxtEndDate_Entry_Changed;
            TxtEndDate.SelectEntityClicked += TxtEndDate_SelectEntityClicked;
        }
    }
}
