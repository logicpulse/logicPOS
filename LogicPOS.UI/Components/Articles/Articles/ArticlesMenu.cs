using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetAllArticles;
using LogicPOS.Api.Features.Articles.GetTotalStocks;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticlesMenu : Menu<Article>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public ArticleSubfamiliesMenu SubfamiliesMenu { get; }
        public IEnumerable<ArticleStock> Stocks { get; private set; }

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
            SubfamiliesMenu = subfamiliesMenu;
            LoadStocks();
            AddEventHandlers();
            PresentEntities();
        }

        private void LoadStocks()
        {
            var query = new GetArticlesTotalStocksQuery();
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(SourceWindow, result.FirstError);
                return;
            }

            Stocks = result.Value;
        }

        private void AddEventHandlers()
        {
            SubfamiliesMenu.SubfamilySelected += SubfamiliesMenu_SubfamilySelected;
            OnEntitySelected += BtnArticle_Clicked;
        }

        private void SubfamiliesMenu_SubfamilySelected(ArticleSubfamily subfamily)
        {
            Refresh();
        }

        protected override IEnumerable<Article> GetFilteredEntities()
        {
            if (SubfamiliesMenu.SelectedSubfamily == null)
            {
                return Entities;
            }

            return Entities.Where(a => a.SubfamilyId == SubfamiliesMenu.SelectedSubfamily.Id);
        }

        private void BtnArticle_Clicked(Article article)
        {
            var totalStock = Stocks.FirstOrDefault(x => x.Id == SelectedEntity.Id)?.Quantity ?? 0;

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

        protected override string GetButtonLabel(Article entity)
        {
            return entity.Button.Label ?? entity.Designation;
        }

        protected override string GetButtonImage(Article article)
        {
            if (string.IsNullOrEmpty(article.Button.ImageExtension) == false)
            {
                return ArticleImageRepository.GetImage(article.Id) ?? ArticleImageRepository.AddBase64Image(article.Id, article.Button.Image, article.Button.ImageExtension);
            }

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            var articles = _mediator.Send(new GetAllArticlesQuery()).Result;

            if (articles.IsError != false)
            {
                return;
            }

            Entities.AddRange(articles.Value);

        }
    }
}
