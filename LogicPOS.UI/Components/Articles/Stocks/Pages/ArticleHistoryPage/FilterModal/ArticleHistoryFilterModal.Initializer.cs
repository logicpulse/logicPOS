using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Articles.Stocks.Modals.Filters
{
    public partial class ArticleHistoryFilterModal : Modal
    {
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
                                          isRequired: false,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtSerialNumber.Entry.IsEditable = true;

            TxtSerialNumber.SelectEntityClicked += BtnSelectSerialNumber_Clicked;
            var autoCompleteLines = ArticlesService.GetUniqueArticlesAutocompleteLines();
            TxtSerialNumber.WithAutoCompletion(autoCompleteLines, id => ArticlesService.GetArticleViewModel(id));
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
