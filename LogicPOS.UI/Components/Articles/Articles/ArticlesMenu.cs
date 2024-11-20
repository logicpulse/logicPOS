using Gtk;
using logicpos;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared.Enums;
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
    public class ArticlesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(Article Article, CustomButton Button)> Buttons { get; set; } = new List<(Article, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = false;
        public Article InitialArticle { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 6;
        public uint Columns { get; set; } = 7;
        public Size ButtonSize { get; set; } = new Size(176, 120);
        public Window SourceWindow { get; set; }
        public Article SelectedArticle { get; set; }
        public CustomButton SelectedButton { get; set; }
        public ArticleSubfamiliesMenu SubfamiliesMenu { get; set; }
        public List<Article> AllArticles { get; set; }
        public IEnumerable<ArticleStock> Stocks { get; private set; }
        public SaleItemsPage SaleItemsPage { get;  }

        public ArticlesMenu(ArticleSubfamiliesMenu subfamiliesMenu,
                            CustomButton btnPrevious,
                            CustomButton btnNext,
                            SaleItemsPage saleItemsPage) : base(6, 7, true)
        {
            SaleItemsPage = saleItemsPage;
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            SubfamiliesMenu = subfamiliesMenu;
            AddEventHandlers();
            LoadStocks();
            PresentArticles();
        }


        private void LoadStocks()
        {
            var query = new GetArticlesTotalStocksQuery();
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(SourceWindow,result.FirstError);
                return;
            }

            Stocks = result.Value;
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
            SubfamiliesMenu.SubfamilySelected += SubfamiliesMenu_SubfamilySelected;
        }

        private void SubfamiliesMenu_SubfamilySelected(ArticleSubfamily subfamily)
        {
            Refresh();
        }

        public virtual CustomButton InitializeButton()
        {
            return new ImageButton(
                new ButtonSettings
                {
                    Name = "buttonArticleId",
                    Text = ButtonLabel,
                    FontSize = ButtonFontSize,
                    Image = ButtonImage,
                    Overlay = ButtonOverlay,
                    ButtonSize = ButtonSize
                });
        }

        public void Update()
        {
            RemoveOldButtons();
            AddItems();
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            if (CurrentPage == 1)
            {
                BtnPrevious.Sensitive = false;
            }
            else
            {
                BtnPrevious.Sensitive = true;
            }

            if (CurrentPage == TotalPages)
            {
                BtnNext.Sensitive = false;
            }
            else
            {
                if (TotalPages > 1) BtnNext.Sensitive = true;
            }
        }

        private void AddItems()
        {
            if (Buttons.Count <= 0)
            {
                return;
            }

            uint currentRow = 0, currentColumn = 0;
            int startItem = (CurrentPage * ItemsPerPage) - ItemsPerPage;
            int endItem = startItem + ItemsPerPage - 1;
            for (int i = startItem; i <= endItem; i++)
            {
                if (i < TotalItems)
                {
                    this.Attach(Buttons[i].Button, currentColumn, currentColumn + 1, currentRow, currentRow + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                }

                if (currentColumn == this.NColumns - 1)
                {
                    ++currentRow;
                    currentColumn = 0;
                }
                else
                {
                    ++currentColumn;
                }
            }

        }

        private void RemoveOldButtons()
        {
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                this.Remove(this.Children[0]);
            }
        }

        private void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            CurrentPage -= 1;
            Update();
        }

        private void BtnNext_Clicked(object obj, EventArgs args)
        {
            CurrentPage += 1;
            Update();
        }

        public void PresentArticles(bool favorites = false)
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;
            CustomButton currentButton = null;

            SelectedArticle = (InitialArticle != null) ? InitialArticle : null;

            Buttons.Clear();

            if (AllArticles == null)
            {
                LoadAllArticles();
            }

            List<Article> articles = favorites ? GetFavoriteArticles() : FilterArticlesBySubfamily();

            foreach (var article in articles)
            {
                if (SelectedArticle == null)
                {
                    SelectedArticle = article;

                    if (InitialArticle == null)
                    {
                        InitialArticle = SelectedArticle;
                    }
                }

                ButtonLabel = article.Button.Label ?? article.Designation;

                if (string.IsNullOrEmpty(article.Button.ImageExtension) == false)
                {
                    ButtonImage = ArticleImageRepository.GetImage(article.Id) ?? ArticleImageRepository.AddBase64Image(article.Id, article.Button.Image, article.Button.ImageExtension);
                }
                else
                {
                    ButtonImage = null;
                }

                if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                {
                    ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                }

                currentButton = InitializeButton();
                currentButton.Clicked += BtnArticle_Clicked;
                Buttons.Add((article, currentButton));
                currentButton.CurrentButtonId = article.Id;

                if (article.Id == InitialArticle.Id)
                {
                    if (ToggleMode)
                    {
                        currentButton.Sensitive = false;
                    }
                    SelectedButton = currentButton;
                }

            }

            TotalItems = Buttons.Count;
            ItemsPerPage = Convert.ToInt16(Rows * Columns);
            TotalPages = (int)Math.Ceiling((float)TotalItems / (float)ItemsPerPage);

            Update();
        }

        private List<Article> FilterArticlesBySubfamily()
        {
            if (SubfamiliesMenu.SelectedSubfamily == null)
            {
                return new List<Article>();
            }

            return AllArticles.Where(a => a.SubfamilyId == SubfamiliesMenu.SelectedSubfamily.Id).ToList();
        }

        private List<Article> GetFavoriteArticles()
        {
            return AllArticles?.Where(a => a.Favorite).ToList();
        }

        private void LoadAllArticles()
        {
            var articles = _mediator.Send(new GetAllArticlesQuery()).Result;
            if (articles.IsError == false)
            {
                AllArticles = articles.Value.ToList();
                return;
            }

            AllArticles = new List<Article>();
        }

        private void BtnArticle_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            if (ToggleMode && SelectedButton != null)
            {
                SelectedButton.Sensitive = true;
            }

            SelectedButton = button;

            if (ToggleMode)
            {
                SelectedButton.Sensitive = false;
            }

            SelectedArticle = Buttons.Find(x => x.Button == SelectedButton).Article;
            var totalStock = Stocks.FirstOrDefault(x => x.Id == SelectedArticle.Id)?.Quantity ?? 0;

            if (totalStock - SelectedArticle.DefaultQuantity <= SelectedArticle.MinimumStock)
            {
                var message= $"{GeneralUtils.GetResourceByName("window_check_stock_question")}\n\n{GeneralUtils.GetResourceByName("global_article")}: {SelectedArticle.Designation}\n{GeneralUtils.GetResourceByName("global_total_stock")}: {totalStock}\n{GeneralUtils.GetResourceByName("global_minimum_stock")}: {SelectedArticle.MinimumStock.ToString()}";
                
                var stockWarningResponse = new CustomAlert(SourceWindow)
                                        .WithMessage(message)
                                        .WithSize(new Size(500,350))
                                        .WithMessageType(MessageType.Question)
                                        .WithButtonsType(ButtonsType.YesNo)
                                        .WithTitleResource("global_stock_movements")
                                        .ShowAlert();
                
                if (stockWarningResponse == ResponseType.No)
                {
                    return;
                }
            }

            var item = new SaleItem(SelectedArticle);


            if(item.UnitPrice <= 0)
            {
                InsertMoneyModalResponse result = InsertMoneyModal.RequestDecimalValue(SourceWindow, GeneralUtils.GetResourceByName("window_title_dialog_moneypad_product_price"), item.UnitPrice);
              
                if (result.Response == ResponseType.Cancel)
                {
                    return;
                }

                item.UnitPrice = result.Value;
            }
           

            SaleItemsPage.AddItem(item);
        }

        internal void Refresh()
        {
            Buttons.Clear();
            PresentArticles();
        }
    }
}
