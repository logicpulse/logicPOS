using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Articles.StockManagement.GetTotalStocks;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticlesMenu : Menu<ArticleViewModel>
    {
        public ArticleSubfamiliesMenu MenuSubfamilies { get; }
        private PaginatedResult<ArticleViewModel> Articles { get; set; }
        public bool PresentFavorites { get;  set; }

        public ArticlesMenu(CustomButton btnPrevious,
                            CustomButton btnNext,
                            Window sourceWindow,
                            ArticleSubfamiliesMenu subfamiliesMenu) : base(rows: 6,
                                                                           columns: 7,
                                                                           buttonSize: new Size(176, 120),
                                                                           buttonName: "buttonArticleId",
                                                                           btnPrevious,
                                                                           btnNext,
                                                                           sourceWindow,
                                                                           toggleMode: false)
        {
            MenuSubfamilies = subfamiliesMenu;
            AddEventHandlers();
            PresentEntities();
        }

        private void AddEventHandlers()
        {
            MenuSubfamilies.OnEntitySelected += OnSubfamilySelected;
            OnEntitySelected += BtnArticle_Clicked;
        }

        private void OnSubfamilySelected(ArticleSubfamily subfamily)
        {
            Refresh();
        }

        protected override IEnumerable<ArticleViewModel> GetFilteredEntities()
        {
            var query = new GetArticlesQuery
            {
                SubFamilyId = MenuSubfamilies.SelectedEntity?.Id,
                Favorite = PresentFavorites,
                Page = Articles.Page,
                PageSize = Articles.PageSize
            };

            Articles = ArticlesService.GetArticles(query);

            return Articles.Items;
        }

        private void BtnArticle_Clicked(ArticleViewModel article)
        {
            var totalStock =  ArticleTotalStockService.GetArticleTotalStock(article.Id);

            if (totalStock - SelectedEntity.DefaultQuantity <= SelectedEntity.MinimumStock)
            {
                var message = $"{GeneralUtils.GetResourceByName("window_check_stock_question")}\n\n{GeneralUtils.GetResourceByName("global_article")}: {SelectedEntity.Designation}\n{GeneralUtils.GetResourceByName("global_total_stock")}: {totalStock}\n{GeneralUtils.GetResourceByName("global_minimum_stock")}: {SelectedEntity.MinimumStock.ToString()}";

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

            var item = new SaleItem(SelectedEntity);


            if (item.UnitPrice <= 0)
            {
                InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(SourceWindow, GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"), item.UnitPrice);

                if (result.Response == ResponseType.Cancel)
                {
                    return;
                }

                item.UnitPrice = result.Value;
            }


            SaleContext.ItemsPage.AddItem(item);
            POSWindow.Instance.SaleOptionsPanel.UpdateButtonsSensitivity();
        }

        protected override string GetButtonLabel(ArticleViewModel entity)
        {
            return entity.ButtonLabel ?? entity.Designation;
        }

        protected override string GetButtonImage(ArticleViewModel article)
        {
            //if (string.IsNullOrEmpty(article.Button.ImageExtension) == false)
            //{
            //    return ArticleImageRepository.GetImage(article.Id) ?? ArticleImageRepository.AddBase64Image(article.Id, article.Button.Image, article.Button.ImageExtension);
            //}

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            Articles = ArticlesService.GetArticles(new GetArticlesQuery());

            Entities.AddRange(Articles.Items);
        }
    }
}
