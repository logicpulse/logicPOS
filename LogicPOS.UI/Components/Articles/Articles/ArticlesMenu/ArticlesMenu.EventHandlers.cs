using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public partial class ArticlesMenu
    {
        private void AddEventHandlers()
        {
            MenuSubfamilies.FamiliesMenu.OnEntitySelected += OnFamilySelected;
            MenuSubfamilies.OnEntitySelected += OnSubfamilySelected;
            OnEntitySelected += BtnArticle_Clicked;
        }
        private void OnFamilySelected(ArticleFamily family)
        {
            Refresh();
        }

        private void OnSubfamilySelected(ArticleSubfamily subfamily)
        {
            CurrentQuery.Page = 1;
            Refresh();
        }

        public void BtnArticle_Clicked(ArticleViewModel article)
        {
            article = ArticlesService.GetArticleViewModel(article.Id);
            var totalStock = ArticlesService.GetArticleTotalStock(article.Id);

            if (totalStock - article.DefaultQuantity <= article.MinimumStock)
            {
                var message = $"{LocalizedString.Instance["window_check_stock_question"]}" +
                    $"\n\n{LocalizedString.Instance["global_article"]}: {article.Designation}" +
                    $"\n{LocalizedString.Instance["global_total_stock"]}: {totalStock:0.00}" +
                    $"\n{LocalizedString.Instance["global_minimum_stock"]}: {article.MinimumStock:0.00}";

                var stockWarningResponse = new CustomAlert(SourceWindow)
                                        .WithMessage(message)
                                        .WithSize(new Size(500, 350))
                                        .WithMessageType(MessageType.Question)
                                        .WithButtonsType(ButtonsType.YesNo)
                                        .WithTitleResource("global_stock_movements")
                                        .ShowAlert();

                if (stockWarningResponse == ResponseType.No)
                {
                    return;
                }
            }

            int priceType = SaleContext.CurrentTable.PriceTypeEnum;
            decimal price = article.GetPrice(priceType);

            if (price <= 0)
            {
                InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(SourceWindow, GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"), price);

                if (result.Response == ResponseType.Cancel)
                {
                    return;
                }

                article.SetPrice(priceType, result.Value);
            }

            var saleItem = new SaleItem(article, priceType);

            SaleContext.ItemsPage.AddItem(saleItem);
            POSWindow.Instance.SaleOptionsPanel.UpdateButtonsSensitivity();
        }

        protected override void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            CurrentQuery.GoToPreviousPage();
            Refresh();
        }

        protected override void BtnNext_Clicked(object obj, EventArgs args)
        {
            CurrentQuery.GoToNextPage();
            Refresh();
        }


    }
}
