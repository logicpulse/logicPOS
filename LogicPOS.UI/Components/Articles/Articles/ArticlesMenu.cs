using Gtk;
using logicpos.Classes.Enums.TicketList;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.GetAllArticles;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
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
        public string ButtonName { get; set; }
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
        public string ButtonNamePrefix { get; set; } = "buttonArticleId";
        public Color ButtonColor { get; set; } = Color.Transparent;
        public Size ButtonSize { get; set; } = new Size(176, 120);
        public Window SourceWindow { get; set; }
        public Article SelectedArticle { get; set; }
        public CustomButton SelectedButton { get; set; }
        public ArticleSubfamiliesMenu SubfamiliesMenu { get; set; }
        public List<Article> AllArticles { get; set; }
        public TicketList TicketList { get; set; }

        public ArticlesMenu(
            ArticleSubfamiliesMenu subfamiliesMenu,
            CustomButton btnPrevious,
            CustomButton btnNext,
            TicketList ticketList) : base(6, 7, true)
        {
            TicketList = ticketList;
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            SubfamiliesMenu = subfamiliesMenu;
            AddEventHandlers();
            LoadEntities();
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
                    Name = ButtonName,
                    BackgroundColor = ButtonColor,
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

        public void LoadEntities(bool favorites = false)
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

                ButtonName = $"{ButtonNamePrefix}_{article.Id}";
                ButtonLabel = article.Button.Label ?? ButtonName;
                ButtonImage = article.Button.Image ?? "";

                if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                {
                    ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                }

                currentButton = InitializeButton();
                currentButton.Clicked += Button_Clicked;
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

        private void Button_Clicked(object sender, EventArgs e)
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

            if (TicketList.ListMode != TicketListMode.Ticket)
            {
                TicketList.ListMode = TicketListMode.Ticket;
                TicketList.UpdateModel();
            }

            TicketList.InsertOrUpdate(new Guid("6aa32272-d0b0-40af-a0ee-b5aa38fa8b3e"));
        }

        internal void Refresh()
        {
            Buttons.Clear();
            LoadEntities();
        }
    }
}
